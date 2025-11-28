using System;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace RichCanvas.UIAutomation.Tests.Tests.Selection.SelectionModes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class SingleSelectionAttribute : Attribute, ITestAction
    {
        public ActionTargets Targets => ActionTargets.Test;

        public void AfterTest(ITest test)
        {
            if (test.Fixture is RichCanvasTestAppTest testParent)
            {
                testParent.RichCanvas.CanSelectMultipleItems = true;
            }
        }

        public void BeforeTest(ITest test)
        {
            if (test.Fixture is RichCanvasTestAppTest testParent)
            {
                testParent.RichCanvas.CanSelectMultipleItems = false;
            }
        }
    }
}
