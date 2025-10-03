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

        static readonly double JSTICK_MAX_SPEED = 2.0 * TICK_TIME_SEC;
        // maximum drive velocity from joystick input

        public static void PrintStatus()
        {
            Console.WriteLine(DriveSub); // TODO
        }

        public static void Main()
        {
            /* register subsystems */
            Scheduler.RegisterSubsystem(DriveSub);

            using (var gamepad = new Gamepad()) // default device is /dev/input/js0
            {
                Scheduler.RegisterTriggers
                (
                    gamepad.Triggers[(int)Gamepad.ButtonIDs.TRIANGLE]
                    .OnTrue
                    (
                        new InstantCommand(() => { Console.WriteLine("Triangle pressed"); })
                    )
                );

                DriveSub.DefaultCommand = new RunCommand(() =>
                {
                    DriveSub.Drive(
                        gamepad.Axes[(int)Gamepad.AxisIDs.LEFT_X] * JSTICK_MAX_SPEED,
                        gamepad.Axes[(int)Gamepad.AxisIDs.LEFT_Y] * JSTICK_MAX_SPEED
                    );
                }, [DriveSub]); // default command: set drive speed to joystick input

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