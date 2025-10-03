namespace CmdBased
{
    public class TimeoutCommand : CommandBase
    {
        private int Expiry;

        public TimeoutCommand(int ticks)
        {
            Expiry = ticks;
        }

        private int Elapsed = 0;

        public override void Initialise()
        {
            Elapsed = 0;
        }

        public override void Execute()
        {
            Elapsed++;
        }

        public override bool IsFinished
        {
            get { return Elapsed > Expiry; }
            // NOTE: The first Execute() invocation will occur right after
            //       Initialise(), so Elapsed starts from 1.
        }

        public override bool IsInterruptible
        {
            get { return true; }
        }
    }
}