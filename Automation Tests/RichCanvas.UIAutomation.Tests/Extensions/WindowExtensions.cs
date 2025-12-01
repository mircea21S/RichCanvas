using System.Drawing;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;

namespace RichCanvas.UIAutomation.Tests.Extensions
{
    internal static class WindowExtensions
    {
        internal static void ResizeByTitleBarDragging(this Window window)
        {
            Mouse.Click(new Point(window.ActualWidth.ToInt() / 2, -5));
            Mouse.Drag(new Point(window.ActualWidth.ToInt() / 2, -5), new Point(window.ActualWidth.ToInt() / 2, 0));
            Wait.UntilInputIsProcessed();
        }

        internal static void TryResize(this Window window, double width, double height)
        {
            if (window.Patterns.Transform.TryGetPattern(out FlaUI.Core.Patterns.ITransformPattern transformPattern))
            {
                transformPattern.Resize(width, height);
            }
        }
    }
}
