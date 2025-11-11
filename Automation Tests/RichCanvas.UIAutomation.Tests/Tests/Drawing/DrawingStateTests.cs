using System.Drawing;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;

using FluentAssertions;
using FluentAssertions.Execution;

using NUnit.Framework;

using RichCanvasUIA.Client;
using RichCanvasUIA.Client.Automation;
using RichCanvasUIA.Client.Models;
using RichCanvasUIA.Client.TestMocks;

using Point = System.Drawing.Point;

namespace RichCanvas.UIAutomation.Tests.Tests.Drawing
{
    [TestFixture]
    public class DrawingStateTests : RichCanvasTestAppTest
    {
        [Test, ShouldExecuteDrawingEndedCommand(false)]
        public void DrawFromItemsSourceWithTwoItems_AfterRemovingOneItem_ShouldDrawOnlyRemainingItem()
        {
            // arrange
            RichCanvas.AddEmptyRectangle();
            RichCanvas.AddEmptyLine();
            RichCanvas.RemoveFirstItem();

            // act
            RichCanvas.Draw(new Size(50, 50), out _);

            // assert
            RichCanvas.Items.Length.Should().Be(1);
            RichCanvasContainerAutomation itemDrawn = RichCanvas.Items[0];
            itemDrawn.RichCanvasContainerData.DataContextType.Should().Be(typeof(Line));
            itemDrawn.IsDrawn.Should().BeTrue();
        }

        [Test]
        public void DrawFromItemsSourceWithTwoItems_WhenMovingFirstToTheEnd_ShouldDrawItemsInOrder()
        {
            // arrange
            RichCanvas.AddEmptyRectangle();
            RichCanvas.AddEmptyLine();
            RichCanvas.MoveFirstItemToTheEnd();

            // draw first item
            RichCanvas.Draw(new Size(50, 50), out _);
            RichCanvasContainerAutomation firstItemDrawn = RichCanvas.Items[0];
            // assert
            firstItemDrawn.RichCanvasContainerData.DataContextType.Should().Be(typeof(Line));
            firstItemDrawn.IsDrawn.Should().BeTrue();

            // draw second item
            RichCanvas.Draw(new Size(50, 50), out _);
            RichCanvasContainerAutomation secondItemDrawn = RichCanvas.Items[1];
            // assert
            secondItemDrawn.RichCanvasContainerData.DataContextType.Should().Be(typeof(RichItemContainerModel));
            secondItemDrawn.IsDrawn.Should().BeTrue();
        }

        [Test]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, -1)]
        public void DrawScaledItem_WithAllowScaleToUpdatePositionFalse_ShouldNotModifyTopAndLeft(int scaleX, int scaleY)
        {
            // arrange
            RichItemContainerModel mockRectangle = DrawingStateDataMocks.ImmutablePositionedRectangleMockWithoutSize;
            var mockRectangleSize = new Size(50, 50);

            // act
            RichCanvas.AddImmutableRectangle();
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            RichCanvas.DrawPositionedContainer(drawnContainer, mockRectangleSize, scaleX, scaleY);

            // assert
            using (new AssertionScope())
            {
                drawnContainer.Location.Should().Be(new Point(mockRectangle.Left.ToInt(), mockRectangle.Top.ToInt()));
                drawnContainer.ActualWidth.Should().Be(mockRectangleSize.Width);
                drawnContainer.ActualHeight.Should().Be(mockRectangleSize.Height);
            }
        }

        [Test]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, -1)]
        public void DrawScaledItem_WithAllowScaleToUpdatePositionTrue_ShouldModifyTopAndLeft(int scaleX, int scaleY)
        {
            // arrange
            RichItemContainerModel rectangleMock = DrawingStateDataMocks.PositionedRectangleMockWithoutSize;
            var mockRectangleSize = new Size(50, 50);

            // act
            RichCanvas.AddPositionedRectangle();
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            RichCanvas.DrawPositionedContainer(drawnContainer, mockRectangleSize, scaleX, scaleY);

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

        [Test]
        [TestCase(1, 1)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, -1)]
        [ShouldExecuteDrawingEndedCommand(false)]
        public void DrawItemFromItemsSource_WhenNotInitialized_ContainerSizeIsTheDraggedSizeAndPositionIsRelativeToScaleTransform(int scaleX, int scaleY)
        {
            // arrange
            var containerSize = new Size(100, 100);

            // act
            RichCanvas.AddEmptyRectangle();
            RichCanvas.Draw(containerSize, out Point unscaledContainerLocation, scaleX, scaleY);

            // assert
            var drawnRectangleContainer = RichCanvas.Items[0];

            double expectedTop = scaleY switch
            {
                -1 => unscaledContainerLocation.Y - drawnRectangleContainer.ActualHeight,
                1 => unscaledContainerLocation.Y,
                _ => unscaledContainerLocation.Y,
            };
            double expectedLeft = scaleX switch
            {
                1 => unscaledContainerLocation.X,
                -1 => unscaledContainerLocation.X - drawnRectangleContainer.ActualWidth,
                _ => unscaledContainerLocation.X,
            };
            drawnRectangleContainer.Location.Should().Be(new Point(expectedLeft.ToInt(), expectedTop.ToInt()));
            drawnRectangleContainer.ActualWidth.Should().Be(containerSize.Width);
            drawnRectangleContainer.ActualHeight.Should().Be(containerSize.Height);
            drawnRectangleContainer.RichCanvasContainerData.ScaleX.Should().Be(scaleX);
            drawnRectangleContainer.RichCanvasContainerData.ScaleY.Should().Be(scaleY);
        }

        [Test]
        public void AddContainerWithBoundPositionAndSize_ShouldDrawContainerWithPositionAndSizeTheSameAsSpecified()
        {
            // arrange
            RichItemContainerModel mockRectangle = DrawingStateDataMocks.DrawnRectangleMock;

            // act
            RichCanvas.AddDrawnRectangle();

            // assert
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            using (new AssertionScope())
            {
                drawnContainer.RichCanvasContainerData.Top.Should().Be(mockRectangle.Top);
                drawnContainer.RichCanvasContainerData.Left.Should().Be(mockRectangle.Left);
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
            RichCanvas.AddEmptyRectangle();
            RichCanvas.Draw(itemSize, out _);

            // assert
            TextBox drawingEndedTextBox = RichCanvas.FindFirstDescendant(x => x.ByAutomationId(AutomationIds.DrawingEndedTextBoxId)).AsTextBox();
            drawingEndedTextBox.Name.Should().Be("DRAWING ENDED");
        }
    }
}