using RichCanvas.UITests.Tests;
using System.Drawing;

namespace RichCanvas.UITests
{
    internal static class PointExtensions
    {
        internal static Point MoveX(this Point point, int x, HorizontalDirection direction)
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
        internal static System.Windows.Point ToCanvasPoint(this System.Windows.Point point) => new System.Windows.Point(point.X, point.Y - RichCanvasTestAppTest.RichCanvasDemoTitleBarHeight);
    }
}
