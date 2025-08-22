using System;
using System.Drawing;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;

using FluentAssertions;

using NUnit.Framework;

using RichCanvasUITests.App.Automation;

namespace RichCanvas.UITests.Tests.Zooming
{
    [TestFixture]
    public class ZoomingTests : RichCanvasTestAppTest
    {
        public override void TearDown()
        {
            RichCanvas.ResetZoom();

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
            RichCanvas.RichCanvasData.ViewportZoom.Should().Be(expectedViewportZoom);
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
            var mousePosition = new Point(xPosition, yPosition);
            Point canvasPoint = mousePosition.ToCanvasDrawingPoint();
            Mouse.Position = canvasPoint;

            System.Windows.Point mousePositionBeforeZooming = RichCanvas.RichCanvasData.MousePosition;

            // act
            RichCanvas.Zoom(zoomIn);

            // assert
            System.Windows.Point mousePositionAfterZooming = RichCanvas.RichCanvasData.MousePosition;
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
            double maxZoom = RichCanvas.RichCanvasData.MaxZoom;
            double scaleFactor = RichCanvas.ScaleFactor;
            double noOfZoomsUntilMax = Math.Log(maxZoom) / Math.Log(scaleFactor);

            // act
            // Zoom in even though the max zoom is reached (that's why we add 4) just for test purposes
            for (int i = 0; i < noOfZoomsUntilMax + 4; i++)
            {
                RichCanvas.Zoom(true);
            }

            // assert
            RichCanvas.ViewportZoom.Should().Be(maxZoom);
        }

        [Test]
        public void RichCanvas_WhenZoomOut_ShouldStopAtMinScale()
        {
            // arrange
            double minZoom = RichCanvas.RichCanvasData.MinZoom;
            double scaleFactor = RichCanvas.ScaleFactor;
            double noOfZoomsUntilMax = Math.Log(1 / minZoom) / Math.Log(scaleFactor);

            // act
            // Zoom out even though the max zoom is reached (that's why we add 4) just for test purposes
            for (int i = 0; i < noOfZoomsUntilMax + 4; i++)
            {
                RichCanvas.Zoom(false);
            }

            // assert
            RichCanvas.ViewportZoom.Should().Be(minZoom);
        }

        [Test]
        public void ZoomedRichCanvas_WhenMouseOnTopLeftCornerOfCanvas_ShouldBeViewportLocation()
        {
            // arrange
            var topLeftCornerPoint = new Point(0, 0);
            Point canvasTopLeftCorner = topLeftCornerPoint.ToCanvasDrawingPoint();

            // act
            Mouse.Position = canvasTopLeftCorner;
            RichCanvas.Zoom(false);
            RichCanvas.Zoom(true);
            RichCanvas.Zoom(true);
            RichCanvas.Zoom(true);
            RichCanvas.Zoom(false);
            RichCanvas.Zoom(false);
            RichCanvas.Zoom(false);

            // assert
            RichCanvas.RichCanvasData.MousePosition.Should().Be(RichCanvas.ViewportLocation.AsWindowsPoint());
        }

        [TestCase(2)]
        [TestCase(3)]
        [TestCase(1.4)]
        [TestCase(1.2)]
        [TestCase(0.553)]
        [TestCase(0.1)]
        [TestCase(0.8)]
        [TestCase(0.02)]
        [Test]
        public void RichCanvas_WhenViewportZoomIsSetThroughBinding_ShouldUpdateZoom(double viewportZoomValue)
        {
            // arrange & act
            RichCanvas.SetViewportZoom(viewportZoomValue);
            // lose focus to trigger the binding
            Keyboard.Press(FlaUI.Core.WindowsAPI.VirtualKeyShort.TAB);

            // assert
            if (viewportZoomValue > RichCanvas.RichCanvasData.MaxZoom)
            {
                viewportZoomValue = RichCanvas.RichCanvasData.MaxZoom;
            }
            else if (viewportZoomValue < RichCanvas.RichCanvasData.MinZoom)
            {
                viewportZoomValue = RichCanvas.RichCanvasData.MinZoom;
            }
            RichCanvas.ViewportZoom.Should().Be(viewportZoomValue);
        }

        [Test]
        public void RichCanvas_WhenZoomIn_ShouldUpdateScrolling()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            Mouse.Position = new Point(203, 303).ToCanvasDrawingPoint();

            // act
            for (int i = 0; i < 4; i++)
            {
                RichCanvas.Zoom(true);
            }

            // assert
            System.Windows.Vector expectedScrollOffset = ViewportLocation - RichCanvas.RichCanvasData.ItemsExtent.Location;
            System.Windows.Rect expectedExtent = RichCanvas.RichCanvasData.ItemsExtent;
            expectedExtent.Union(new System.Windows.Rect(ViewportLocation, ViewportSize));
            double doubleTolerance = 1e-5;

            RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.Y < 0 ? 0 : expectedScrollOffset.Y, doubleTolerance);
            RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.X < 0 ? 0 : expectedScrollOffset.X, doubleTolerance);
            RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, doubleTolerance);
            RichCanvas.RichCanvasData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, doubleTolerance);
        }

        [Test]
        public void RichCanvas_WhenZoomOutAfterZoomIn_ShouldUpdateScrolling()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            Mouse.Position = new Point(203, 303).ToCanvasDrawingPoint();
            for (int i = 0; i < 6; i++)
            {
                RichCanvas.Zoom(true);
            }

            // act
            for (int i = 0; i < 3; i++)
            {
                RichCanvas.Zoom(false);
            }

            // assert
            System.Windows.Vector expectedScrollOffset = ViewportLocation - RichCanvas.RichCanvasData.ItemsExtent.Location;
            System.Windows.Rect expectedExtent = RichCanvas.RichCanvasData.ItemsExtent;
            expectedExtent.Union(new System.Windows.Rect(ViewportLocation, ViewportSize));
            double doubleTolerance = 1e-5;

            RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.Y < 0 ? 0 : expectedScrollOffset.Y, doubleTolerance);
            RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.X < 0 ? 0 : expectedScrollOffset.X, doubleTolerance);
            RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, doubleTolerance);
            RichCanvas.RichCanvasData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, doubleTolerance);
        }
    }
}
