using System.Windows;

namespace RichCanvas.Helpers
{
    public static class RichCanvasExtensions
    {
        public static bool HasTouchedNegativeLimit(this RichCanvas canvas, Point offset) =>
            canvas.BottomElement.HasTouchedBottomLimit(offset) || canvas.RightElement.HasTouchedRightLimit(offset);

        public static bool HasTouchedExtentSizeLimit(this RichCanvas canvas, Point offset) =>
            canvas.TopElement.HasTouchedTopExtentSizeLimit(offset) || canvas.LeftElement.HasTouchedLeftExtentSizeLimit(offset) ||
            canvas.RightElement.HasTouchedRightExtentSizeLimit(offset) || canvas.BottomElement.HasTouchedBottomExtentSizeLimit(offset);
    }
}
