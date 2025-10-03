namespace CmdBased
{
    public class WaitUntilCommand : CommandBase
    {
        private Func<bool> Predicate;

        public WaitUntilCommand(Func<bool> predicate)
        {
            Predicate = predicate;
        }

        public override bool IsFinished
        {
            get { return Predicate(); }
        }
    }
}