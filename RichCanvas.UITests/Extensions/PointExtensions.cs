using RichCanvas.UITests.Tests;
using System.Drawing;

namespace RichCanvas.UITests
{
    internal static class PointExtensions
    {
        internal static Point MoveX(this Point point, int x, HorizontalDirection direction = HorizontalDirection.LeftToRight)
        {
            if (direction == HorizontalDirection.LeftToRight)
            {
                point.X += x;
            }
            else
            {
                point.X += x;
            }
            return point;
        }

        internal static System.Windows.Point ToCanvasPoint(this Point point) => new System.Windows.Point(point.X, point.Y - RichCanvasTestAppTest.RichCanvasDemoTitleBarHeight);

        internal static Point ToCanvasDrawingPoint(this Point point) => new Point(point.X, point.Y + (int)RichCanvasTestAppTest.RichCanvasDemoTitleBarHeight);

        internal static System.Windows.Point ToCanvasPoint(this System.Windows.Point point) => new System.Windows.Point(point.X, point.Y - RichCanvasTestAppTest.RichCanvasDemoTitleBarHeight);

        /// <summary>
        /// Workaround of issue with FlaUI https://github.com/FlaUI/FlaUI/issues/612.
        /// <br/>
        /// Transforms a point to coordinates that matches the specified coordintats on instantiation.
        /// </summary>
        /// <returns></returns>
        internal static Point AsFlaUIFixedPoint(this Point point) => new Point(point.X * 2, point.Y * 2);

        internal static Point AsDrawingPoint(this System.Windows.Point windowsPoint) => new Point((int)windowsPoint.X, (int)windowsPoint.Y);
    }
}
