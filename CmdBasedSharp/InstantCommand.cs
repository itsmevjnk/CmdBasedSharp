namespace CmdBased
{
    public class InstantCommand : CommandBase
    {
        private Procedure InitFunction;

        private bool IsInterruptibleFlag;
        public override bool IsInterruptible
        {
            get { return IsInterruptibleFlag; }
        }

        public InstantCommand(Procedure func, bool interruptible)
        {
            InitFunction = func;
            IsInterruptibleFlag = interruptible;
        }

        public override void Initialise()
        {
            InitFunction();
        }

        public override bool IsFinished
        {
            get { return true; }
        }
    }
}