using RichCanvas.Helpers;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RichCanvas.States.ContainerStates
{
    public class SingleDraggingStrategy : DraggingStrategy
    {
        public SingleDraggingStrategy(RichItemsControl parent) : base(parent)
        {
        }

        public override void OnItemsDragDelta(object sender, DragDeltaEventArgs e)
        {
            var offset = new Point(e.HorizontalChange, e.VerticalChange);
            var container = Parent.SelectedContainer;

            if (Parent.ItemsHost.HasTouchedNegativeLimit(offset))
            {
                return;
            }

            if (Parent.ItemsHost.HasTouchedExtentSizeLimit(offset))
            {
                return;
            }

            TranslateTransform? translateTransform = container.TranslateTransform;

            if (translateTransform != null)
            {
                if (container.Host.RealTimeDraggingEnabled)
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
        }

        public override void OnItemsDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var container = Parent.SelectedContainer;
            TranslateTransform? translateTransform = container.TranslateTransform;

            if (translateTransform != null)
            {
                container.Left += translateTransform.X;
                container.Top += translateTransform.Y;
                translateTransform.X = 0;
                translateTransform.Y = 0;
            }

            // Correct the final position
            if (container.Host.EnableSnapping)
            {
                container.Left = Math.Round(container.Left / container.Host.GridSpacing) * container.Host.GridSpacing;
                container.Top = Math.Round(container.Top / container.Host.GridSpacing) * container.Host.GridSpacing;
            }

            if (!container.Host.RealTimeDraggingEnabled)
            {
                container.Host.ScrollContainer?.SetCurrentScroll();
            }
        }
    }
}