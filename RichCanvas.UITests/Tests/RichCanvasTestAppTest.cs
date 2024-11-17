using FlaUI.Core.AutomationElements;
using NUnit.Framework;
using System.Windows;

namespace RichCanvas.UITests.Tests
{
    public class RichCanvasTestAppTest : UITestBase
    {
        /// <summary>
        /// Size of Title bar (SystemParamters.WindowCaptionHeight) = 22.5
        /// </summary>
        public const double RichCanvasDemoTitleBarHeight = 23;

        protected RichItemsControlAutomation RichItemsControl => Window.FindFirstDescendant(d => d.ByAutomationId("source")).AsRichItemsControlAutomation();
        protected Size ViewportSize => RichItemsControl?.RichItemsControlData?.ViewportSize ?? new Size(1187, 800);
        protected System.Drawing.Point ViewportCenter => new System.Drawing.Point((int)ViewportSize.Width / 2, (int)ViewportSize.Height / 2);
        protected Point ViewportLocation => RichItemsControl?.RichItemsControlData?.ViewportLocation ?? new Point(0, 0);
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
