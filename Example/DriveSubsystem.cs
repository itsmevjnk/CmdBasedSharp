namespace CmdBased
{
    class DriveSubsystem : SubsystemBase
    {
        private double XVelocity = 0, YVelocity = 0;

        public double XPosition { get; private set; } = 0;
        public double YPosition { get; private set; } = 0;

        public DriveSubsystem()
        {
            XPosition = YPosition = 0;
        }

        public DriveSubsystem(double xPos, double yPos)
        {
            XPosition = xPos;
            YPosition = yPos;
        }

        public override void Periodic()
        {
            /* update X/Y position */
            XPosition += XVelocity;
            YPosition += YVelocity;
        }

        public void Drive(double xVel, double yVel)
        {
            XVelocity = xVel;
            YVelocity = yVel;
        }

        public void Stop()
        {
            XVelocity = YVelocity = 0;
        }

        public override string ToString()
        {
            return "Drivetrain("
                + $"Pos=({XPosition:F4},{YPosition:F4}),"
                + $"Vel=({XVelocity:F4},{YVelocity:F4}))";
        }
    }
}