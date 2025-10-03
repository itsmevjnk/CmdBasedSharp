namespace CmdBased
{
    public class ScheduleCommand : CommandBase
    {
        private CommandScheduler Scheduler;

        private ICollection<CommandBase> Commands; // commands to be scheduled

        public ScheduleCommand(CommandScheduler scheduler,
                               ICollection<CommandBase> commands)
        {
            Scheduler = scheduler;
            Commands = commands;
        }

        public ScheduleCommand(CommandScheduler scheduler, CommandBase command)
        {
            Scheduler = scheduler;
            Commands = [command]; // single command
        }

        public override void Initialise()
        {
            foreach (var command in Commands) Scheduler.Schedule(command);
            // NOTE: This will silently fail if the scheduler no-ops or fails
            //       to schedule a command in.
        }

        public override bool IsFinished
        {
            get { return true; }
        }

        public override bool IsInterruptible
        {
            get { return true; } // NOTE: this is not really relevant anyway
        }
    }
}