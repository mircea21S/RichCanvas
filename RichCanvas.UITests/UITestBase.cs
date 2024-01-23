using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using System;
using System.Diagnostics;
using System.Threading;

namespace RichCanvas.UITests
{
    public abstract class UITestBase
    {
        private AutomationBase _automation = new UIA3Automation();

        protected Application Application { get; private set; }
        protected Window Window => Application.GetMainWindow(_automation);

        public UITestBase(string applicationPath)
        {
            StartApplication(applicationPath);
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

        protected void StartApplication(string applicationPath)
        {
            var app = Application.AttachOrLaunch(new ProcessStartInfo
            {
                FileName = applicationPath
            });
            app.WaitWhileMainHandleIsMissing();
            // hack to wait for all the initializations (some NullRefException being thrown if not)
            Thread.Sleep(1000);
            Application = app;
        }
    }
}
