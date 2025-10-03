namespace CmdBased
{
    public class SelectCommand<T> : CommandBase
    {
        private Func<T> PredFunction; // predicate for selection

        private IDictionary<T, CommandBase> Commands;

        public SelectCommand(Func<T> pred,
                             IDictionary<T, CommandBase> commands)
        {
            PredFunction = pred;
            Commands = commands;

            /* aggregate requirements from commands */
            HashSet<SubsystemBase> reqs = [];
            foreach (var command in commands.Values)
            {
                foreach (var req in command.Requirements) reqs.Add(req);
            }
            Requirements = reqs;
        }

        private T PredOutput; // predicate's output for this run

        private bool IsRunning = false;

        public override void Initialise()
        {
            PredOutput = PredFunction();
            Commands[PredOutput].Initialise();
            IsRunning = true;
        }

        public override void Execute()
        {
            Commands[PredOutput].Execute();
        }

        public override void End(bool interrupted)
        {
            Commands[PredOutput].End(interrupted);
            IsRunning = false;
        }

        public override bool IsInterruptible
        {
            get
            {
                if (!IsRunning) throw new InvalidOperationException();
                // NOTE: We throw an exception here since the result might be
                //       stale/inconsistent. This shouldn't occur anyway, since
                //       IsInterruptible is only checked when the command is
                //       being executed.
                return Commands[PredOutput].IsInterruptible;
            }
        }

        public override bool IsFinished
        {
            get
            {
                if (!IsRunning)
                    return false; // can't finish if the it's not being run
                return Commands[PredOutput].IsFinished;
            }
        }
    }
}