namespace CmdBased
{
    public class StartEndCommand : CommandBase
    {
        private Action InitFunction;
        private Action EndFunction;

        private bool IsInterruptibleFlag;
        public override bool IsInterruptible
        {
            get { return IsInterruptibleFlag; }
        }

        public StartEndCommand(Action initFunc, Action endFunc,
                              ICollection<SubsystemBase>? requirements = null,
                              bool interruptible = true)
        {
            InitFunction = initFunc;
            EndFunction = endFunc;
            Requirements = requirements ?? [];
            IsInterruptibleFlag = interruptible;
        }

        public override void Initialise()
        {
            InitFunction();
        }

        public override void End(bool interrupted)
        {
            EndFunction();
        }

        public override bool IsFinished
        {
            get { return false; } // do not end immediately
        }
    }
}