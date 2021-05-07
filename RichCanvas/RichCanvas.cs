using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace RichCanvas
{
    public class RichCanvas : VirtualizingPanel
    {
        internal Rect BoundingBox { get; private set; }

        internal RichItemsControl ItemsOwner { get; set; }

        protected override Size MeasureOverride(Size constraint)
        {
            if (ItemsOwner.IsDrawing && !ItemsOwner.NeedMeasure)
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
                minX = Math.Min(minX, container.Left);
                minY = Math.Min(minY, container.Top);
                maxX = Math.Max(maxX, container.Left + container.Width);
                maxY = Math.Max(maxY, container.Top + container.Height);
            }
            BoundingBox = new Rect(minX, minY, Math.Abs(maxX), Math.Abs(maxY));

            CleanupItems();

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

        private void VirtualizeItems()
        {
            IItemContainerGenerator generator = ItemsOwner.ItemContainerGenerator;
            var pos = generator.GeneratorPositionFromIndex(0);
            int childIndex = pos.Offset == 0 ? pos.Index : pos.Index + 1;

            using (generator.StartAt(pos, GeneratorDirection.Forward, true))
            {
                for (int i = 0; i < ItemsOwner.Items.Count; i++, childIndex++)
                {
                    var container = (RichItemContainer)generator.GenerateNext(out bool isNewlyRealized);
                    if (container.IsValid())
                    {
                        if (ContainerInViewport(container))
                        {
                            if (isNewlyRealized)
                            {
                                //if (childIndex >= InternalChildren.Count)
                                //{
                                //    AddInternalChild(container);
                                //}
                                //else
                                //{
                                if (i >= InternalChildren.Count)
                                {
                                    Console.WriteLine("Added " + i);
                                    AddInternalChild(container);
                                }
                                else
                                {
                                    Console.WriteLine("insert " + i);
                                    InsertInternalChild(i, container);
                                }

                                //}
                            }
                        }
                    }
                    else if (!container.IsDrawn)
                    {
                        if (isNewlyRealized)
                        {
                            //if (childIndex >= InternalChildren.Count)
                            //{
                            //    AddInternalChild(container);
                            //}
                            //else
                            //{
                            if (i >= InternalChildren.Count)
                            {
                                Console.WriteLine("Added invalid " + i);
                                AddInternalChild(container);
                            }
                            else
                            {
                                Console.WriteLine("insert invalid " + i);
                                InsertInternalChild(i, container);
                            }

                            //}
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
                if (!ContainerInViewport(container))
                {
                    generator.Remove(position, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        private bool ContainerInViewport(RichItemContainer container)
        {
            return container.Top >= ItemsOwner.ScrollContainer.TopLimit && container.Left + container.Width >= ItemsOwner.ScrollContainer.LeftLimit
                && container.Top + container.Height <= ItemsOwner.ScrollContainer.BottomLimit && container.Left + container.Width <= ItemsOwner.ScrollContainer.RightLimit;
        }
    }
}
