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

        /* decorators */

        public virtual ParallelRaceGroup WithTimeout(int ticks)
        {
            return new ParallelRaceGroup([this, new TimeoutCommand(ticks)]);
        }

        public virtual ParallelRaceGroup InterruptOn(Func<bool> predicate)
        {
            return new ParallelRaceGroup
                ([this, new WaitUntilCommand(predicate)]);
        }

        public virtual SequentialCommandGroup WhenFinished(Action func)
        {
            return new SequentialCommandGroup
                ([this, new InstantCommand(func)]);
        }

        public virtual SequentialCommandGroup BeforeStarting(Action func)
        {
            return new SequentialCommandGroup
                ([new InstantCommand(func), this]);
        }

        public virtual SequentialCommandGroup AndThen(CommandBase command)
        {
            return AndThen([command]);
        }

        public virtual SequentialCommandGroup AndThen
            (ICollection<CommandBase> commands)
        {
            return new SequentialCommandGroup([this, .. commands]);
        }

        public virtual ParallelDeadlineGroup DeadlineWith(CommandBase command)
        {
            return DeadlineWith([command]);
        }

        public virtual ParallelDeadlineGroup DeadlineWith
            (ICollection<CommandBase> commands)
        {
            return new ParallelDeadlineGroup(this, commands);
        }

        public virtual ParallelCommandGroup AlongWith(CommandBase command)
        {
            return AlongWith([command]);
        }

        public virtual ParallelCommandGroup AlongWith
            (ICollection<CommandBase> commands)
        {
            return new ParallelCommandGroup([this, .. commands]);
        }

        public virtual ParallelRaceGroup RaceWith(CommandBase command)
        {
            return RaceWith([command]);
        }

        public virtual ParallelRaceGroup RaceWith
            (ICollection<CommandBase> commands)
        {
            return new ParallelRaceGroup([this, .. commands]);
        }

        public virtual PerpetualCommand Perpetually()
        {
            return new PerpetualCommand(this);
        }
    }
}