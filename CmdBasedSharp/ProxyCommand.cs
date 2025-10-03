namespace CmdBased
{
    public class ProxyCommand : CommandBase
    {
        private CommandScheduler Scheduler;
        private Func<CommandBase> CommandSupplier;

        public ProxyCommand(CommandScheduler scheduler,
                            Func<CommandBase> supplier)
        {
            Scheduler = scheduler;
            CommandSupplier = supplier;
        }

        public override void Initialise()
        {
            Scheduler.Schedule(CommandSupplier());
            // NOTE: This will silently fail if the scheduler fails to schedule
            //       the supplied command.
        }

        public override bool IsFinished
        {
            get { return true; }
        }
    }
}