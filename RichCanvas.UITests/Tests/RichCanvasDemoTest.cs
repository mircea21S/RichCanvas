using NUnit.Framework;
using System.Drawing;

namespace RichCanvas.UITests.Tests
{
    public class RichCanvasDemoTest : UITestBase
    {
        private const string RichCanvasDemoBinPath = @"C:\Programming Projects\RichCanvas\RichCanvasDemo\bin\Debug\net48\RichCanvasDemo.exe";
        // size of Title bar (SystemParamters.WindowCaptionHeight) = 22.5 -> it's 23 because of easier usage
        protected const int RichCavnasDemoTitleBarHeight = 23;

        protected RichItemsControlAutomation RichItemsControl => Window.FindFirstDescendant(d => d.ByAutomationId("source")).AsRichItemsControlAutomation();
        protected Size DemoControlSize => new Size(1187, 800);
        protected Point ViewportCenter => new Point(DemoControlSize.Width / 2, DemoControlSize.Height / 2);

        public RichCanvasDemoTest() : base(RichCanvasDemoBinPath)
        {
        }

        [TearDown]
        public void UITestBaseTearDown()
        {
            Window.ClearAllItems();
        }
    }
}
