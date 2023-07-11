using System.Windows;

namespace RichCanvas.Helpers
{
    public static class RichItemContainerExtensions
    {
        public static bool HasTouchedBottomLimit(this RichItemContainer container, Point offset)
        {
            var host = container.Host;
            if (host.EnableNegativeScrolling)
            {
                return false;
            }

            // stop dragging if the container inside the selection is the bottom or right limit
            if (offset.Y > 0 && container.IsSelected &&
                host.ScrollContainer.BottomLimit < container.BoundingBox.Bottom + offset.Y)
            {
                return true;
            }

            return false;
        }

        public static bool HasTouchedRightLimit(this RichItemContainer container, Point offset)
        {
            var host = container.Host;
            if (host.EnableNegativeScrolling)
            {
                return false;
            }

            if (offset.X > 0 && container.IsSelected &&
                host.ScrollContainer.RightLimit < container.BoundingBox.Right + offset.X)
            {
                return true;
            }

            return false;
        }

        public static bool HasTouchedBottomExtentSizeLimit(this RichItemContainer container, Point offset)
        {
            var host = container.Host;
            if (host.ExtentSize.IsEmpty)
            {
                return false;
            }

            if (offset.Y > 0 && container.IsSelected &&
                container.BoundingBox.Bottom + offset.Y > host.ScrollContainer.BottomLimit + host.ExtentSize.Height)
            {
                return true;
            }

            return false;
        }

        public static bool HasTouchedTopExtentSizeLimit(this RichItemContainer container, Point offset)
        {
            var host = container.Host;
            if (host.ExtentSize.IsEmpty)
            {
                return false;
            }

            if (offset.Y < 0 && container.IsSelected &&
                container.Top + offset.Y < host.ScrollContainer.TopLimit - host.ExtentSize.Height)
            {
                return true;
            }

            return false;
        }

        public static bool HasTouchedLeftExtentSizeLimit(this RichItemContainer container, Point offset)
        {
            var host = container.Host;
            if (host.ExtentSize.IsEmpty)
            {
                return false;
            }

            if (offset.X < 0 && container.IsSelected &&
                container.BoundingBox.Left + offset.X < host.ScrollContainer.LeftLimit - host.ExtentSize.Width)
            {
                return true;
            }

            return false;
        }

        public static bool HasTouchedRightExtentSizeLimit(this RichItemContainer container, Point offset)
        {
            var host = container.Host;
            if (host.ExtentSize.IsEmpty)
            {
                return false;
            }

            if (offset.X > 0 && container.IsSelected &&
               container.BoundingBox.Right + offset.X > host.ScrollContainer.RightLimit + host.ExtentSize.Width)
            {
                return true;
            }

            return false;
        }
    }
}
