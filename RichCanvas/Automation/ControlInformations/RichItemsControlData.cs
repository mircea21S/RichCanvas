using RichCanvas.States;
using System.Windows.Input;

namespace RichCanvas.Automation.ControlInformations
{
    public class RichItemsControlData
    {
        public CanvasState CurrentState { get; set; }
        public double TranslateTransformX { get; set; }
        public double TranslateTransformY { get; set; }

        public InputGesture DrawingGesture { get; set; }
    }
}
