using System.Drawing;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;

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
        protected Size VisualViewportSize => _visualViewportSize;
        protected bool ShouldRestartApplication { get; set; }
        protected bool IgnoreItemsClearOnTearDown { get; set; }

        internal Point CurrentMousePosition => new(Mouse.Position.X, Mouse.Position.Y);

        public RichCanvasTestAppTest()
        {
            _visualViewportSize = new Size(RichCanvas.GetRichCanvasSettings().ViewportSize.Width.ToInt(), RichCanvas.GetRichCanvasSettings().ViewportSize.Height.ToInt());
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
