namespace CmdBased
{
    public abstract class SubsystemBase
    {
        public virtual void Periodic() { }

        public CommandBase? DefaultCommand = null;
        internal CommandBase? OccupyingCommand = null;

        internal bool IsAvailable
        {
            get { return OccupyingCommand == null; }
        }

        internal bool HasDefaultCommand
        {
            get { return DefaultCommand != null; }
        }
    }
}