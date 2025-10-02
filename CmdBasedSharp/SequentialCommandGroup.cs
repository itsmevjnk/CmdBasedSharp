namespace CmdBased
{
    public class SequentialCommandGroup : CommandBase
    {
        private readonly IEnumerator<CommandBase> Commands;

        public SequentialCommandGroup(ICollection<CommandBase> commands)
        {
            Commands = commands.GetEnumerator();
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
                if (Commands.MoveNext())
                {
                    command.End(false);
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