using System;
using System.Windows;
using System.Windows.Media;

namespace RichCanvas.States.ContainerStates
{
    public class SingleDraggingStrategy : DraggingStrategy
    {
        public SingleDraggingStrategy(RichItemContainer container) : base(container)
        {
        }

        public override void OnItemsDragDelta(Point offsetPoint)
        {
            TranslateTransform? translateTransform = Container.TranslateTransform;

            if (translateTransform != null)
            {
                if (Container.Host.RealTimeDraggingEnabled)
                {
                    Container.Top += offsetPoint.Y;
                    Container.Left += offsetPoint.X;
                }
                else
                {
                    translateTransform.X += offsetPoint.X;
                    translateTransform.Y += offsetPoint.Y;
                    Container.CalculateBoundingBox();
                    Container.OnPreviewLocationChanged(new Point(Container.Left + translateTransform.X, Container.Top + translateTransform.Y));
                }
            }
        }

        public override void OnItemsDragCompleted()
        {
            TranslateTransform? translateTransform = Container.TranslateTransform;

            if (translateTransform != null)
            {
                Container.Left += translateTransform.X;
                Container.Top += translateTransform.Y;
                translateTransform.X = 0;
                translateTransform.Y = 0;
            }

            // Correct the final position
            if (Parent.EnableGrid && Parent.EnableSnapping)
            {
                Container.Left = Math.Round(Container.Left / Container.Host.GridSpacing) * Container.Host.GridSpacing;
                Container.Top = Math.Round(Container.Top / Container.Host.GridSpacing) * Container.Host.GridSpacing;
            }
        }
    }
}