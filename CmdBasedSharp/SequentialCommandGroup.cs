namespace CmdBased
{
    public class SequentialCommandGroup : CommandBase
    {
        private readonly IEnumerator<CommandBase> Commands;

        public SequentialCommandGroup(ICollection<CommandBase> commands)
        {
            Commands = commands.GetEnumerator();
            HashSet<SubsystemBase> reqs = new();
            foreach (var command in commands)
            {
                foreach (var req in command.Requirements) reqs.Add(req);
            }
            Requirements = reqs;
        }

        private bool IsRunning = false;

        public override void Initialise()
        {
            Commands.Reset();
            if (!Commands.MoveNext()) return; // no elements
            Commands.Current.Initialise();
            IsRunning = true;
            IsFinishedFlag = false;
        }

        public override void Execute()
        {
            CommandBase command = Commands.Current; // current command
            command.Execute();
            if (command.IsFinished)
            {
                command.End(false);
                if (Commands.MoveNext())
                {
                    // NOTE: End() will end the current command, including the
                    //       last command if we exit gracefully.
                    command = Commands.Current; // next command
                    command.Initialise();
                }
                else IsFinishedFlag = true;
            }
        }

        public override void End(bool interrupted)
        {
            if (Commands.Current != null)
                Commands.Current.End(interrupted);
            IsFinishedFlag = true; // in case we exit abruptly
        }

        private bool IsFinishedFlag;
        public override bool IsFinished
        {
            get { return IsFinishedFlag; }
        }

        public override bool IsInterruptible
        {
            get
            {
                if (!IsRunning) throw new InvalidOperationException();
                return Commands.Current.IsInterruptible;
            }
        }
    }
}