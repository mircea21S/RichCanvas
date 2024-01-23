using NUnit.Framework;
using System.Drawing;

namespace RichCanvas.UITests.Tests
{
    public class RichCanvasTestAppTest : UITestBase
    {
        internal const string RichCanvasDemoBinPath = @"C:\Programming Projects\RichCanvas\RichCanvasUITests.App\bin\Debug\RichCanvasUITests.App.exe";
        /// <summary>
        /// Size of Title bar (SystemParamters.WindowCaptionHeight) = 22.5
        /// </summary>
        public const double RichCanvasDemoTitleBarHeight = 22.5;

        protected RichItemsControlAutomation RichItemsControl => Window.FindFirstDescendant(d => d.ByAutomationId("source")).AsRichItemsControlAutomation();
        protected Size DemoControlSize => new Size(1187, 800);
        protected Point ViewportCenter => new Point(DemoControlSize.Width / 2, DemoControlSize.Height / 2);
        protected bool ShouldRestartApplication { get; set; }

        public RichCanvasTestAppTest() : base(RichCanvasDemoBinPath)
        {
        }

        [SetUp]
        public void SetUp()
        {
            if (ShouldRestartApplication)
            {
                StartApplication(RichCanvasDemoBinPath);
            }
        }

        [TearDown]
        public void TearDown()
        {
            Window.ClearAllItems();
            if (ShouldRestartApplication)
            {
                CloseApplication();
            }
        }
    }
}
