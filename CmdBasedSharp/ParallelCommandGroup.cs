namespace CmdBased
{
    public class ParallelCommandGroup : CommandBase
    {
        protected readonly ICollection<CommandBase> Commands;

        public ParallelCommandGroup(ICollection<CommandBase> commands)
        {
            Commands = commands;
            HashSet<SubsystemBase> reqs = new();
            foreach (var command in commands)
            {
                foreach (var req in command.Requirements) reqs.Add(req);
            }
            Requirements = reqs;
        }

        protected HashSet<CommandBase> ActiveCommands;

        protected bool IsRunning = false;

        public override void Initialise()
        {
            IsRunning = true;
            ActiveCommands = [.. Commands]; // init list of active commands
            foreach (var command in Commands) command.Initialise();
        }

        public override void Execute()
        {
            List<CommandBase> commandsToRemove = [];
            foreach (var command in ActiveCommands)
            {
                command.Execute();
                if (command.IsFinished) // prune finished commands
                {
                    commandsToRemove.Add(command);
                    command.End(false);
                }
            }
            foreach (var command in commandsToRemove)
                ActiveCommands.Remove(command);
        }

        public override void End(bool interrupted)
        {
            foreach (var command in ActiveCommands)
                command.End(interrupted);
            IsRunning = false;
        }

        public override bool IsFinished
        {
            get
            {
                if (!IsRunning) throw new InvalidOperationException();
                return ActiveCommands.Count == 0; // if all commands 
            }
        }

        public override bool IsInterruptible
        {
            get
            {
                if (!IsRunning) throw new InvalidOperationException();
                if (ActiveCommands.Count == 0) return true;
                return ActiveCommands.All(command => command.IsInterruptible);
            }
        }
    }
}