namespace CmdBased
{
    public class InstantCommand : CommandBase
    {
        private Action InitFunction;

        public override bool IsInterruptible
        {
            get { return true; }
        }
        // NOTE: InstantCommand does not have an execute or end phase (and only
        //       occupies the required subsystems briefly), so it is inherently
        //       interruptble.

        public InstantCommand(Action func, 
                              ICollection<SubsystemBase>? requirements = null)
        {
            InitFunction = func;
            Requirements = requirements ?? [];
        }

        public override void Initialise()
        {
            InitFunction();
        }

        public override bool IsFinished
        {
            get { return true; }
        }
    }
}