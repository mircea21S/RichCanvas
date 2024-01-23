using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using RichCanvasUITests.App;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace RichCanvas.UITests
{
    public abstract class UITestBase
    {
        private AutomationBase _automation = new UIA3Automation();

        private string AppPath { get; }
        protected Application Application { get; private set; }
        protected Window Window => Application.GetMainWindow(_automation);
        protected IEventLibrary EventLibrary => _automation.EventLibrary;


        public UITestBase()
        {
            AppPath = Assembly.GetAssembly(typeof(MainWindow)).Location;
            StartApplication();
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

        protected void StartApplication()
        {
            var app = Application.AttachOrLaunch(new ProcessStartInfo
            {
                FileName = AppPath
            });
            app.WaitWhileMainHandleIsMissing();
            // hack to wait for all the initializations (some NullRefException being thrown if not)
            Thread.Sleep(1000);
            Application = app;
        }
    }
}
