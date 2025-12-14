using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace RichCanvas.Helpers
{
    /// <summary>
    /// Class holding a List of indexes valid for drawing, build based on a <see cref="INotifyCollectionChanged"/> collection.
    /// </summary>
    public class DrawingIndexesTracker
    {
        private readonly Action? _executeOnReset;
        private readonly Func<int, bool> _shouldDrawIndex;

        /// <summary>
        /// List of <see cref="int"/> holding valid indexes for drawing the items from collection passed inside the <see cref="DrawingIndexesTracker"/> constructor.
        /// </summary>
        public List<int> DrawingIndexes { get; } = [];

        /// <summary>
        /// Creates a new instance of <see cref="DrawingIndexesTracker"/>.
        /// </summary>
        /// <param name="items">Collection used to listen for changes and build <see cref="DrawingIndexes"/> based on it.</param>
        /// <param name="isItemAtIndexValidForDrawingFunction"><see cref="Func{T, TResult}"/> recieving last added index and returning a <see cref="bool"/> indicating if the index is valid to be stored inside <see cref="DrawingIndexes"/>.</param>
        /// <param name="executeOnReset"><see cref="Action"/> called when <paramref name="items"/> collection is reset. Default is null.</param>
        public DrawingIndexesTracker(INotifyCollectionChanged items,
            Func<int, bool> isItemAtIndexValidForDrawingFunction,
            Action? executeOnReset = null)
        {
            items.CollectionChanged += OnItemsCollectionChanged;
            _executeOnReset = executeOnReset;
            _shouldDrawIndex = isItemAtIndexValidForDrawingFunction;
        }

        private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                DrawingIndexes.Clear();
                _executeOnReset?.Invoke();
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var newIndex = e.NewStartingIndex;
                var newAddedItemsCount = e.NewItems?.Count ?? 1;
                for (int i = 0; i < DrawingIndexes.Count; i++)
                {
                    if (DrawingIndexes[i] >= newIndex)
                        DrawingIndexes[i] += newAddedItemsCount;
                }

                for (int i = 0; i < newAddedItemsCount; i++)
                {
                    int insertedIndex = newIndex + i;
                    if (_shouldDrawIndex(insertedIndex))
                    {
                        DrawingIndexes.Add(insertedIndex);
                    }
                }

                DrawingIndexes.Sort();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                int removedIndex = e.OldStartingIndex;
                for (int i = DrawingIndexes.Count - 1; i >= 0; i--)
                {
                    if (DrawingIndexes[i] == removedIndex)
                        DrawingIndexes.RemoveAt(i);
                    else if (DrawingIndexes[i] > removedIndex)
                        DrawingIndexes[i]--;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                var oldIndex = e.OldStartingIndex;
                var newIndex = e.NewStartingIndex;
                if (oldIndex == newIndex) return;

                var currentDrawingIndexes = DrawingIndexes.ToArray();
                var updatedDrawingIndexes = new List<int>(currentDrawingIndexes.Length);

                foreach (var currentDrawingIndex in currentDrawingIndexes)
                {
                    int updatedDrawingIndex;
                    if (currentDrawingIndex == oldIndex)
                    {
                        updatedDrawingIndex = newIndex;
                    }
                    else if (oldIndex < newIndex)
                    {
                        if (currentDrawingIndex > oldIndex && currentDrawingIndex <= newIndex) updatedDrawingIndex = currentDrawingIndex - 1;
                        else updatedDrawingIndex = currentDrawingIndex;
                    }
                    else
                    {
                        if (currentDrawingIndex >= newIndex && currentDrawingIndex < oldIndex) updatedDrawingIndex = currentDrawingIndex + 1;
                        else updatedDrawingIndex = currentDrawingIndex;
                    }

                    updatedDrawingIndexes.Add(updatedDrawingIndex);
                }

                DrawingIndexes.Clear();
                DrawingIndexes.AddRange(updatedDrawingIndexes);
                DrawingIndexes.Sort();
            }
            // Replace event not implemented because the index doesn't change
        }
    }
}
