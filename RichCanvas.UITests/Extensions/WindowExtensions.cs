using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using RichCanvas.Gestures;
using RichCanvasUITests.App.Automation;

namespace RichCanvas.UITests
{
    internal static class WindowExtensions
    {
        /// <summary>
        /// When RichCanvasDemo is running, finds the button with specified <paramref name="buttonAutomationId"/> and invokes it.
        /// </summary>
        /// <param name="buttonAutomationId">Button automation id.</param>
        internal static void InvokeButton(this Window window, string buttonAutomationId)
        {
            var button = window.FindFirstDescendant(d => d.ByAutomationId(buttonAutomationId));
            if (button.Patterns.Invoke.TryGetPattern(out var invokePattern))
            {
                invokePattern.Invoke();
            }
        }

        internal static void ClearAllItems(this Window window)
        {
            var button = window.FindFirstDescendant(d => d.ByAutomationId(AutomationIds.ClearItemsButtonId));
            if (button.Patterns.Invoke.TryGetPattern(out var invokePattern))
            {
                invokePattern.Invoke();
            }
        }
    }
}
