using System.Collections.ObjectModel;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.Helpers;

namespace RichCanvas.UIAutomation.Tests.Tests.Drawing
{
    [TestFixture]
    public class DrawingIndexesTrackerTests
    {
        #region Reset

        [Test]
        public void DrawingIndexesTracker_WhenClearingItemsCollection_ShouldCallPassedResetAction()
        {
            // arrange
            var isExecuteOnResetActionCalled = false;
            var fakeItems = new ObservableCollection<int>();
            var drawingIndexesTracker = new DrawingIndexesTracker(fakeItems, i => true, () =>
            {
                isExecuteOnResetActionCalled = true;
            });

            // act
            fakeItems.Clear();

            // assert
            isExecuteOnResetActionCalled.Should().BeTrue();
        }

        #endregion Reset

        #region Add

        [Test]
        public void DrawingIndexesTracker_WhenAddingDrawableItems_ShouldTrackAllDrawableIndexes()
        {
            // arrange
            var fakeItems = new ObservableCollection<int>();
            var drawingIndexesTracker = new DrawingIndexesTracker(fakeItems, i => true);

            // act
            fakeItems.Add(1);
            fakeItems.Add(2);

            // assert
            drawingIndexesTracker.DrawingIndexes.Should().HaveCount(2);
            drawingIndexesTracker.DrawingIndexes.Should().ContainInOrder([0, 1]);
        }

        [Test]
        public void DrawingIndexesTracker_WhenAddingNonDrawableItems_ShouldIgnoreNonDrawableIndexes()
        {
            // arrange
            var fakeItems = new ObservableCollection<int>();
            var drawingIndexesTracker = new DrawingIndexesTracker(fakeItems, i =>
            {
                if (i == 2) return true;
                return false;
            });

            // act
            fakeItems.Add(1);
            fakeItems.Add(2);
            fakeItems.Add(3);

            // assert
            drawingIndexesTracker.DrawingIndexes.Should().HaveCount(1);
            drawingIndexesTracker.DrawingIndexes.Should().ContainInOrder([2]);
        }

        #endregion Add

        #region Remove

        [Test]
        public void DrawingIndexesTracker_WhenRemovingDrawableItem_ShouldTrackDrawableIndexes()
        {
            // arrange
            var fakeItems = new ObservableCollection<int>();
            var drawingIndexesTracker = new DrawingIndexesTracker(fakeItems, i => true);

            // act
            fakeItems.Add(1);
            fakeItems.Add(2);
            fakeItems.Remove(2);

            // assert
            drawingIndexesTracker.DrawingIndexes.Should().HaveCount(1);
            drawingIndexesTracker.DrawingIndexes.Should().ContainInOrder([0]);
        }

        [Test]
        public void DrawingIndexesTracker_WhenRemovingNonDrawableItem_ShouldNotRemoveFromDrawingIndexes()
        {
            // arrange
            var fakeItems = new ObservableCollection<int>();
            var drawingIndexesTracker = new DrawingIndexesTracker(fakeItems, i =>
            {
                if (i == 2) return false;
                return true;
            });

            // act
            fakeItems.Add(1);
            fakeItems.Add(2);
            fakeItems.Add(3);
            fakeItems.Remove(3);

            // assert
            drawingIndexesTracker.DrawingIndexes.Should().HaveCount(2);
            drawingIndexesTracker.DrawingIndexes.Should().ContainInOrder([0, 1]);
        }

        [Test]
        public void DrawingIndexesTracker_WhenRemovingIndexInTheMiddle_ShouldUpdateTrackedDrawingIndexes()
        {
            // arrange
            var fakeItems = new ObservableCollection<int>();
            var drawingIndexesTracker = new DrawingIndexesTracker(fakeItems, i => true);

            // act
            fakeItems.Add(1);
            fakeItems.Add(2);
            fakeItems.Add(3);
            fakeItems.Remove(2);

            // assert
            drawingIndexesTracker.DrawingIndexes.Should().HaveCount(2);
            drawingIndexesTracker.DrawingIndexes.Should().ContainInOrder([0, 1]);
        }

        #endregion Remove

        #region Move

