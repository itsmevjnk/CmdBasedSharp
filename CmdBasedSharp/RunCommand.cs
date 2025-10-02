namespace CmdBased
{
    public class RunCommand : CommandBase
    {
        private Procedure ExecFunction;

        private bool IsInterruptibleFlag;
        public override bool IsInterruptible
        {
            get { return IsInterruptibleFlag; }
        }

        public RunCommand(Procedure func, ICollection<SubsystemBase>? requirements = null, bool interruptible = true)
        {
            ExecFunction = func;
            IsInterruptibleFlag = interruptible;
            Requirements = requirements ?? [];
        }

        public override void Execute()
        {
            ExecFunction();
        }

        public override bool IsFinished
        {
            get { return false; }
        }
    }
}