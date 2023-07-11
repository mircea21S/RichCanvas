using RichCanvas.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RichCanvas.States.Dragging
{
    public class MultipleDraggingStrategy : DraggingStrategy
    {
        private readonly List<RichItemContainer> _draggableContainers = new List<RichItemContainer>(16);

        public MultipleDraggingStrategy(RichItemsControl parent) : base(parent)
        {
        }

        public override void OnItemsDragStarted(object sender, DragStartedEventArgs e)
        {
            IList selectedItems = Parent.BaseSelectedItems;

            if (selectedItems.Count > 0)
            {
                // Make sure we're not adding to a previous selection
                if (_draggableContainers.Count > 0)
                {
                    _draggableContainers.Clear();
                }

                // Increase cache capacity
                if (_draggableContainers.Capacity < selectedItems.Count)
                {
                    _draggableContainers.Capacity = selectedItems.Count;
                }

                // Cache selected containers
                for (var i = 0; i < selectedItems.Count; i++)
                {
                    var container = (RichItemContainer)Parent.ItemContainerGenerator.ContainerFromItem(selectedItems[i]);
                    if (container.IsDraggable)
                    {
                        _draggableContainers.Add(container);
                    }
                }

                Parent.ItemsHost?.InvalidateArrange();
                Parent.ScrollContainer?.SetCurrentScroll();
            }
        }

        public override void OnItemsDragDelta(object sender, DragDeltaEventArgs e)
        {
            var offset = new Point(e.HorizontalChange, e.VerticalChange);
            if (Parent.ItemsHost.HasTouchedExtentSizeLimit(offset))
            {
                return;
            }
            if (Parent.ItemsHost.HasTouchedNegativeLimit(offset))
            {
                return;
            }

            for (int i = 0; i < _draggableContainers.Count; i++)
            {
                RichItemContainer container = _draggableContainers[i];
                TranslateTransform? translateTransform = container.TranslateTransform;

                if (translateTransform != null)
                {
                    if (Parent.RealTimeDraggingEnabled)
                    {
                        container.Top += e.VerticalChange;
                        container.Left += e.HorizontalChange;
                    }
                    else
                    {
                        translateTransform.X += e.HorizontalChange;
                        translateTransform.Y += e.VerticalChange;
                        container.CalculateBoundingBox();
                        container.OnPreviewLocationChanged(new Point(container.Left + translateTransform.X, container.Top + translateTransform.Y));
                    }
                }

                //if (!container.Host.RealTimeDraggingEnabled)
                //{
                //    if (container == container.Host.ItemsHost?.BottomElement)
                //    {
                //        container.Host.ItemsHost.BottomElement = container;
                //    }

                //    if (container == container.Host.ItemsHost?.RightElement)
                //    {
                //        container.Host.ItemsHost.RightElement = container;
                //    }

                //    if (container == container.Host.ItemsHost?.TopElement)
                //    {
                //        container.Host.ItemsHost.TopElement = container;
                //    }

                //    if (container == container.Host.ItemsHost?.LeftElement)
                //    {
                //        container.Host.ItemsHost.LeftElement = container;
                //    }
                //}
            }

        }

        public override void OnItemsDragCompleted(object sender, DragCompletedEventArgs e)
        {
            for (var i = 0; i < _draggableContainers.Count; i++)
            {
                RichItemContainer container = _draggableContainers[i];
                var translateTransform = container.TranslateTransform;

                if (translateTransform != null)
                {
                    container.Left += translateTransform.X;
                    container.Top += translateTransform.Y;
                    translateTransform.X = 0;
                    translateTransform.Y = 0;
                }

                // Correct the final position
                if (Parent.EnableSnapping)
                {
                    container.Left = Math.Round(container.Left / container.Host.GridSpacing) * container.Host.GridSpacing;
                    container.Top = Math.Round(container.Top / container.Host.GridSpacing) * container.Host.GridSpacing;
                }
            }

            _draggableContainers.Clear();
        }
    }
}
