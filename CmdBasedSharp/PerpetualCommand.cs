namespace CmdBased
{
    public class PerpetualCommand : CommandBase
    {
        private CommandBase Command;

        public PerpetualCommand(CommandBase command)
        {
            Command = command;
            Requirements = command.Requirements;
        }

        public override void Initialise()
        {
            Command.Initialise();
        }

        public override void Execute()
        {
            Command.Execute();
        }

        public override void End(bool interrupted)
        {
            Command.End(interrupted);
        }

        public override bool IsFinished
        {
            get { return false; }
        }

        public override bool IsInterruptible
        {
            get { return Command.IsInterruptible; }
        }
    }
}