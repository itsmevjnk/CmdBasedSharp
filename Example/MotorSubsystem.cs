namespace CmdBased
{
    class MotorSubsystem : SubsystemBase
    {
        private double VelocityValue = 0.0;
        public double Velocity
        {
            get { return VelocityValue; }
            set { VelocityValue = Math.Clamp(value, -1.0, 1.0); }
        }

        public double Position { get; private set; } = 0;

        public readonly string Name;

        public MotorSubsystem(string name = "Motor")
        {
            Name = name;
        }

        public override void Periodic()
        {
            Position += Velocity;
        }

        public override string ToString()
        {
            return $"{Name}(Pos={Position:F4},Vel={Velocity:F4})";
        }

        public CommandBase RunCommand(double vel)
        {
            return new InstantCommand(() => { Velocity = vel; }, [this]);
            // NOTE: this command will NOT stop the motor at the end!
        }

        public CommandBase RunStopCommand(double vel)
        {
            return new FunctionalCommand(
                () => { Velocity = vel; }, // start motor
                () => { }, // do nothing in between
                (interrupted) => { Velocity = 0; }, // stop motor at the end
                () => { return false; }, // never finishes
                [this] // requiring this subsystem
            );
        }

        public CommandBase StopCommand()
        {
            return new InstantCommand(() => { Velocity = 0; }, [this]);
        }

        public CommandBase RampCommand(double targetVel, double rate,
                                       double tolerance = 0.01)
        {
            return new WaitUntilCommand(() =>
            {
                return Math.Abs(targetVel - Velocity) <= tolerance;
            }).DeadlineWith(new RunCommand(
                () =>
                {
                    Velocity += rate * ((Velocity < targetVel) ? 1 : -1);
                },
                [this]
            ));
        }
    }
}