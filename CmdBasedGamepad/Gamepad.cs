using System.Collections.ObjectModel;
using Gamepad;

namespace CmdBased
{
    public class Gamepad : IDisposable
    {
        private GamepadController Controller; // controller obj being wrapped

        /* button and axis IDs - for PS3 controller */
        public enum ButtonIDs
        {
            CROSS,
            CIRCLE,
            TRIANGLE,
            SQUARE,
            L1,
            R1,
            L2,
            R2,
            SELECT,
            START,
            PS,
            L3,
            R3,
            DPAD_UP,
            DPAD_DOWN,
            DPAD_LEFT,
            DPAD_RIGHT,
            NUM_BUTTONS // total number of buttons
        }

        public enum AxisIDs
        {
            LEFT_X,
            LEFT_Y,
            L2,
            RIGHT_X,
            RIGHT_Y,
            R2,
            NUM_AXES // total number of axes
        }

        private ButtonTrigger[] TriggersArray =
            new ButtonTrigger[(int)ButtonIDs.NUM_BUTTONS];
        public readonly ReadOnlyCollection<ButtonTrigger> Triggers;

        private double[] AxesArray = new double[(int)AxisIDs.NUM_AXES];
        public readonly ReadOnlyCollection<double> Axes;

        public Gamepad(string device = "/dev/input/js0")
        {
            Controller = new(device);

            for (int i = 0; i < TriggersArray.Length; i++)
                TriggersArray[i] = new ButtonTrigger();

            Triggers = new(TriggersArray);
            Axes = new(AxesArray);

            Controller.ButtonChanged += (sender, e) =>
            {
                TriggersArray[e.Button].StateFlag = e.Pressed;
            };

            Controller.AxisChanged += (sender, e) =>
            {
                AxesArray[e.Axis] =
                    Math.Clamp((double)e.Value / 32767, -1.0, 1.0);
            };
        }

        public void RegisterTriggers(CommandScheduler scheduler)
        {
            foreach (var trig in Triggers) scheduler.RegisterTriggers(trig);
        }

        public void Dispose()
        {
            Controller?.Dispose();
        }
    }
}