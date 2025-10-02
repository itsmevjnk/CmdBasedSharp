namespace CmdBased
{
    public abstract class CommandBase
    {
        public virtual void Initialise() { }
        public virtual void Execute() { }
        public virtual void End(bool interrupted) { }
        public virtual bool IsFinished
        {
            get { return true; }
        }

        public virtual bool IsInterruptible
        {
            get { return true; }
        }

        public ICollection<SubsystemBase> Requirements = [];

        /* doubly linked list of scheduled commands */
        internal CommandBase? PrevScheduled = null;
        internal CommandBase? NextScheduled = null;
        // NOTE: the command is treated as unscheduled if both are null

        internal void AddScheduled(CommandBase command) // add after this
        {
            if (command.IsScheduled) throw new ArgumentException();
            command.NextScheduled = NextScheduled;
            if (NextScheduled != null) NextScheduled.PrevScheduled = command;
            command.PrevScheduled = this;
            NextScheduled = command;
        }

        internal void RemoveScheduled() // remove this
        {
            if (!IsScheduled || PrevScheduled == null)
                throw new InvalidOperationException();
            PrevScheduled.NextScheduled = NextScheduled;
            if (NextScheduled != null)
                NextScheduled.PrevScheduled = PrevScheduled;
            PrevScheduled = NextScheduled = null;
        }

        internal virtual bool IsScheduled
        {
            get { return PrevScheduled != null || NextScheduled != null; }
        }
    }
}