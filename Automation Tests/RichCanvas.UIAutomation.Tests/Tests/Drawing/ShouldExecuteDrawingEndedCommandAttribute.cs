using System;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace RichCanvas.UIAutomation.Tests.Tests.Drawing
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class ShouldExecuteDrawingEndedCommandAttribute(bool enabled) : Attribute, ITestAction
    {
        public bool Enabled { get; } = enabled;

        public ActionTargets Targets => ActionTargets.Test;

        public void AfterTest(ITest test)
        {
            if (test.Fixture is RichCanvasTestAppTest testParent)
            {
                // always reset to default value
                testParent.RichCanvas.EnableDrawingEndedCommandExecution();
            }
        }

        public void BeforeTest(ITest test)
        {
            if (test.Fixture is RichCanvasTestAppTest testParent)
            {
                testParent.RichCanvas.DisableDrawingEndedCommandExecution();
            }
        }
    }
}
