using System.Windows.Input;

namespace RichCanvas.Helpers
{
    internal static class MouseEventArgsExtensions
    {
        internal static bool HasAnyButtonPressed(this MouseEventArgs e) => e.LeftButton == MouseButtonState.Pressed ||
            e.RightButton == MouseButtonState.Pressed ||
            e.MiddleButton == MouseButtonState.Pressed;

        internal static bool HasAllButtonsReleased(this MouseEventArgs e) => e.RightButton == MouseButtonState.Released &&
            e.LeftButton == MouseButtonState.Released &&
            e.MiddleButton == MouseButtonState.Released;
    }
}
