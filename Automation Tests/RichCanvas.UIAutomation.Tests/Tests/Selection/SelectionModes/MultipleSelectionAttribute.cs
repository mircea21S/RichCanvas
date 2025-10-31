using System;

using FlaUI.Core.AutomationElements;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

using RichCanvasUIA.Client.Automation;

namespace RichCanvas.UIAutomation.Tests.Tests.Selection.SelectionModes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    internal class MultipleSelectionAttribute : Attribute, ITestAction
    {
        public ActionTargets Targets => ActionTargets.Test;

        public void AfterTest(ITest test)
        {
            if (test.Fixture is RichCanvasTestAppTest testParent)
            {
                ToggleButton realTimeDraggingToggleButton = testParent.Window
                    .FindFirstDescendant(x => x.ByAutomationId(AutomationIds.CanSelectMultipleItemsToggleButtonId))?
                    .AsToggleButton() ?? throw new ArgumentException($"Element with automation id {AutomationIds.CanSelectMultipleItemsToggleButtonId} was not found.");
                if (realTimeDraggingToggleButton.IsToggled ?? false)
                {
                    realTimeDraggingToggleButton.Toggle();
                }
            }
        }

        public void BeforeTest(ITest test)
        {
            if (test.Fixture is RichCanvasTestAppTest testParent)
            {
                ToggleButton realTimeDraggingToggleButton = testParent.Window
                    .FindFirstDescendant(x => x.ByAutomationId(AutomationIds.CanSelectMultipleItemsToggleButtonId))?
                    .AsToggleButton() ?? throw new ArgumentException($"Element with automation id {AutomationIds.CanSelectMultipleItemsToggleButtonId} was not found.");
                if (!realTimeDraggingToggleButton.IsToggled ?? true)
                {
                    realTimeDraggingToggleButton.Toggle();
                }
            }
        }
    }
}
