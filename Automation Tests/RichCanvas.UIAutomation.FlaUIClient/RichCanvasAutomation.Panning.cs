using System.Drawing;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.FlaUIClient.Input;

namespace RichCanvas.UIAutomation.FlaUIClient
{
    public partial class RichCanvasAutomation
    {
        public void Pan(double x, double y)
            => Patterns.Transform.Pattern.Move(x, y);

        public void Pan(Point startPoint, Point endPoint)
        {
            FlaUIInputData panInputGesture = InputMapper.MapToFlaUIInput(RichCanvasGestures.Pan);
            panInputGesture.Drag(startPoint, endPoint);
        }
    }
}
