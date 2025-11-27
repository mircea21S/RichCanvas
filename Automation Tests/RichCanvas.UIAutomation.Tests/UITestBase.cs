using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;

using RichCanvas.UIAutomation.Tests.IPC;

using RichCanvasUIA.Client;

namespace RichCanvas.UIAutomation.Tests
{
    public abstract class UITestBase
    {
        private readonly AutomationBase _automation = new UIA3Automation();

        private string AppPath { get; }
        protected Application Application { get; private set; }
        public Window Window { get; private set; }
        public RichCanvasUIAClientChannel RichCanvasUIAClientCommunicator { get; private set; }

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
                RichCanvasUIAClientCommunicator.Dispose();
                Application.Dispose();
                Application = null;
            }
        }

        protected void StartApplication()
        {
            RichCanvasUIAClientCommunicator = new RichCanvasUIAClientChannel();

            var app = Application.AttachOrLaunch(new ProcessStartInfo
            {
                FileName = AppPath,
                Arguments = RichCanvasUIAClientCommunicator.GetClientHandleAsString(),
                UseShellExecute = false
            });
            app.WaitWhileMainHandleIsMissing();
            // hack to wait for all the initializations (some NullRefException being thrown if not)
            Thread.Sleep(1000);

            RichCanvasUIAClientCommunicator.DisposeLocalCopyOfClientHandle();

            Application = app;
            Window = app.GetMainWindow(_automation);
        }
    }
}
