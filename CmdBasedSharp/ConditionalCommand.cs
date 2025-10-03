namespace CmdBased
{
    public class ConditionalCommand : SelectCommand<bool>
    {

        public ConditionalCommand(Func<bool> pred,
                                  CommandBase onTrue, CommandBase onFalse)
            : base(pred, new Dictionary<bool, CommandBase>
            {
                { true, onTrue }, { false, onFalse }
            }) {}
    }
}