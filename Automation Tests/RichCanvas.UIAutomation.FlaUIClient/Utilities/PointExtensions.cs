using System.Drawing;

using FlaUI.Core.Tools;

namespace RichCanvas.UIAutomation.FlaUIClient.Utilities
{
    public static class PointExtensions
    {
        public static Point AsDrawingPoint(this System.Windows.Point windowsPoint) => new(windowsPoint.X.ToInt(), windowsPoint.Y.ToInt());

        public static System.Windows.Point AsWindowsPoint(this Point drawingPoint) => new(drawingPoint.X, drawingPoint.Y);
    }
}
