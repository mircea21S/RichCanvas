using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
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
            Wait.UntilInputIsProcessed();
        }

        internal static void ToggleButton(this Window window, string buttonAutomationId)
        {
            var toggleButton = window.FindFirstDescendant(d => d.ByAutomationId(buttonAutomationId));
            toggleButton.AsToggleButton()?.Toggle();
            Wait.UntilInputIsProcessed();
        }

        internal static void ToggleCheckbox(this Window window, string buttonAutomationId)
        {
            var toggleButton = window.FindFirstDescendant(d => d.ByAutomationId(buttonAutomationId));
            toggleButton.AsCheckBox()?.Toggle();
            Wait.UntilInputIsProcessed();
        }

        internal static void ClearAllItems(this Window window)
        {
            var button = window.FindFirstDescendant(d => d.ByAutomationId(AutomationIds.ClearItemsButtonId));
            if (button.Patterns.Invoke.TryGetPattern(out var invokePattern))
            {
                invokePattern.Invoke();
            }
            Wait.UntilInputIsProcessed();
        }
    }
}
