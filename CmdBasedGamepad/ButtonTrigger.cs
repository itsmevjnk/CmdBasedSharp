namespace CmdBased
{
    public class ButtonTrigger : TriggerBase
    {
        internal bool StateFlag = false; // to be set by Gamepad class
        public override bool State
        {
            get { return StateFlag; }
        }
    }
}