
namespace CmdBased
{
    public class ParallelRaceGroup : ParallelCommandGroup
    {
        public ParallelRaceGroup(ICollection<CommandBase> commands)
            : base(commands) { }

        protected List<CommandBase> FinishedCommands = [];

        public override void Initialise()
        {
            base.Initialise();
            FinishedCommands.Clear();
        }

        public override void Execute()
        {
            foreach (var command in ActiveCommands)
            {
                command.Execute();
                if (command.IsFinished) // prune finished commands
                    {
                        FinishedCommands.Add(command);
                        command.End(false);
                    }
            }
            foreach (var command in FinishedCommands)
                ActiveCommands.Remove(command);
        }

        public override void End(bool interrupted)
        {
            foreach (var command in ActiveCommands)
                command.End(true); // still running -> interrupting
            foreach (var command in FinishedCommands)
                command.End(interrupted);
            IsRunning = false;
        }
        
        public override bool IsFinished
        {
            get
            {
                if (!IsRunning) throw new InvalidOperationException();
                return FinishedCommands.Count != 0;
            }
        }
    }
}