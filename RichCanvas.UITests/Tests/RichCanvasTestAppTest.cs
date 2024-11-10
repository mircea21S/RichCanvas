using FlaUI.Core.AutomationElements;
using NUnit.Framework;
using System.Drawing;

namespace RichCanvas.UITests.Tests
{
    public class RichCanvasTestAppTest : UITestBase
    {
        /// <summary>
        /// Size of Title bar (SystemParamters.WindowCaptionHeight) = 22.5
        /// </summary>
        public const double RichCanvasDemoTitleBarHeight = 23;

        protected RichItemsControlAutomation RichItemsControl => Window.FindFirstDescendant(d => d.ByAutomationId("source")).AsRichItemsControlAutomation();
        protected Size DemoControlSize => new Size(1187, 800);
        protected Point ViewportCenter => new Point(DemoControlSize.Width / 2, DemoControlSize.Height / 2);
        protected bool ShouldRestartApplication { get; set; }
        protected bool IgnoreItemsClearOnTearDown { get; set; }

        public RichCanvasTestAppTest()
        {
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
    }
}
