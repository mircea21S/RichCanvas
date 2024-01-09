using NUnit.Framework;
using System.Drawing;

namespace RichCanvas.UITests.Tests
{
    public class RichCanvasTestAppTest : UITestBase
    {
        private const string RichCanvasDemoBinPath = @"C:\Programming Projects\RichCanvas\RichCanvasUITests.App\bin\Debug\RichCanvasUITests.App.exe";
        /// <summary>
        /// Size of Title bar (SystemParamters.WindowCaptionHeight) = 22.5
        /// </summary>
        public const double RichCanvasDemoTitleBarHeight = 22.5;

        protected RichItemsControlAutomation RichItemsControl => Window.FindFirstDescendant(d => d.ByAutomationId("source")).AsRichItemsControlAutomation();
        protected Size DemoControlSize => new Size(1187, 800);
        protected Point ViewportCenter => new Point(DemoControlSize.Width / 2, DemoControlSize.Height / 2);

        public RichCanvasTestAppTest() : base(RichCanvasDemoBinPath)
        {
        }

        [TearDown]
        public void UITestBaseTearDown()
        {
            Window.ClearAllItems();
        }
    }
}
