using FlaUI.Core.AutomationElements;

namespace RichCanvas.UITests.RichCanvasDemoHelpers
{
    internal static class ActionCommandsHelper
    {
        /// <summary>
        /// When RichCanvasDemo is running, finds the button that adds a new Rectangle to <see cref="RichItemsControl"/> ItemsSource and invokes it.
        /// </summary>
        /// <param name="window"></param>
        internal static void CallDrawRectButton(Window window)
        {
            var drawRectButton = window.FindFirstDescendant(d => d.ByAutomationId("DrawRectButton"));
            if (drawRectButton.Patterns.Invoke.TryGetPattern(out var invokePattern))
            {
                invokePattern.Invoke();
            }
        }
    }
}
