using System;
using System.Drawing;

using FlaUI.Core.Tools;

using RichCanvas.UIAutomation.Tests.Utilities;

namespace RichCanvas.UIAutomation.Tests.Extensions
{
    internal static class PointExtensions
    {
        /// <summary>
        /// Workaround of issue with FlaUI https://github.com/FlaUI/FlaUI/issues/612.
        /// <br/>
        /// Transforms a point to coordinates that matches the specified coordintats on instantiation.
        /// </summary>
        /// <returns></returns>
        internal static Point AsFlaUIFixedPoint(this Point point) => new Point(point.X * 2, point.Y * 2);

        internal static Point AsDrawingPoint(this System.Windows.Point windowsPoint) => new Point(windowsPoint.X.ToInt(), windowsPoint.Y.ToInt());

        internal static System.Windows.Point AsWindowsPoint(this Point drawingPoint) => new System.Windows.Point(drawingPoint.X, drawingPoint.Y);

        internal static Point OffsetNew(this Point drawingPoint, Point offset) => new Point(drawingPoint.X + offset.X, drawingPoint.Y + offset.Y);

        internal static Point AddOffset(this Point point, int offset, DragDirection dragDirection = DragDirection.Both) => dragDirection switch
        {
            DragDirection.Both => new Point(point.X + offset, point.Y + offset),
            DragDirection.OnlyX => new Point(point.X + offset, point.Y),
            DragDirection.OnlyY => new Point(point.X, point.Y + offset),
            _ => throw new NotImplementedException()
        };

        internal static Point AddOffset(this UIAClientAppPoint point, int offset, DragDirection dragDirection = DragDirection.Both) => dragDirection switch
        {
            DragDirection.Both => new Point(point.X + offset, point.Y + offset),
            DragDirection.OnlyX => new Point(point.X + offset, point.Y),
            DragDirection.OnlyY => new Point(point.X, point.Y + offset),
            _ => throw new NotImplementedException()
        };

        internal static Point GetEndPointByScale(this UIAClientAppPoint fromPoint, Size containerSize, int scaleX = 1, int scaleY = 1)
        {
            if (scaleX == 1 && scaleY == 1)
            {
                return new Point(fromPoint.X + containerSize.Width, fromPoint.Y + containerSize.Height);
            }
            else if (scaleX == -1 && scaleY == 1)
            {
                return new Point(fromPoint.X - containerSize.Width, fromPoint.Y + containerSize.Height);
            }
            else if (scaleX == 1 && scaleY == -1)
            {
                return new Point(fromPoint.X + containerSize.Width, fromPoint.Y - containerSize.Height);
            }
            return new Point(fromPoint.X - containerSize.Width, fromPoint.Y - containerSize.Height);
        }
    }
}
