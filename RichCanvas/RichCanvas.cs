using RichCanvas.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace RichCanvas
{
    public class RichCanvas : VirtualizingPanel
    {
        private bool _boundingBoxInitialized;

        internal double TopLimit { get; private set; } = double.PositiveInfinity;
        internal double BottomLimit { get; private set; } = double.NegativeInfinity;
        internal double LeftLimit { get; private set; } = double.PositiveInfinity;
        internal double RightLimit { get; private set; } = double.NegativeInfinity;

        internal RichItemsControl ItemsOwner { get; set; }

        protected override Size MeasureOverride(Size constraint)
        {
            if ((ItemsOwner.IsDrawing || ItemsOwner.IsSelecting || DragBehavior.IsDragging) && !ItemsOwner.NeedMeasure)
            {
                return default;
            }

            VirtualizeItems();

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            foreach (UIElement child in InternalChildren)
            {
                var container = (RichItemContainer)child;
                container.Measure(constraint);
                if (container.IsValid())
                {
                    _boundingBoxInitialized = true;
                    minX = Math.Min(minX, container.Left);
                    minY = Math.Min(minY, container.Top);
                    maxX = Math.Max(maxX, container.Left + container.Width);
                    maxY = Math.Max(maxY, container.Top + container.Height);
                }
            }
            if (_boundingBoxInitialized)
            {
                TopLimit = minY;
                LeftLimit = minX;
                BottomLimit = maxY;
                RightLimit = maxX;
                ItemsOwner.SetValue(RichItemsControl.ViewportRectPropertyKey, new Rect(LeftLimit, TopLimit, 0, 0));
                ItemsOwner.AdjustScroll();
            }

            if (ItemsOwner.EnableVirtualization)
            {
                CleanupItems();
            }
            ItemsOwner.VisibleElementsCount = InternalChildren.Count;

            return default;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                if (child is RichItemContainer container)
                {
                    child.Arrange(new Rect(new Point(container.Left, container.Top), child.DesiredSize));
                }
            }
            return arrangeSize;
        }

        internal void VirtualizeItems()
        {
            IItemContainerGenerator generator = ItemsOwner.ItemContainerGenerator;
            var pos = generator.GeneratorPositionFromIndex(0);

            using (generator.StartAt(pos, GeneratorDirection.Forward, true))
            {
                for (int i = 0; i < ItemsOwner.Items.Count; i++)
                {
                    var container = (RichItemContainer)generator.GenerateNext(out bool isNewlyRealized);
                    if (container.IsValid())
                    {
                        if (ContainerInViewport(container) && ItemsOwner.EnableVirtualization)
                        {
                            if (isNewlyRealized)
                            {
                                if (i >= InternalChildren.Count)
                                {
                                    AddInternalChild(container);
                                }
                                else
                                {
                                    InsertInternalChild(i, container);
                                }
                            }
                        }
                        else if (!ItemsOwner.EnableVirtualization)
                        {
                            if (isNewlyRealized)
                            {
                                AddInternalChild(container);
                            }
                        }
                    }
                    else if (!container.IsDrawn)
                    {
                        if (isNewlyRealized)
                        {
                            if (i >= InternalChildren.Count)
                            {
                                AddInternalChild(container);
                            }
                            else
                            {
                                InsertInternalChild(i, container);
                            }
                        }
                    }
                    generator.PrepareItemContainer(container);
                }
            }
        }

        private void CleanupItems()
        {
            IItemContainerGenerator generator = ItemsOwner.ItemContainerGenerator;
            for (int i = InternalChildren.Count - 1; i >= 0; i--)
            {
                GeneratorPosition position = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(position);
                var container = (RichItemContainer)ItemsOwner.ItemContainerGenerator.ContainerFromIndex(itemIndex);

                if (!ContainerInViewport(container) && container.IsValid() && container != ItemsOwner.CurrentDrawingItem && ((DragBehavior.IsDragging && Mouse.LeftButton == MouseButtonState.Released) || !DragBehavior.IsDragging))
                {
                    generator.Remove(position, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        private bool ContainerInViewport(RichItemContainer container)
        {
            return container.Top + container.Height >= ItemsOwner.ScrollContainer.TopLimit && container.Left + container.Width >= ItemsOwner.ScrollContainer.LeftLimit
                && container.Top <= ItemsOwner.ScrollContainer.BottomLimit && container.Left <= ItemsOwner.ScrollContainer.RightLimit;
        }
    }
}
