namespace CmdBased
{
    public class InstantCommand : CommandBase
    {
        private Action InitFunction;

        private bool IsInterruptibleFlag;
        public override bool IsInterruptible
        {
            get { return IsInterruptibleFlag; }
        }

        public InstantCommand(Action func, 
                              ICollection<SubsystemBase>? requirements = null,
                              bool interruptible = true)
        {
            InitFunction = func;
            IsInterruptibleFlag = interruptible;
            Requirements = requirements ?? [];
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