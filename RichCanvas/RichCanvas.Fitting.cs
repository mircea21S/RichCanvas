using System;
using System.Windows;

namespace RichCanvas
{
    public partial class RichCanvas
    {
        private void FitViewToChildren(double marginX, double marginY)
        {
            if (ItemsHost.Children.Count == 0)
            {
                return;
            }

            Rect overallBounds = new(0, 0, 0, 0);

            for (int i = 0; i < ItemsHost.Children.Count; i++)
            {
                var child = ContainerFromElement(ItemsHost.Children[i]);
                if (child is RichCanvasContainer container)
                {
                    overallBounds.Union(container.BoundingBox);
                }
            }
            overallBounds.Inflate(marginX, marginY);

            FitViewToRect(overallBounds);
        }

        private void FitViewToRect(Rect newView)
        {
            // Size / Size
            Vector zoomAmounts = new Vector(ActualWidth / newView.Size.Width, ActualHeight / newView.Size.Height);
            double zoomAmount = Math.Min(zoomAmounts.X, zoomAmounts.Y);

            // Set viewport zoom
            ViewportZoom = zoomAmount;

            // If the canvas has a ScrollViewer and the scrollbars are visible,
            // the offset will be off by a tad. Calling this command immediately again will fix it.
            // We can check if there are scrollbars present by doing:
            //      if (ExtentHeight != ViewportSize.Height) => VerticalScrollBar present
            //      if (ExtentWidth != ViewportSize.Width) => HorizontalScrollBar present
            // You can then try to use SystemParameters.ScrollWidth to try and account for it,
            // but I could not get the offset perfect. Additionally, if the ScrollBar is ever styled,
            // then we would have to go find the ScrollBar and get its width. But that opens up another whole
            // can: we would want to cache it so we don't traverse the tree everytime, but then we need to
            // add handling for if/when the styling changes.

            // Thus I have decided that being 10 pixels off is not the end of the world, and if anyone is
            // bothered they can hit the FitToScreen button again.

            // Set viewport location
            double offsetLeft = (ViewportSize.Width - newView.Size.Width) / 2;
            double offsetTop = (ViewportSize.Height - newView.Size.Height) / 2;
            Point topLeft = new Point(newView.TopLeft.X - offsetLeft, newView.TopLeft.Y - offsetTop);
            ViewportLocation = topLeft;
        }

        /// <summary>
        /// Change the viewport location and zoom to fit as many items as possible on screen,
        /// using a default margin of 0.
        /// </summary>
        public void FitToScreen() => FitViewToChildren(0, 0);
    }
}

