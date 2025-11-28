using RichCanvas.UIAutomation.Tests.Tests;

namespace RichCanvas.UIAutomation.Tests.Utilities
{
    internal struct UIAClientAppPoint
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

        public static implicit operator System.Drawing.Point(UIAClientAppPoint clientAppPoint)
            => new(clientAppPoint.X, clientAppPoint.Y);
    }
}
