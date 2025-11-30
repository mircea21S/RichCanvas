using System.Drawing;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;

using FluentAssertions;
using FluentAssertions.Execution;

using NUnit.Framework;

using RichCanvas.UIAutomation.FlaUIClient;
using RichCanvas.UIAutomation.Tests.Extensions;
using RichCanvas.UIAutomation.Tests.Utilities;

using RichCanvasUIA.Client.UIA_Mode;

namespace RichCanvas.UIAutomation.Tests.Tests.Drawing
{
    [TestFixture]
    public class DrawingStateTests : RichCanvasTestAppTest
    {
        [TestCase(1, 1)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, -1)]
        [Test]
        public void DrawScaledItem_WithAllowScaleToUpdatePositionFalse_ShouldNotModifyTopAndLeft(int scaleX, int scaleY)
        {
            // arrange
            var mockRectangle = PreDefinedAutomationItemModels.ImmutablePositionedRectangleWithoutSize;
            var mockRectangleSize = new Size(50, 50);

            // act
            RichCanvasUIAClientCommunicator.AddImmutableRectangle();
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];

            var drawingStartPoint = new UIAClientAppPoint(drawnContainer.Location);
            var drawingEndPoint = drawingStartPoint.GetEndPointByScale(mockRectangleSize, scaleX, scaleY);
            RichCanvas.Draw(drawingStartPoint, drawingEndPoint);

            // assert
            using (new AssertionScope())
            {
                drawnContainer.Location.Should().Be(new Point(mockRectangle.Left.ToInt(), mockRectangle.Top.ToInt()));
                drawnContainer.ActualWidth.Should().Be(mockRectangleSize.Width);
                drawnContainer.ActualHeight.Should().Be(mockRectangleSize.Height);
            }
        }

        [TestCase(1, 1)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, -1)]
        [Test]
        public void DrawScaledItem_WithAllowScaleToUpdatePositionTrue_ShouldModifyTopAndLeft(int scaleX, int scaleY)
        {
            // arrange
            var rectangleMock = PreDefinedAutomationItemModels.ImmutablePositionedRectangleWithoutSize;
            var mockRectangleSize = new Size(50, 50);

            // act
            RichCanvasUIAClientCommunicator.AddPositionedRectangle();
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            var drawingStartPoint = new UIAClientAppPoint(drawnContainer.Location);
            var drawingEndPoint = drawingStartPoint.GetEndPointByScale(mockRectangleSize, scaleX, scaleY);
            RichCanvas.Draw(drawingStartPoint, drawingEndPoint);

            // assert
            double expectedTop = scaleY switch
            {
                -1 => rectangleMock.Top - drawnContainer.ActualHeight,
                1 => rectangleMock.Top,
                _ => rectangleMock.Top,
            };
            double expectedLeft = scaleX switch
            {
                1 => rectangleMock.Left,
                -1 => rectangleMock.Left - drawnContainer.ActualWidth,
                _ => rectangleMock.Left,
            };
            using (new AssertionScope())
            {
                drawnContainer.Location.Should().Be(new Point(expectedLeft.ToInt(), expectedTop.ToInt()));
            }
        }

        [TestCase(1, 1)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, -1)]
        [Test, ShouldExecuteDrawingEndedCommand(false)]
        public void DrawItemFromItemsSource_WhenNotInitialized_ContainerSizeIsTheDraggedSizeAndPositionIsRelativeToScaleTransform(int scaleX, int scaleY)
        {
            // arrange
            var containerSize = new Size(100, 100);

            // act
            RichCanvasUIAClientCommunicator.AddEmptyRectangle();
            var containerLocationPoint = RichCanvas.GetRandomPointOnRichCanvas();
            var visualContainerLocationPoint = new UIAClientAppPoint(containerLocationPoint);
            var containerDrawEndPoint = visualContainerLocationPoint.GetEndPointByScale(containerSize, scaleX, scaleY);
            RichCanvas.Draw(visualContainerLocationPoint, containerDrawEndPoint);

            // assert
            var drawnRectangleContainer = RichCanvas.Items[0];

            double expectedTop = scaleY switch
            {
                -1 => containerLocationPoint.Y - drawnRectangleContainer.ActualHeight,
                1 => containerLocationPoint.Y,
                _ => containerLocationPoint.Y,
            };
            double expectedLeft = scaleX switch
            {
                1 => containerLocationPoint.X,
                -1 => containerLocationPoint.X - drawnRectangleContainer.ActualWidth,
                _ => containerLocationPoint.X,
            };
            drawnRectangleContainer.Location.Should().Be(new Point(expectedLeft.ToInt(), expectedTop.ToInt()));
            drawnRectangleContainer.ActualWidth.Should().Be(containerSize.Width);
            drawnRectangleContainer.ActualHeight.Should().Be(containerSize.Height);
            drawnRectangleContainer.GetRichCanvasContainerSettings().ScaleX.Should().Be(scaleX);
            drawnRectangleContainer.GetRichCanvasContainerSettings().ScaleY.Should().Be(scaleY);
        }

        [Test]
        public void AddContainerWithBoundPositionAndSize_ShouldDrawContainerWithPositionAndSizeTheSameAsSpecified()
        {
            // arrange
            var mockRectangle = PreDefinedAutomationItemModels.FullyDrawnRectangle;

            // act
            RichCanvasUIAClientCommunicator.AddDrawnRectangle();

            // assert
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            using (new AssertionScope())
            {
                drawnContainer.GetRichCanvasContainerSettings().Top.Should().Be(mockRectangle.Top);
                drawnContainer.GetRichCanvasContainerSettings().Left.Should().Be(mockRectangle.Left);
                drawnContainer.ActualHeight.Should().Be(mockRectangle.Height);
                drawnContainer.ActualWidth.Should().Be(mockRectangle.Width);
            }
        }

        [Test]
        public void DrawItem_WhenDrawingIsFinished_ShouldInvokeDrawEndedCommand()
        {
            // arrange
            var itemSize = new Size(40, 40);

            // act
            RichCanvasUIAClientCommunicator.AddEmptyRectangle();
            var containerLocationPoint = new UIAClientAppPoint(RichCanvas.GetRandomPointOnRichCanvas());
            var containerDrawEndPoint = containerLocationPoint.GetEndPointByScale(itemSize);
            RichCanvas.Draw(containerLocationPoint, containerDrawEndPoint);

            // assert
            TextBox drawingEndedTextBox = RichCanvas.FindFirstDescendant(x => x.ByAutomationId(AutomationIds.DrawingEndedTextBoxId)).AsTextBox();
            drawingEndedTextBox.Name.Should().Be("DRAWING ENDED");
        }
    }
}