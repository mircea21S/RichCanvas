using System;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace RichCanvas.UIAutomation.Tests.Tests.Selection
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RealTimeSelectionAttribute(bool enabled) : Attribute, ITestAction
    {
        public ActionTargets Targets => ActionTargets.Test;

        public bool Enabled { get; } = enabled;

        public void AfterTest(ITest test)
        {
            if (test.Fixture is RichCanvasTestAppTest testParent)
            {
                // always reset to default value
                testParent.RichCanvas.RealTimeSelectionEnabled = false;
            }
        }

        public void BeforeTest(ITest test)
        {
            if (test.Fixture is RichCanvasTestAppTest testParent)
            {
                testParent.RichCanvas.RealTimeSelectionEnabled = Enabled;
            }
        }
    }
}
