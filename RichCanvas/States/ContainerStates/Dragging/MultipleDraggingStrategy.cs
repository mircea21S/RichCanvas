using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace RichCanvas.States.ContainerStates
{
    public class MultipleDraggingStrategy : DraggingStrategy
    {
        private readonly List<RichItemContainer> _draggableContainers = new List<RichItemContainer>(16);

        public MultipleDraggingStrategy(RichItemContainer container) : base(container)
        {
        }

        public override void OnItemsDragStarted()
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
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    var container = (RichItemContainer)Parent.ItemContainerGenerator.ContainerFromItem(selectedItems[i]);
                    if (container.IsDraggable)
                    {
                        _draggableContainers.Add(container);
                    }
                }

                Parent.ItemsHost.InvalidateArrange();
            }
        }

        public override void OnItemsDragDelta(Point offsetPoint)
        {
            for (int i = 0; i < _draggableContainers.Count; i++)
            {
                RichItemContainer container = _draggableContainers[i];
                TranslateTransform? translateTransform = container.TranslateTransform;

                if (Parent.RealTimeDraggingEnabled)
                {
                    container.Top += offsetPoint.Y;
                    container.Left += offsetPoint.X;
                }
                else
                {
                    if (translateTransform != null)
                    {
                        translateTransform.X += offsetPoint.X;
                        translateTransform.Y += offsetPoint.Y;
                        container.CalculateBoundingBox();
                        container.OnPreviewLocationChanged(new Point(container.Left + translateTransform.X, container.Top + translateTransform.Y));
                    }
                }
            }
        }

        public override void OnItemsDragCompleted()
        {
            for (int i = 0; i < _draggableContainers.Count; i++)
            {
                RichItemContainer container = _draggableContainers[i];
                if (!Parent.RealTimeDraggingEnabled)
                {
                    TranslateTransform? translateTransform = container.TranslateTransform;

                    if (translateTransform != null)
                    {
                        container.Left += translateTransform.X;
                        container.Top += translateTransform.Y;
                        translateTransform.X = 0;
                        translateTransform.Y = 0;
                    }
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
