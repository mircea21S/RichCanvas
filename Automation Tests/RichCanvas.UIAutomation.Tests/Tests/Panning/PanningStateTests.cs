using System.Drawing;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.UIAutomation.Tests.Extensions;
using RichCanvas.UIAutomation.Tests.Utilities;

namespace RichCanvas.UIAutomation.Tests.Tests.Panning
{
    [TestFixture]
    public class PanningStateTests : RichCanvasTestAppTest
    {
        [Test]
        public void VisuallyPanningCanvas_ShouldUpdateViewportLocationToDraggedDistance()
        {
            // arrange
            RichCanvas.Focus();
            var initialViewportLocation = RichCanvas.ViewportLocation;

            // act
            Point startPoint = RichCanvas.GetRandomPointOnRichCanvas();
            Point endPoint = RichCanvas.GetRandomPointOnRichCanvas();
            var distanceVector = endPoint.AsWindowsPoint() - startPoint.AsWindowsPoint();

            RichCanvas.Pan(new UIAClientAppPoint(startPoint), new UIAClientAppPoint(endPoint));

            // assert
            var expectedLocation = new System.Windows.Point(initialViewportLocation.X - distanceVector.X, initialViewportLocation.Y - distanceVector.Y);
            RichCanvas.ViewportLocation.Should().Be(expectedLocation.AsDrawingPoint());

            RichCanvas.ViewportLocation = initialViewportLocation;
        }

        [TestCase(1, 10)]
        [TestCase(2, 10)]
        [TestCase(5, 5)]
        [Test]
        public void ProgramaticallyPanningCanvas_ShouldUpdateViewportLocationToDraggedDistance(int panningFactor, int panningDistance)
        {
            // arrange
            RichCanvas.Focus();
            var initialViewportLocation = RichCanvas.ViewportLocation;

            // act
            for (int i = 0; i < panningDistance; i++)
            {
                RichCanvas.Pan(panningFactor, panningFactor);
            }
            var distanceVector = new Point(panningFactor * panningDistance, panningFactor * panningDistance).AsWindowsPoint();

            // assert
            var expectedLocation = new System.Windows.Point(initialViewportLocation.X - distanceVector.X, initialViewportLocation.Y - distanceVector.Y);
            RichCanvas.ViewportLocation.Should().Be(expectedLocation.AsDrawingPoint());

            RichCanvas.ViewportLocation = initialViewportLocation;
        }

        [Test]
        public void PanningCanvas_ThroughBindedViewportLocation_ShouldUpdateTranslateTransformValue()
        {
            // act
            var initialViewportLocation = RichCanvas.ViewportLocation;
            RichCanvas.ViewportLocation = new Point(100, 100);

            // assert
            RichCanvas.GetRichCanvasSettings().TranslateTransformX.Should().Be(-100);
            RichCanvas.GetRichCanvasSettings().TranslateTransformY.Should().Be(-100);
            RichCanvas.ViewportLocation = initialViewportLocation;
        }
    }
}
