using System.Drawing;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;

using NUnit.Framework;

using RichCanvas.UIAutomation.FlaUIClient;

using RichCanvasUIA.Client.UIA_Mode;

namespace RichCanvas.UIAutomation.Tests.Tests
{
    public class RichCanvasTestAppTest : UITestBase
    {
        private readonly Size _visualViewportSize;
        private RichCanvasAutomation _richCanvas;

        public RichCanvasAutomation RichCanvas => _richCanvas ??= GetCurrentRichCanvasElement();
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
            RichCanvasUIAClientCommunicator.ClearAllItems();
        }

        protected RichCanvasAutomation GetCurrentRichCanvasElement()
            => Window.FindFirstDescendant(d => d.ByAutomationId(AutomationIds.RichCanvasControl)).AsRichCanvasAutomation(Window);

        protected Point CreatePointRelativeToRichCanvasElement(int x, int y) => new(x + RichCanvas.BoundingRectangle.Left, y + RichCanvas.BoundingRectangle.Top);
    }
}
