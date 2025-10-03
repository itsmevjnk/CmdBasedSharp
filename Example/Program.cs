namespace CmdBased
{
    class Example
    {
        static readonly int TICK_TIME = 20; // in milliseconds
        static CommandScheduler Scheduler = new();

        static readonly double TICK_TIME_SEC =
            (double)TICK_TIME / 1000;

        /* subsystems */
        static DriveSubsystem DriveSub = new();
        static MotorSubsystem Motor1 = new("Motor1");

        static readonly double JSTICK_MAX_SPEED = 2.0 * TICK_TIME_SEC;
        // maximum drive velocity from joystick input

        public static void PrintStatus()
        {
            Console.WriteLine(
                $"{DriveSub}, {Motor1}"
            );
        }

        public static void Main()
        {
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
                            DriveSub.Drive(1.0, 0.0);
                        }, [DriveSub]).WithTimeout((int)(1 / TICK_TIME_SEC)),
                        new RunCommand(() => {
                            DriveSub.Drive(0.0, 1.0);
                        }, [DriveSub]).WithTimeout((int)(1 / TICK_TIME_SEC)),
                        new RunCommand(() => {
                            DriveSub.Drive(-1.0, 0.0);
                        }, [DriveSub]).WithTimeout((int)(1 / TICK_TIME_SEC)),
                        new RunCommand(() => {
                            DriveSub.Drive(0.0, -1.0);
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