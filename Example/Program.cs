namespace CmdBased
{
    class Example
    {
        public static void Main()
        {
            CommandScheduler scheduler = new();

            Gamepad gamepad = new(); // default device is /dev/input/js0

            scheduler.RegisterTriggers
            (
                gamepad.Triggers[(int)Gamepad.ButtonIDs.TRIANGLE]
                .OnTrue
                (
                    new InstantCommand(() => { Console.WriteLine("Triangle pressed"); })
                )
            );

            while (true)
            {
                scheduler.Periodic();
                Thread.Sleep(20);
            }
        }
    }
}