namespace CmdBased
{
    public class ConditionalCommand : CommandBase
    {
        private Predicate PredFunction; // true/false predicate for selection

        private CommandBase TrueCommand;
        private CommandBase FalseCommand;

        public ConditionalCommand(Predicate pred,
                                  CommandBase onTrue, CommandBase onFalse)
        {
            PredFunction = pred;
            TrueCommand = onTrue;
            FalseCommand = onFalse;

            /* aggregate requirements from TrueCommand and FalseCommand */
            Requirements = new HashSet<SubsystemBase>([
                .. TrueCommand.Requirements,
                .. FalseCommand.Requirements
            ]);
        }

        private bool PredOutput; // predicate's output for this run

        private bool IsRunning = false;

        public override void Initialise()
        {
            PredOutput = PredFunction();
            if (PredOutput) TrueCommand.Initialise();
            else FalseCommand.Initialise();
            IsRunning = true;
        }

        public override void Execute()
        {
            if (PredOutput) TrueCommand.Execute();
            else FalseCommand.Execute();
        }

        public override void End(bool interrupted)
        {
            if (PredOutput) TrueCommand.End(interrupted);
            else FalseCommand.End(interrupted);
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
                return PredOutput
                    ? TrueCommand.IsInterruptible
                    : FalseCommand.IsInterruptible;
            }
        }

        public override bool IsFinished
        {
            get
            {
                if (!IsRunning)
                    return false; // can't finish if the it's not being run
                return PredOutput
                    ? TrueCommand.IsFinished : FalseCommand.IsFinished;
            }
        }
    }
}