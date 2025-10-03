namespace CmdBased
{
    public class CommandScheduler
    {
        private List<SubsystemBase> Subsystems = [];

        public void RegisterSubsystem(SubsystemBase subsystem)
        {
            Subsystems.Add(subsystem);
        }

        private List<TriggerBase> Triggers = [];

        public void RegisterTrigger(TriggerBase trigger)
        {
            Triggers.Add(trigger);
        }

        /* null command class - to be used as sentinel/head of command LL */
        private class NullCommand : CommandBase
        {
            public override void Initialise()
            {
                throw new InvalidOperationException();
            }

            public override void Execute()
            {
                throw new InvalidOperationException();
            }
            public override void End(bool interrupted)

            {
                throw new InvalidOperationException();
            }

            public override bool IsInterruptible
                => throw new InvalidOperationException();

            public override bool IsFinished
                => throw new InvalidOperationException();

            internal override bool IsScheduled
                => throw new InvalidOperationException();
        }
        NullCommand ScheduledCommandHead = new();

        public bool Schedule(CommandBase command)
        {
            if (command.IsScheduled) return false; // already scheduled

            /* check requirements */
            foreach (var requirement in command.Requirements)
            {
                if (!requirement.IsAvailable)
                { // NOTE: this also checks if OccupyingCommand is not null
                    if (!requirement.OccupyingCommand.IsInterruptible)
                        return false; // conflict with uninterruptible command
                }
            }

            /* claim requirement occupation */
            foreach (var requirement in command.Requirements)
            {
                if (!requirement.IsAvailable)
                    Cancel(requirement.OccupyingCommand, true);
                requirement.OccupyingCommand = command;
            }

            ScheduledCommandHead.AddScheduled(command); // add to queue
            command.Initialise();
            return true;
        }

        private bool Cancel(CommandBase command, bool interrupt)
        {
            if (!command.IsScheduled) throw new InvalidOperationException();
            if (interrupt && !command.IsInterruptible)
                return false; // attempting to interrupt uninterruptible cmd

            command.End(interrupt);

            foreach (var requirement in command.Requirements)
                requirement.OccupyingCommand = null;
            command.RemoveScheduled();

            return true;
        }

        public void Periodic()
        {
            /* run subsystems' periodic procedures */
            foreach (var subsystem in Subsystems) subsystem.Periodic();

            /* iterate through triggers */
            foreach (var trigger in Triggers)
            {
                trigger.Periodic();
                bool state = trigger.State;

                if (!trigger.LastState && state) // rising edge
                {
                    if (trigger.OnChangeCommand != null)
                        Schedule(trigger.OnChangeCommand);
                    if (trigger.OnTrueCommand != null)
                        Schedule(trigger.OnTrueCommand);
                    if (trigger.WhileTrueCommand != null)
                        Schedule(trigger.WhileTrueCommand);
                    if (trigger.WhileFalseCommand != null
                        && trigger.WhileFalseCommand.IsScheduled)
                        Cancel(trigger.WhileFalseCommand, true);
                    if (trigger.ToggleOnTrueCommand != null)
                    {
                        if (trigger.ToggleOnTrueCommand.IsScheduled)
                            Cancel(trigger.ToggleOnTrueCommand, true);
                        else Schedule(trigger.ToggleOnTrueCommand);
                    }
                }
                else if (trigger.LastState && !state) // falling edge
                {
                    if (trigger.OnChangeCommand != null)
                        Schedule(trigger.OnChangeCommand);
                    if (trigger.OnFalseCommand != null)
                        Schedule(trigger.OnFalseCommand);
                    if (trigger.WhileTrueCommand != null
                        && trigger.WhileTrueCommand.IsScheduled)
                        Cancel(trigger.WhileTrueCommand, true);
                    if (trigger.WhileFalseCommand != null)
                        Schedule(trigger.WhileFalseCommand);
                    if (trigger.ToggleOnFalseCommand != null)
                    {
                        if (trigger.ToggleOnFalseCommand.IsScheduled)
                            Cancel(trigger.ToggleOnFalseCommand, true);
                        else Schedule(trigger.ToggleOnFalseCommand);
                    }

                }

                trigger.LastState = state;
            }

            /* iterate through scheduled commands */
            CommandBase? command = ScheduledCommandHead.NextScheduled;
            while (command != null)
            {
                command.Execute();
                CommandBase? nextCommand = command.NextScheduled;
                if (command.IsFinished) Cancel(command, false);
                command = nextCommand;
            }

            /* schedule default commands of unoccupied subsystems */
            foreach (var subsystem in Subsystems)
            {
                if (subsystem.IsAvailable && subsystem.HasDefaultCommand)
                    Schedule(subsystem.DefaultCommand);
                // NOTE: This will silently fail for default commands that
                //       require another subsystem that is being occupied by an
                //       uninterruptible command.
            }
        }
    }
}