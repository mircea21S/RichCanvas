using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FluentAssertions;
using NUnit.Framework;
using RichCanvasUITests.App.Automation;
using System;
using System.Drawing;

namespace RichCanvas.UITests.Tests.Zooming
{
    [TestFixture]
    public class ZoomingTests : RichCanvasTestAppTest
    {
        public override void TearDown()
        {
            RichItemsControl.ResetZoom();

            base.TearDown();
        }

        // TODO: bool "zoomIn" should be converted to an enum.
        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void RichCanvas_WhenUsingMouseWheelAndZoomKeyModifier_ShouldZoomAtMousePosition(bool zoomIn)
        {
            // arrange
            var desiredMousePosition = new Point(100, 100);
            Mouse.Position = desiredMousePosition;

            // act
            Zoom(zoomIn);

            // assert
            Mouse.Position.Should().Be(desiredMousePosition);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void RichCanvas_WhenZooming_ShouldUpdateViewportZoom(bool zoomIn)
        {
            // arrange
            var initialZoom = RichItemsControl.ViewportZoom;

            // act
            Zoom(zoomIn);

            // assert
            double expectedViewportZoom = zoomIn
                ? initialZoom * RichItemsControl.ScaleFactor
                : initialZoom / RichItemsControl.ScaleFactor;
            RichItemsControl.RichItemsControlData.ViewportZoom.Should().Be(expectedViewportZoom);
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
            var canvasPoint = mousePosition.ToCanvasDrawingPoint();
            Mouse.Position = canvasPoint;

            var mousePositionBeforeZooming = RichItemsControl.RichItemsControlData.MousePosition;

            // act
            Zoom(zoomIn);

            // assert
            var mousePositionAfterZooming = RichItemsControl.RichItemsControlData.MousePosition;
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
            var scaleFactor = RichItemsControl.ScaleFactor;
            var zoomBefore = RichItemsControl.ViewportZoom;

            // act
            Zoom(zoomIn);

            // assert
            if (zoomIn)
            {
                RichItemsControl.ViewportZoom.Should().Be(scaleFactor * zoomBefore);
            }
            else
            {
                RichItemsControl.ViewportZoom.Should().Be((1 / scaleFactor) * zoomBefore);
            }
        }

        [Test]
        public void RichCanvas_WhenZoomIn_ShouldStopAtMaxScale()
        {
            // arrange
            var maxZoom = RichItemsControl.RichItemsControlData.MaxZoom;
            var scaleFactor = RichItemsControl.ScaleFactor;
            double noOfZoomsUntilMax = Math.Log(maxZoom) / Math.Log(scaleFactor);

            // act
            // Zoom in even though the max zoom is reached (that's why we add 4) just for test purposes
            for (int i = 0; i < noOfZoomsUntilMax + 4; i++)
            {
                Zoom(true);
            }

            // assert
            RichItemsControl.ViewportZoom.Should().Be(maxZoom);
        }

        [Test]
        public void RichCanvas_WhenZoomOut_ShouldStopAtMinScale()
        {
            // arrange
            var minZoom = RichItemsControl.RichItemsControlData.MinZoom;
            var scaleFactor = RichItemsControl.ScaleFactor;
            double noOfZoomsUntilMax = Math.Log(1 / minZoom) / Math.Log(scaleFactor);

            // act
            // Zoom out even though the max zoom is reached (that's why we add 4) just for test purposes
            for (int i = 0; i < noOfZoomsUntilMax + 4; i++)
            {
                Zoom(false);
            }

            // assert
            RichItemsControl.ViewportZoom.Should().Be(minZoom);
        }

        [Test]
        public void ZoomedRichCanvas_WhenMouseOnTopLeftCornerOfCanvas_ShouldBeViewportLocation()
        {
            // arrange
            var topLeftCornerPoint = new Point(0, 0);
            var canvasTopLeftCorner = topLeftCornerPoint.ToCanvasDrawingPoint();

            // act
            Mouse.Position = canvasTopLeftCorner;
            Zoom(false);
            Zoom(true);
            Zoom(true);
            Zoom(true);
            Zoom(false);
            Zoom(false);
            Zoom(false);

            // assert
            RichItemsControl.RichItemsControlData.MousePosition.Should().Be(RichItemsControl.ViewportLocation.AsWindowsPoint());
        }

        [TestCase("2")]
        [TestCase("3")]
        [TestCase("1.4")]
        [TestCase("1.2")]
        [TestCase("0.553")]
        [TestCase("0.1")]
        [TestCase("0.8")]
        [TestCase("0.02")]
        [Test]
        public void RichCanvas_WhenViewportZoomIsSetThroughBinding_ShouldUpdateZoom(string viewportZoomValue)
        {
            // arrange
            var viewportZoomTextBox = Window.FindFirstDescendant(d => d.ByAutomationId(AutomationIds.ViewportZoomTextBoxId)).AsTextBox();

            // act
            viewportZoomTextBox.Patterns.Value.Pattern.SetValue(viewportZoomValue);
            // lose focus to trigger the binding
            Keyboard.Press(FlaUI.Core.WindowsAPI.VirtualKeyShort.TAB);

            // assert
            var setZoomValue = double.Parse(viewportZoomValue);
            if (setZoomValue > RichItemsControl.RichItemsControlData.MaxZoom)
            {
                setZoomValue = RichItemsControl.RichItemsControlData.MaxZoom;
            }
            else if (setZoomValue < RichItemsControl.RichItemsControlData.MinZoom)
            {
                setZoomValue = RichItemsControl.RichItemsControlData.MinZoom;
            }
            RichItemsControl.ViewportZoom.Should().Be(setZoomValue);
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
                Zoom(true);
            }

            // assert
            var expectedScrollOffset = ViewportLocation - RichItemsControl.RichItemsControlData.ItemsExtent.Location;
            var expectedExtent = RichItemsControl.RichItemsControlData.ItemsExtent;
            expectedExtent.Union(new System.Windows.Rect(ViewportLocation, ViewportSize));
            var doubleTolerance = 1e-5;

            RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.Y < 0 ? 0 : expectedScrollOffset.Y, doubleTolerance);
            RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.X < 0 ? 0 : expectedScrollOffset.X, doubleTolerance);
            RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, doubleTolerance);
            RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, doubleTolerance);
        }

        [Test]
        public void RichCanvas_WhenZoomOutAfterZoomIn_ShouldUpdateScrolling()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            Mouse.Position = new Point(203, 303).ToCanvasDrawingPoint();
            for (int i = 0; i < 6; i++)
            {
                Zoom(true);
            }

            // act
            for (int i = 0; i < 3; i++)
            {
                Zoom(false);
            }

            // assert
            var expectedScrollOffset = ViewportLocation - RichItemsControl.RichItemsControlData.ItemsExtent.Location;
            var expectedExtent = RichItemsControl.RichItemsControlData.ItemsExtent;
            expectedExtent.Union(new System.Windows.Rect(ViewportLocation, ViewportSize));
            var doubleTolerance = 1e-5;

            RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.Y < 0 ? 0 : expectedScrollOffset.Y, doubleTolerance);
            RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.X < 0 ? 0 : expectedScrollOffset.X, doubleTolerance);
            RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, doubleTolerance);
            RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, doubleTolerance);
        }

        private void Zoom(bool zoomIn)
        {
            if (zoomIn)
            {
                RichItemsControl.ZoomIn();
            }
            else
            {
                RichItemsControl.ZoomOut();
            }
        }
    }
}
