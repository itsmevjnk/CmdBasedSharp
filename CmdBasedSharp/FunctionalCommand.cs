namespace CmdBased
{
    public class FunctionalCommand : CommandBase
    {
        private Action InitFunction;
        private Action ExecFunction;
        private Func<bool> FinishPredicate;
        private Action<bool> EndFunction;

        public FunctionalCommand(Action onInit, Action onExecute,
                                 Action<bool> onEnd, Func<bool> isFinished,
                                 ICollection<SubsystemBase>? requirements = null,
                                 bool interruptible = true)
        {
            InitFunction = onInit;
            ExecFunction = onExecute;
            EndFunction = onEnd;
            FinishPredicate = isFinished;
            Requirements = requirements ?? [];
            IsInterruptibleFlag = interruptible;
        }

        public override void Initialise()
        {
            InitFunction();
        }

        public override void Execute()
        {
            ExecFunction();
        }

        public override void End(bool interrupted)
        {
            EndFunction(interrupted);
        }

        public override bool IsFinished
        {
            get { return FinishPredicate(); }
        }

        private bool IsInterruptibleFlag;
        public override bool IsInterruptible
        {
            get { return IsInterruptibleFlag; }
        }
    }
}