        [Test]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void DrawingIndexesTracker_WhenMovingDrawableTrackedIndexes_ShouldKeepThemInTheSameOrder(int oldIndexMove, int newIndexMove)
        {
            // arrange
            var fakeItems = new ObservableCollection<int>();
            var drawingIndexesTracker = new DrawingIndexesTracker(fakeItems, i => true);

            // act
            fakeItems.Add(1);
            fakeItems.Add(2);
            fakeItems.Move(oldIndexMove, newIndexMove);

            // assert
            drawingIndexesTracker.DrawingIndexes.Should().HaveCount(2);
            drawingIndexesTracker.DrawingIndexes.Should().ContainInOrder([0, 1]);
        }

        [Test]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        public void DrawingIndexesTracker_WhenMovingDrawableTrackedIndexesInTheMiddle_ShouldKeepThemInTheSameOrder(int oldIndexMove, int newIndexMove)
        {
            // arrange
            var fakeItems = new ObservableCollection<int>();
            var drawingIndexesTracker = new DrawingIndexesTracker(fakeItems, i =>
            {
                if (i == 1 || i == 2) return true;
                return false;
            });

            // act
            fakeItems.Add(1);
            fakeItems.Add(2);
            fakeItems.Add(3);
            fakeItems.Add(4);
            fakeItems.Move(oldIndexMove, newIndexMove);

            // assert
            drawingIndexesTracker.DrawingIndexes.Should().HaveCount(2);
            drawingIndexesTracker.DrawingIndexes.Should().ContainInOrder([1, 2]);
        }

        [Test]
        public void DrawingIndexesTracker_WhenMovingIndexOutsideTrackedIndexes_ShouldNotUpdateDrawingIndexes()
        {
            // arrange
            var fakeItems = new ObservableCollection<int>();
            var drawingIndexesTracker = new DrawingIndexesTracker(fakeItems, i =>
            {
                if (i == 2 || i == 3) return true;
                return false;
            });

            // act
            fakeItems.Add(1);
            fakeItems.Add(2);
            fakeItems.Add(3);
            fakeItems.Add(4);
            fakeItems.Move(0, 1);

            // assert
            drawingIndexesTracker.DrawingIndexes.Should().HaveCount(2);
            drawingIndexesTracker.DrawingIndexes.Should().ContainInOrder([2, 3]);
        }

        [Test]
        public void DrawingIndexesTracker_WhenMovingOneOfTrackedIndexes_ShouldUpdateOnlyTheMovedIndex()
        {
            // arrange
            var fakeItems = new ObservableCollection<int>();
            var drawingIndexesTracker = new DrawingIndexesTracker(fakeItems, i =>
            {
                if (i == 2 || i == 3) return true;
                return false;
            });

            // act
            fakeItems.Add(1);
            fakeItems.Add(2);
            fakeItems.Add(3);
            fakeItems.Add(4);
            fakeItems.Move(0, 2);

            // assert
            drawingIndexesTracker.DrawingIndexes.Should().HaveCount(2);
            drawingIndexesTracker.DrawingIndexes.Should().ContainInOrder([1, 3]);
        }

        [Test]
        [TestCase(1, 4)]
        [TestCase(4, 1)]
        public void DrawingIndexesTracker_WhenMovingIndexesBetweenTrackedIndexes_ShouldUpdateDrawingIndexes(int oldIndex, int newIndex)
        {
            // arrange
            var fakeItems = new ObservableCollection<int>();
            var drawableIndex1 = 2;
            var drawableIndex2 = 3;
            var drawingIndexesTracker = new DrawingIndexesTracker(fakeItems, i =>
            {
                if (i == drawableIndex1 || i == drawableIndex2) return true;
                return false;
            });

            // act
            fakeItems.Add(1);
            fakeItems.Add(2);
            fakeItems.Add(3);
            fakeItems.Add(4);
            fakeItems.Add(5);
            fakeItems.Move(oldIndex, newIndex);

            // assert
            drawingIndexesTracker.DrawingIndexes.Should().HaveCount(2);
            if (oldIndex < newIndex)
            {
                drawingIndexesTracker.DrawingIndexes.Should().ContainInOrder([drawableIndex1 - 1, drawableIndex2 - 1]);
            }
            else
            {
                drawingIndexesTracker.DrawingIndexes.Should().ContainInOrder([drawableIndex1 + 1, drawableIndex2 + 1]);
            }
        }

        #endregion Move
    }
}
