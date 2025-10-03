namespace CmdBased
{
    class Example
    {
        static readonly int TICK_TIME = 20; // in milliseconds
        static CommandScheduler Scheduler = new();

        static readonly double TICK_TIME_SEC =
            (double)TICK_TIME / 1000;

        /* subsystems */
        static DriveSubsystem DriveSub;
        static MotorSubsystem Motor1 = new("Motor1");

        static readonly double JSTICK_MAX_SPEED = 2.0 * TICK_TIME_SEC;
        // maximum drive velocity from joystick input

        static bool AdvancedConsole = true; // set if console supports clearing

        static int ConsoleWidth, ConsoleHeight;

        public static void PrintStatus()
        {
            if (!AdvancedConsole) // show robot state on one line
                Console.WriteLine($"{DriveSub}, {Motor1}");
            else
            {
                Console.Clear();

                /* draw robot position */
                int xPosition = (int)DriveSub.XPosition;
                if (xPosition >= ConsoleWidth) xPosition = ConsoleWidth - 1;
                int yPosition = (int)DriveSub.YPosition / 2;
                if (yPosition >= ConsoleHeight - 1) xPosition = ConsoleHeight - 2;
                Console.SetCursorPosition(
                    (int)DriveSub.XPosition, (int)DriveSub.YPosition / 2
                );
                Console.Write(
                    ((int)DriveSub.YPosition % 2 == 0) ? "\u2580" : "\u2584"
                );

                /* print motor status */
                Console.SetCursorPosition(0, ConsoleHeight - 1);
                Console.Write($"{DriveSub}, {Motor1}");
            }
        }

        public static void Main()
        {
            /* test if advanced console features are supported */
            try
            {
                Console.Clear();
                ConsoleWidth = Console.WindowWidth;
                ConsoleHeight = Console.WindowHeight;
                // Console.CursorVisible = false;
            }
            catch (IOException)
            {
                AdvancedConsole = false;
                Console.WriteLine(
                    "Advanced console features are not supported"
                );
            }

            if (AdvancedConsole)
                DriveSub = new(ConsoleWidth / 2, (ConsoleHeight * 2 - 1) / 2);
            // NOTE: The robot space's dimensions will be ConsoleWidth by
            //       ConsoleHeight * 2 - 1, with half block characters used to
            //       represent the robot.
            else
                DriveSub = new();

            /* register subsystems */
            Scheduler.RegisterSubsystem(DriveSub);
            Scheduler.RegisterSubsystem(Motor1);

            using (var gamepad = new Gamepad()) // default to /dev/input/js0
            {
                gamepad.RegisterTriggers(Scheduler); // register all buttons

                /* drivetrain default command: joystick teleoperation */
                DriveSub.DefaultCommand = new RunCommand(() =>
                {
                    double xInput = gamepad.Axes[(int)Gamepad.AxisIDs.LEFT_X];
                    double yInput = gamepad.Axes[(int)Gamepad.AxisIDs.LEFT_Y];
                    DriveSub.Drive(
                        xInput * JSTICK_MAX_SPEED, yInput * JSTICK_MAX_SPEED
                    );
                }, [DriveSub]);

                /* press TRIANGLE to run Motor1 for 1 sec */
                gamepad.Triggers[(int)Gamepad.ButtonIDs.TRIANGLE].OnTrue(
                    Motor1.RunStopCommand(1.0)
                        .WithTimeout((int)(1 / TICK_TIME_SEC))
                );

                /* press CIRCLE to do the same, but then reverse when rel'd */
                gamepad.Triggers[(int)Gamepad.ButtonIDs.CIRCLE].OnTrue(
                    Motor1.RunStopCommand(1.0)
                        .WithTimeout((int)(1 / TICK_TIME_SEC))
                ).OnFalse(
                     Motor1.RunStopCommand(-1.0)
                        .WithTimeout((int)(1 / TICK_TIME_SEC))
                );

                /* hold START to run drivetrain in a square */
                gamepad.Triggers[(int)Gamepad.ButtonIDs.START].WhileTrue(
                    new SequentialCommandGroup([
                        new RunCommand(() => {
                            DriveSub.Drive(0.25, 0.0);
                        }, [DriveSub]).WithTimeout((int)(1 / TICK_TIME_SEC)),
                        new RunCommand(() => {
                            DriveSub.Drive(0.0, 0.25);
                        }, [DriveSub]).WithTimeout((int)(1 / TICK_TIME_SEC)),
                        new RunCommand(() => {
                            DriveSub.Drive(-0.25, 0.0);
                        }, [DriveSub]).WithTimeout((int)(1 / TICK_TIME_SEC)),
                        new RunCommand(() => {
                            DriveSub.Drive(0.0, -0.25);
                        }, [DriveSub]).WithTimeout((int)(1 / TICK_TIME_SEC))
                    ]).Perpetually()
                );
                // NOTE: this should block the default command from running

                while (true)
                {
                    Scheduler.Periodic();
                    PrintStatus(); // periodically log robot status
                    Thread.Sleep(TICK_TIME);
                }
            }

        }
    }
}