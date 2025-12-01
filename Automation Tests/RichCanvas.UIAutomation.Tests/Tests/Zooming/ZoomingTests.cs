using System;
using System.Drawing;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements.Scrolling;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.UIAutomation.Tests.Extensions;

namespace RichCanvas.UIAutomation.Tests.Tests.Zooming
{
    [TestFixture]
    public class ZoomingTests : RichCanvasTestAppTest
    {
        private const double Tolerance = 1e-5;

        public override void TearDown()
        {
            RichCanvas.ResetZoom();
            RichCanvas.ViewportLocation = new Point(0, 0);

            base.TearDown();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void RichCanvas_WhenUsingMouseWheelAndZoomKeyModifier_ShouldZoomAtMousePosition(bool zoomIn)
        {
            // arrange
            var desiredMousePosition = new Point(100, 100);
            Mouse.Position = desiredMousePosition;

            // act
            RichCanvas.Zoom(zoomIn);

            // assert
            Mouse.Position.Should().Be(desiredMousePosition);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void RichCanvas_WhenZooming_ShouldUpdateViewportZoom(bool zoomIn)
        {
            // arrange
            double initialZoom = RichCanvas.ViewportZoom;

            // act
            RichCanvas.Zoom(zoomIn);

            // assert
            double expectedViewportZoom = zoomIn
                ? initialZoom * RichCanvas.ScaleFactor
                : initialZoom / RichCanvas.ScaleFactor;
            RichCanvas.ViewportZoom.Should().Be(expectedViewportZoom);
        }

        [TestCase(100, 100, true)]
        [TestCase(110, 110, true)]
        [TestCase(120, 120, true)]
        [TestCase(120, 120, true)]
        [TestCase(150, 150, false)]
        [TestCase(120, 120, false)]
        [TestCase(123, 123, false)]
        [TestCase(105, 115, false)]
        [TestCase(130, 134, true)]
        [TestCase(121, 120, false)]
        [TestCase(90, 109, true)]
        [TestCase(88, 100, false)]
        [Test]
        public void RichCanvas_WhenZoomingAtMultiplePositions_ShouldKeepEachPositionSteadyThroughoutZooming(int xPosition, int yPosition, bool zoomIn)
        {
            // arrange
            Mouse.Position = CreatePointRelativeToRichCanvasElement(xPosition, yPosition);
            System.Windows.Point mousePositionBeforeZooming = GetCurrentRichCanvasElement().GetRichCanvasSettings().MousePosition;

            // act
            RichCanvas.Zoom(zoomIn);

            // assert
            System.Windows.Point mousePositionAfterZooming = GetCurrentRichCanvasElement().GetRichCanvasSettings().MousePosition;
            mousePositionBeforeZooming.Should().Be(mousePositionAfterZooming);
        }

        [TestCase(true)]
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(false)]
        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void RichCanvas_WhenZooming_EachStepShouldBeChangedAccordingToScaleFactor(bool zoomIn)
        {
            // arrange
            double scaleFactor = RichCanvas.ScaleFactor;
            double zoomBefore = RichCanvas.ViewportZoom;

            // act
            RichCanvas.Zoom(zoomIn);

            // assert
            if (zoomIn)
            {
                RichCanvas.ViewportZoom.Should().Be(scaleFactor * zoomBefore);
            }
            else
            {
                RichCanvas.ViewportZoom.Should().Be((1 / scaleFactor) * zoomBefore);
            }
        }

        [Test]
        public void RichCanvas_WhenZoomIn_ShouldStopAtMaxScale()
        {
            // arrange
            double maxZoom = RichCanvas.GetRichCanvasSettings().MaxZoom;
            double scaleFactor = RichCanvas.ScaleFactor;
            double noOfZoomsUntilMax = Math.Log(maxZoom) / Math.Log(scaleFactor);

            // act
            // Zoom in even though the max zoom is reached (that's why we add 4) just for test purposes
            for (int i = 0; i < noOfZoomsUntilMax + 4; i++)
            {
                RichCanvas.Zoom(true);
            }

            // assert
            GetCurrentRichCanvasElement().ViewportZoom.Should().Be(maxZoom);
        }

        [Test]
        public void RichCanvas_WhenZoomOut_ShouldStopAtMinScale()
        {
            // arrange
            double minZoom = RichCanvas.GetRichCanvasSettings().MinZoom;
            double scaleFactor = RichCanvas.ScaleFactor;
            double noOfZoomsUntilMax = Math.Log(1 / minZoom) / Math.Log(scaleFactor);

            // act
            // Zoom out even though the max zoom is reached (that's why we add 4) just for test purposes
            for (int i = 0; i < noOfZoomsUntilMax + 4; i++)
            {
                RichCanvas.Zoom(false);
            }

            // assert
            GetCurrentRichCanvasElement().ViewportZoom.Should().Be(minZoom);
        }

        [Test]
        public void ZoomedRichCanvas_WhenMouseOnTopLeftCornerOfCanvas_ShouldBeViewportLocation()
        {
            // arrange & act
            Mouse.Position = CreatePointRelativeToRichCanvasElement(0, 0);
            RichCanvas.Zoom(false);
            RichCanvas.Zoom(true);
            RichCanvas.Zoom(true);
            RichCanvas.Zoom(true);
            RichCanvas.Zoom(false);
            RichCanvas.Zoom(false);
            RichCanvas.Zoom(false);

            // assert
            GetCurrentRichCanvasElement().GetRichCanvasSettings().MousePosition.X
                .Should().BeApproximately(GetCurrentRichCanvasElement().ViewportLocation.AsWindowsPoint().X, Tolerance);
            GetCurrentRichCanvasElement().GetRichCanvasSettings().MousePosition.Y
                .Should().BeApproximately(GetCurrentRichCanvasElement().ViewportLocation.AsWindowsPoint().Y, Tolerance);
        }

        [Test]
        public void RichCanvas_WhenZoomIn_ShouldUpdateScrolling()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSelectableItems();
            Mouse.Position = CreatePointRelativeToRichCanvasElement(203, 303);

            // act
            for (int i = 0; i < 4; i++)
            {
                RichCanvas.Zoom(true);
            }

            // assert
            ScrollbarsShouldBeVisible();
        }

        [Test]
        public void RichCanvas_WhenZoomOutAfterZoomIn_ShouldUpdateScrolling()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSelectableItems();
            Mouse.Position = CreatePointRelativeToRichCanvasElement(203, 303);

            // act & assert
            for (int i = 0; i < 6; i++)
            {
                RichCanvas.Zoom(true);
            }
            ScrollbarsShouldBeVisible();

            for (int i = 0; i < 6; i++)
            {
                RichCanvas.Zoom(false);
            }
            ScrollbarsShouldNotBeVisible();
        }

        private void ScrollbarsShouldBeVisible()
        {
            VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
            verticalScrollBar.Should().NotBeNull();
            HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
            horizontalScrollBar.Should().NotBeNull();
        }

        private void ScrollbarsShouldNotBeVisible()
        {
            VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
            verticalScrollBar.Should().BeNull();
            HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
            horizontalScrollBar.Should().BeNull();
        }
    }
}
