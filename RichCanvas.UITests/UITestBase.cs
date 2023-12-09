using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using System;
using System.Diagnostics;

namespace RichCanvas.UITests
{
    public abstract class UITestBase
    {
        private AutomationBase _automation = new UIA3Automation();

        protected Application Application { get; private set; }
        protected Window Window => Application.GetMainWindow(_automation);

        public UITestBase(string applicationPath)
        {
            var app = Application.AttachOrLaunch(new ProcessStartInfo
            {
                FileName = applicationPath
            });
            app.WaitWhileMainHandleIsMissing();
            Application = app;
        }

        // Note: use TearDown and SetUp attributes for NUnit if any usage for before and after each test executes is needed
        // Run once needed now

        protected void CloseApplication()
        {
            if (Application != null)
            {
                Application.Close();
                Retry.WhileFalse(() => Application.HasExited, TimeSpan.FromSeconds(2), ignoreException: true);
                Application.Dispose();
                Application = null;
            }
        }
    }
}
