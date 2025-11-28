using System.Drawing;

using RichCanvas.UIAutomation.Tests.Tests;

namespace RichCanvas.UIAutomation.Tests.Utilities
{
    internal readonly struct UIAClientAppPoint
    {
        internal int X { get; }
        internal int Y { get; }
        internal double Xd { get; }
        internal double Yd { get; }

        internal UIAClientAppPoint(int x, int y)
        {
            X = x;
            Y = y + RichCanvasTestAppTest.RichCanvasDemoTitleBarHeight;
        }

        internal UIAClientAppPoint(double x, double y)
        {
            Xd = x;
            Yd = y + RichCanvasTestAppTest.RichCanvasDemoTitleBarHeight;
        }

        internal UIAClientAppPoint(Point point)
        {
            X = point.X;
            Y = point.Y + RichCanvasTestAppTest.RichCanvasDemoTitleBarHeight;
        }

        public static implicit operator Point(UIAClientAppPoint clientAppPoint)
            => new(clientAppPoint.X, clientAppPoint.Y);

        public static implicit operator UIAClientAppPoint(Point point)
          => new(point.X, point.Y);
    }
}
