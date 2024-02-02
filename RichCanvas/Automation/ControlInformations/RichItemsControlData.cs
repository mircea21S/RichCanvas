using RichCanvas.States;
using System.Windows.Input;

namespace RichCanvas.Automation.ControlInformations
{
    public class RichItemsControlData
    {
        public CanvasState CurrentState { get; internal set; }
        public double TranslateTransformX { get; internal set; }
        public double TranslateTransformY { get; internal set; }
        public InputGesture DrawingGesture { get; internal set; }
    }
}
