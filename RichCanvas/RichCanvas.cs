using System;
using System.Windows;
using System.Windows.Controls;

namespace RichCanvas
{
    /// <summary>
    /// ItemsHost of <see cref="RichItemsControl"/>
    /// </summary>
    public class RichCanvas : Panel
    {
        private RichItemsControl? _itemsOwner;
        internal RichItemsControl ItemsOwner
        {
            get => _itemsOwner ?? throw new InvalidOperationException("RichCanvas not initialized");
            set => _itemsOwner = value;
        }

        public static readonly DependencyProperty ExtentProperty = DependencyProperty.Register(nameof(Extent), typeof(Rect), typeof(RichCanvas), new FrameworkPropertyMetadata(Rect.Empty));
        /// <summary>The area covered by the children of this panel.</summary>
        public Rect Extent
        {
            get => (Rect)GetValue(ExtentProperty);
            set => SetValue(ExtentProperty, value);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size constraint)
        {
            if (ItemsOwner != null && (ItemsOwner.IsSelecting || ItemsOwner.IsDragging))
            {
                return default;
            }

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                var container = (RichItemContainer)InternalChildren[i];
                container.Measure(constraint);
            }

            return default;
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];
                if (child is RichItemContainer container)
                {
                    child.Arrange(new Rect(new Point(container.Left, container.Top), child.DesiredSize));

                    if (container.IsValid())
                    {
                        container.CalculateBoundingBox();

                        minX = Math.Min(minX, container.BoundingBox.Left);
                        minY = Math.Min(minY, container.BoundingBox.Top);
                        maxX = Math.Max(maxX, container.BoundingBox.Right);
                        maxY = Math.Max(maxY, container.BoundingBox.Bottom);
                    }
                }
            }
            Extent = minX == double.MaxValue
                ? Rect.Empty
                : new Rect(minX, minY, maxX - minX, maxY - minY);

            return arrangeSize;
        }
    }
}
