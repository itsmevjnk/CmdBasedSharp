namespace CmdBased
{
    public class ParallelDeadlineGroup : ParallelCommandGroup
    {
        protected CommandBase Deadline;

        public ParallelDeadlineGroup(CommandBase deadline,
                                     ICollection<CommandBase> commands)
            : base(commands) { Deadline = deadline; }

        public override void Initialise()
        {
            Deadline.Initialise();
            base.Initialise();
        }

        public override void Execute()
        {
            Deadline.Initialise();
            base.Execute();
        }

        public override void End(bool interrupted)
        {
            Deadline.End(interrupted);
            base.End(interrupted);
        }

        public override bool IsFinished
        {
            get { return base.IsFinished || Deadline.IsFinished; }
        }

        public override bool IsInterruptible
        {
            get { return base.IsInterruptible && Deadline.IsInterruptible; }
        }
    }
}