using System.Drawing;

namespace RichCanvas.UITests
{
    internal static class PointExtensions
    {
        internal static void MoveX(this Point point, int x, HorizontalDirection direction)
        {
            if (direction == HorizontalDirection.LeftToRight)
            {
                point.X += x;
            }
            else
            {
                point.X += x;
            }
        }
    }
}
