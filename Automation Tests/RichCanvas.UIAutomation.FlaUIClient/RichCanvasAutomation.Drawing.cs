using System.Drawing;

using FlaUI.Core.Tools;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.FlaUIClient.Input;

namespace RichCanvas.UIAutomation.FlaUIClient
{
    public partial class RichCanvasAutomation
    {
        private static readonly Random _rand = new();

        public static void Draw(Point startPoint, Point endPoint)
        {
            FlaUIInputData flaUiInput = InputMapper.MapToFlaUIInput(RichCanvasGestures.Drawing);
            flaUiInput.Drag(startPoint, endPoint);
        }

        public virtual Point GetRandomPointOnRichCanvas()
        {
            // add a tolerance to not get a point on the edge and not being able to draw due to not having enough space
            int viewportSizeTolerance = 50;
            double width = ActualWidth - viewportSizeTolerance;
            double height = ActualHeight - viewportSizeTolerance;

            if (width <= 0 || height <= 0)
            {
                throw new InvalidOperationException("Canvas has no rendered size. Ensure it's loaded and visible.");
            }

            double x = _rand.NextDouble() * width;
            double y = _rand.NextDouble() * height;
            return new Point(x.ToInt(), y.ToInt());
        }
    }
}
