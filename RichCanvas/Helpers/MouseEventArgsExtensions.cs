using System.Windows.Input;

namespace RichCanvas.Helpers
{
    public static class MouseEventArgsExtensions
    {
        public static bool HasAnyButtonPressed(this MouseEventArgs e) => e.LeftButton == MouseButtonState.Pressed ||
            e.RightButton == MouseButtonState.Pressed ||
            e.MiddleButton == MouseButtonState.Pressed;

        public static bool HasAllButtonsReleased(this MouseEventArgs e) => e.RightButton == MouseButtonState.Released &&
            e.LeftButton == MouseButtonState.Released &&
            e.MiddleButton == MouseButtonState.Released;
    }
}
