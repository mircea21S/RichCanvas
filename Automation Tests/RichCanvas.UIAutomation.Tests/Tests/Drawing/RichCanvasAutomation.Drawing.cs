using System;
using System.Drawing;

using FlaUI.Core.Tools;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.Tests.Extensions;
using RichCanvas.UIAutomation.Tests.Helpers;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation
    {
        private static readonly Random _rand = new();

        public void Draw(Size containerSize, out Point containerLocation, int scaleX = 1, int scaleY = 1)
        {
            Point pointOnCanvas = GetRandomPointOnRichCanvas();
            FlaUIInputData flaUiInput = InputMapper.MapToFlaUIInput(RichCanvasGestures.Drawing);
            Point endPoint = GetEndPointByScale(pointOnCanvas, containerSize, scaleX, scaleY);
            flaUiInput.Drag(pointOnCanvas.ToCanvasDrawingPoint(), endPoint.ToCanvasDrawingPoint());
            containerLocation = pointOnCanvas;
        }

        public void DrawPositionedContainer(RichCanvasContainerAutomation container, Size containerSize, int scaleX = 1, int scaleY = 1)
        {
            Point startPoint = container.Location;
            FlaUIInputData flaUiInput = InputMapper.MapToFlaUIInput(RichCanvasGestures.Drawing);
            Point endPoint = GetEndPointByScale(startPoint, containerSize, scaleX, scaleY);
            flaUiInput.Drag(startPoint.ToCanvasDrawingPoint(), endPoint.ToCanvasDrawingPoint());
        }

        private Point GetEndPointByScale(Point pointOnCanvas, Size containerSize, int scaleX, int scaleY)
        {
            if (scaleX == 1 && scaleY == 1)
            {
                return new Point(pointOnCanvas.X + containerSize.Width, pointOnCanvas.Y + containerSize.Height);
            }
            else if (scaleX == -1 && scaleY == 1)
            {
                return new Point(pointOnCanvas.X - containerSize.Width, pointOnCanvas.Y + containerSize.Height);
            }
            else if (scaleX == 1 && scaleY == -1)
            {
                return new Point(pointOnCanvas.X + containerSize.Width, pointOnCanvas.Y - containerSize.Height);
            }
            return new Point(pointOnCanvas.X - containerSize.Width, pointOnCanvas.Y - containerSize.Height);
        }

        private Point GetRandomPointOnRichCanvas()
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
