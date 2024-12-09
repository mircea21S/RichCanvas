using FluentAssertions;
using NUnit.Framework;
using RichCanvasUITests.App.Automation;
using RichCanvasUITests.App.TestMocks;
using System.Drawing;

namespace RichCanvas.UITests.Tests
{
    [TestFixture]
    public class PanningStateTests : RichCanvasTestAppTest
    {
        [TestCase(Direction.Left, 50)]
        [TestCase(Direction.Up, 50)]
        [TestCase(Direction.Down, 50)]
        [TestCase(Direction.Right, 50)]
        [TestCase(Direction.Left, -50)]
        [TestCase(Direction.Up, -50)]
        [TestCase(Direction.Down, -50)]
        [TestCase(Direction.Right, -50)]
        [TestCase(Direction.Left, 100)]
        [TestCase(Direction.Up, 100)]
        [TestCase(Direction.Down, 100)]
        [TestCase(Direction.Right, 100)]
        [Test]
        public void PanningCanvas_InDirectionByDistance_ShouldUpdateViewportLocationToDraggedDistance(Direction direction, int distance)
        {
            // arrange
            RichItemsControl.Focus();
            var fromPoint = ViewportCenter;
            var toPoint = direction switch
            {
                Direction.Up => new Point(fromPoint.X, fromPoint.Y - distance),
                Direction.Down => new Point(fromPoint.X, fromPoint.Y + distance),
                Direction.Left => new Point(fromPoint.X - distance, fromPoint.Y),
                Direction.Right => new Point(fromPoint.X + distance, fromPoint.Y),
                _ => throw new System.NotImplementedException(),
            };

            var distanceVector = toPoint.AsWindowsPoint() - fromPoint.AsWindowsPoint();
            var initialViewportLocation = ViewportLocation;

            // act
            RichItemsControl.Pan(fromPoint, toPoint);

            // assert
            var expectedLocation = new System.Windows.Point(initialViewportLocation.X - distanceVector.X, initialViewportLocation.Y - distanceVector.Y);
            ViewportLocation.Should().Be(expectedLocation);
            RichItemsControl.ResetViewportLocation();
        }

        [TestCase(Direction.Left, 50)]
        [TestCase(Direction.Up, 50)]
        [TestCase(Direction.Down, 50)]
        [TestCase(Direction.Right, 50)]
        [TestCase(Direction.Left, -50)]
        [TestCase(Direction.Up, -50)]
        [TestCase(Direction.Down, -50)]
        [TestCase(Direction.Right, -50)]
        [TestCase(Direction.Left, 100)]
        [TestCase(Direction.Up, 100)]
        [TestCase(Direction.Down, 100)]
        [TestCase(Direction.Right, 100)]
        [Test]
        public void PanningCanvas_InDirectionByDistance_ShouldUpdateTranslateTransformValueToDraggedDistance(Direction direction, int distance)
        {
            // arrange
            RichItemsControl.Focus();
            var fromPoint = ViewportCenter;
            var toPoint = direction switch
            {
                Direction.Up => new Point(fromPoint.X, fromPoint.Y - distance),
                Direction.Down => new Point(fromPoint.X, fromPoint.Y + distance),
                Direction.Left => new Point(fromPoint.X - distance, fromPoint.Y),
                Direction.Right => new Point(fromPoint.X + distance, fromPoint.Y),
                _ => throw new System.NotImplementedException(),
            };

            var distanceVector = toPoint.AsWindowsPoint() - fromPoint.AsWindowsPoint();

            // act
            RichItemsControl.Pan(fromPoint, toPoint);

            // assert
            RichItemsControl.RichItemsControlData.TranslateTransformX.Should().Be(distanceVector.X);
            RichItemsControl.RichItemsControlData.TranslateTransformY.Should().Be(distanceVector.Y);
            RichItemsControl.ResetViewportLocation();
        }

        [TestCase(Direction.Left, 50)]
        [TestCase(Direction.Up, 50)]
        [TestCase(Direction.Down, 50)]
        [TestCase(Direction.Right, 50)]
        [TestCase(Direction.Left, -50)]
        [TestCase(Direction.Up, -50)]
        [TestCase(Direction.Down, -50)]
        [TestCase(Direction.Right, -50)]
        [TestCase(Direction.Left, 100)]
        [TestCase(Direction.Up, 100)]
        [TestCase(Direction.Down, 100)]
        [TestCase(Direction.Right, 100)]
        [Test]
        public void MultipleCanvasPanning_InDirectionByDistance_ShouldAddUpDragDistanceToTranslateTransformValue(Direction direction, int distance)
        {
            // arrange
            RichItemsControl.Focus();
            var fromPoint = ViewportCenter;
            var toPoint = direction switch
            {
                Direction.Up => new Point(fromPoint.X, fromPoint.Y - distance),
                Direction.Down => new Point(fromPoint.X, fromPoint.Y + distance),
                Direction.Left => new Point(fromPoint.X - distance, fromPoint.Y),
                Direction.Right => new Point(fromPoint.X + distance, fromPoint.Y),
                _ => throw new System.NotImplementedException(),
            };

            var distanceVector = toPoint.AsWindowsPoint() - fromPoint.AsWindowsPoint();

            // act & assert
            RichItemsControl.Pan(fromPoint, toPoint);
            RichItemsControl.RichItemsControlData.TranslateTransformX.Should().Be(distanceVector.X);
            RichItemsControl.RichItemsControlData.TranslateTransformY.Should().Be(distanceVector.Y);

            RichItemsControl.Pan(fromPoint, toPoint);
            RichItemsControl.RichItemsControlData.TranslateTransformX.Should().Be(distanceVector.X * 2);
            RichItemsControl.RichItemsControlData.TranslateTransformY.Should().Be(distanceVector.Y * 2);

            RichItemsControl.ResetViewportLocation();
        }

        [Test]
        public void PanningCanvas_ThroughBindedViewportLocation_ShouldUpdateTranslateTransformValue()
        {
            // act
            Window.InvokeButton(AutomationIds.SetViewportLocationValueButtonId);

            // assert
            RichItemsControl.RichItemsControlData.TranslateTransformX.Should().Be(-PanningStateDataMocks.ViewportLocationMockValue.X);
            RichItemsControl.RichItemsControlData.TranslateTransformY.Should().Be(-PanningStateDataMocks.ViewportLocationMockValue.Y);
            RichItemsControl.ResetViewportLocation();
        }
    }
}
