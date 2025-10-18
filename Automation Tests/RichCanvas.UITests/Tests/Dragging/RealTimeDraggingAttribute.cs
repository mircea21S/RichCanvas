using System;

using FlaUI.Core.AutomationElements;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

using RichCanvasUITests.App.Automation;

namespace RichCanvas.UITests.Tests.Dragging
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    internal class RealTimeDraggingAttribute(bool enabled) : Attribute, ITestAction
    {
        public ActionTargets Targets => ActionTargets.Test;

        public bool Enabled { get; } = enabled;

        public void AfterTest(ITest test)
        {
            if (test.Fixture is RichCanvasTestAppTest testParent)
            {
                ToggleButton realTimeDraggingToggleButton = testParent.Window
                    .FindFirstDescendant(x => x.ByAutomationId(AutomationIds.RealTimeDraggingToggleButtonId))?
                    .AsToggleButton() ?? throw new ArgumentException($"Element with automation id {AutomationIds.RealTimeDraggingToggleButtonId} was not found.");
                // just reset to default state which is not enabled
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
                    .FindFirstDescendant(x => x.ByAutomationId(AutomationIds.RealTimeDraggingToggleButtonId))?
                    .AsToggleButton() ?? throw new ArgumentException($"Element with automation id {AutomationIds.RealTimeDraggingToggleButtonId} was not found.");
                if (Enabled && (!realTimeDraggingToggleButton.IsToggled ?? false))
                {
                    realTimeDraggingToggleButton.Toggle();
                }
                else if (!Enabled && (realTimeDraggingToggleButton.IsToggled ?? true))
                {
                    realTimeDraggingToggleButton.Toggle();
                }
            }
        }
    }
}
