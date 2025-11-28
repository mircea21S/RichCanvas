using System.Windows;

using FlaUI.Core.AutomationElements;

using NUnit.Framework;

namespace RichCanvas.UIAutomation.Tests.Tests
{
    public class RichCanvasTestAppTest : UITestBase
    {
        /// <summary>
        /// Size of Title bar (SystemParamters.WindowCaptionHeight) = 22.5
        /// </summary>
        public const int RichCanvasDemoTitleBarHeight = 23;

        private readonly Size _visualViewportSize;
        private RichCanvasAutomation _richCanvas;

        public RichCanvasAutomation RichCanvas => _richCanvas ??= Window.FindFirstDescendant(d => d.ByAutomationId("source")).AsRichCanvasAutomation(Window);
        protected Size ViewportSize => RichCanvas?.RichCanvasSettings?.ViewportSize ?? new Size(1187, 800);
        protected Size VisualViewportSize => _visualViewportSize;
        protected Point ViewportLocation => RichCanvas?.RichCanvasSettings?.ViewportLocation ?? new Point(0, 0);
        protected bool ShouldRestartApplication { get; set; }
        protected bool IgnoreItemsClearOnTearDown { get; set; }

        public RichCanvasTestAppTest()
        {
            _visualViewportSize = new Size(RichCanvas.RichCanvasSettings.ViewportSize.Width, RichCanvas.RichCanvasSettings.ViewportSize.Height);
        }

        [SetUp]
        public virtual void SetUp()
        {
            if (ShouldRestartApplication)
            {
                ShouldRestartApplication = false;
                StartApplication();
            }
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (ShouldRestartApplication)
            {
                ShouldRestartApplication = false;
                CloseApplication();
            }
            if (IgnoreItemsClearOnTearDown)
            {
                IgnoreItemsClearOnTearDown = false;
                return;
            }
            Window.ClearAllItems();
        }

        protected RichCanvasAutomation GetCurrentRichCanvasElement()
            => Window.FindFirstDescendant(d => d.ByAutomationId("source")).AsRichCanvasAutomation(Window);
    }
}
