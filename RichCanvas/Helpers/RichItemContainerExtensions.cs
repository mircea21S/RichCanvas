using System;
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
            var containerBottomValue = container.BoundingBox.Bottom + offset.Y;
            return offset.Y > 0 && container.IsSelected
                && IsLimitTouched(host.ScrollContainer.BottomLimit, containerBottomValue);
        }

        public static bool HasTouchedRightLimit(this RichItemContainer container, Point offset)
        {
            var host = container.Host;
            if (host.EnableNegativeScrolling)
            {
                return false;
            }

            var containerRightValue = container.BoundingBox.Right + offset.X;
            return offset.X > 0 && container.IsSelected
                && IsLimitTouched(host.ScrollContainer.RightLimit, containerRightValue);
        }

        public static bool HasTouchedBottomExtentSizeLimit(this RichItemContainer container, Point offset)
        {
            var host = container.Host;
            if (host.ExtentSize.IsEmpty)
            {
                return false;
            }

            var extentLimit = host.ScrollContainer.BottomLimit + host.ExtentSize.Height;
            var containerBottomValue = container.BoundingBox.Bottom + offset.Y;
            return offset.Y > 0 && container.IsSelected && IsLimitTouched(extentLimit, containerBottomValue);
        }

        public static bool HasTouchedTopExtentSizeLimit(this RichItemContainer container, Point offset)
        {
            var host = container.Host;
            if (host.ExtentSize.IsEmpty)
            {
                return false;
            }

            var extentLimit = Math.Abs(host.ScrollContainer.TopLimit - host.ExtentSize.Height);
            var containerTopValue = Math.Abs(container.BoundingBox.Top + offset.Y);
            return offset.Y < 0 && container.IsSelected && IsLimitTouched(extentLimit, containerTopValue);
        }

        public static bool HasTouchedLeftExtentSizeLimit(this RichItemContainer container, Point offset)
        {
            var host = container.Host;
            if (host.ExtentSize.IsEmpty)
            {
                return false;
            }

            var extentLimit = Math.Abs(host.ScrollContainer.LeftLimit - host.ExtentSize.Width);
            var containerLeftValue = Math.Abs(container.BoundingBox.Left + offset.X);
            return offset.X < 0 && container.IsSelected && IsLimitTouched(extentLimit, containerLeftValue);
        }

        public static bool HasTouchedRightExtentSizeLimit(this RichItemContainer container, Point offset)
        {
            var host = container.Host;
            if (host.ExtentSize.IsEmpty)
            {
                return false;
            }

            var extentLimit = host.ScrollContainer.RightLimit + host.ExtentSize.Width;
            var containerRightValue = container.BoundingBox.Right + offset.X;
            return offset.X > 0 && container.IsSelected && IsLimitTouched(extentLimit, containerRightValue);
        }

        private static bool IsLimitTouched(double limit, double value) => value >= limit;

    }
}
