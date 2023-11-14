using Newtonsoft.Json;
using RichCanvas.States;

namespace RichCanvas.Automation.ControlInformations
{
    public class RichItemsControlData
    {
        public CanvasState CurrentState { get; set; }
        public double TranslateTransformX { get; set; }
        public double TranslateTransformY { get; set; }
    }
}
