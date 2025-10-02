namespace CmdBased
{
    public abstract class TriggerBase
    {
        public virtual void Periodic() { }

        public virtual bool State { get; }

        internal bool LastState = false; // assumed initial state
        // NOTE: This is only used for CommandScheduler

        /* commands */
        public CommandBase? OnChangeCommand = null;

        public TriggerBase OnChange(CommandBase command)
        {
            OnChangeCommand = command;
            return this;
        }

        public CommandBase? OnTrueCommand = null;

        public TriggerBase OnTrue(CommandBase command)
        {
            OnTrueCommand = command;
            return this;
        }

        public CommandBase? OnFalseCommand = null;

        public TriggerBase OnFalse(CommandBase command)
        {
            OnFalseCommand = command;
            return this;
        }

        public CommandBase? WhileTrueCommand = null;

        public TriggerBase WhileTrue(CommandBase command)
        {
            WhileTrueCommand = command;
            return this;
        }

        public CommandBase? WhileFalseCommand = null;

        public TriggerBase WhileFalse(CommandBase command)
        {
            WhileFalseCommand = command;
            return this;
        }

        public CommandBase? ToggleOnTrueCommand = null;

        public TriggerBase ToggleOnTrue(CommandBase command)
        {
            ToggleOnTrueCommand = command;
            return this;
        }

        public CommandBase? ToggleOnFalseCommand = null;

        public TriggerBase ToggleOnFalse(CommandBase command)
        {
            ToggleOnFalseCommand = command;
            return this;
        }
    }
}