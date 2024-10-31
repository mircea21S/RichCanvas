using System.Windows;

namespace RichCanvas.Helpers
{
    public static class RichCanvasExtensions
    {
        public static bool HasTouchedNegativeLimit(this RichCanvas canvas, Point offset) => false;
        //(canvas.BottomElement?.HasTouchedBottomLimit(offset) ?? false) || (canvas.RightElement?.HasTouchedRightLimit(offset) ?? false);

        public static bool HasTouchedExtentSizeLimit(this RichCanvas canvas, Point offset) => false;
        //(canvas.TopElement?.HasTouchedTopExtentSizeLimit(offset) ?? false) || (canvas.LeftElement?.HasTouchedLeftExtentSizeLimit(offset) ?? false) ||
        //(canvas.RightElement?.HasTouchedRightExtentSizeLimit(offset) ?? false) || (canvas.BottomElement?.HasTouchedBottomExtentSizeLimit(offset) ?? false);
    }
}
