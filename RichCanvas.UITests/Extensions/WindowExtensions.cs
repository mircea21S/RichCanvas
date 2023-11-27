using FlaUI.Core.AutomationElements;

namespace RichCanvas.UITests.Extensions
{
    internal static class WindowExtensions
    {
        /// <summary>
        /// When RichCanvasDemo is running, finds the button that adds a new Rectangle to <see cref="RichItemsControl"/> ItemsSource and invokes it.
        /// </summary>
        /// <param name="window"></param>
        internal static void AddEmptyRectangle(this Window window)
        {
            var drawRectButton = window.FindFirstDescendant(d => d.ByAutomationId("DrawRectButton"));
            if (drawRectButton.Patterns.Invoke.TryGetPattern(out var invokePattern))
            {
                invokePattern.Invoke();
            }
        }

        /// <summary>
        /// When RichCanvasDemo is running, finds the button that clears all items from <see cref="RichItemsControl"/> ItemsSource and invokes it.
        /// </summary>
        /// <param name="window"></param>
        internal static void ClearAllItems(this Window window)
        {
            var drawRectButton = window.FindFirstDescendant(d => d.ByAutomationId("ClearItems"));
            if (drawRectButton.Patterns.Invoke.TryGetPattern(out var invokePattern))
            {
                invokePattern.Invoke();
            }
        }

        /// <summary>
        /// When RichCanvasDemo is running, finds the button that adds a defined rectangle to <see cref="RichItemsControl"/> ItemsSource and invokes it.
        /// </summary>
        /// <param name="window"></param>
        internal static void AddDrawnRectangle(this Window window)
        {
            var drawRectButton = window.FindFirstDescendant(d => d.ByAutomationId("AddDrawnRectangle"));
            if (drawRectButton.Patterns.Invoke.TryGetPattern(out var invokePattern))
            {
                invokePattern.Invoke();
            }
        }
    }
}
