using RichCanvas.Helpers;
using System;
using System.Windows;
using System.Windows.Media;

namespace RichCanvas.States.ContainerStates
{
    public class SingleDraggingStrategy : DraggingStrategy
    {
        public SingleDraggingStrategy(RichItemsControl parent) : base(parent)
        {
        }

        public override void OnItemsDragDelta(Point offsetPoint)
        {
            var container = Parent.SingleSelectedContainer;

            if (Parent.ItemsHost.HasTouchedNegativeLimit(offsetPoint))
            {
                return;
            }

            if (Parent.ItemsHost.HasTouchedExtentSizeLimit(offsetPoint))
            {
                return;
            }

            TranslateTransform? translateTransform = container.TranslateTransform;

            if (translateTransform != null)
            {
                if (container.Host.RealTimeDraggingEnabled)
                {
                    container.Top += offsetPoint.Y;
                    container.Left += offsetPoint.X;
                }
                else
                {
                    translateTransform.X += offsetPoint.X;
                    translateTransform.Y += offsetPoint.Y;
                    container.CalculateBoundingBox();
                    container.OnPreviewLocationChanged(new Point(container.Left + translateTransform.X, container.Top + translateTransform.Y));
                }
            }
        }

        public override void OnItemsDragCompleted()
        {
            var container = Parent.SingleSelectedContainer;
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