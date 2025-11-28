using System.Drawing;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.Tests.Helpers;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation
    {
        public void Pan(out System.Windows.Vector distanceVector)
        {
            FlaUIInputData panInputGesture = InputMapper.MapToFlaUIInput(RichCanvasGestures.Pan);
            Point startPoint = GetRandomPointOnRichCanvas();
            Point endPoint = GetRandomPointOnRichCanvas();
            distanceVector = endPoint.AsWindowsPoint() - startPoint.AsWindowsPoint();

            panInputGesture.Drag(startPoint.ToCanvasDrawingPoint(), endPoint.ToCanvasDrawingPoint());
        }

        public void Pan(double x, double y)
            => Patterns.Transform.Pattern.Move(x, y);

        public void Pan(Point startPoint, Point endPoint)
        {
            FlaUIInputData panInputGesture = InputMapper.MapToFlaUIInput(RichCanvasGestures.Pan);
            panInputGesture.Drag(startPoint.ToCanvasDrawingPoint(), endPoint.ToCanvasDrawingPoint());
        }
    }
}
