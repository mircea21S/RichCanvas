using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using RichCanvas.Gestures;

namespace RichCanvas
{
    public partial class RichItemsControl
    {
        #region Dependency Properties

        /// <summary>
        /// Gets or sets the factor used to change <see cref="ScaleTransform"/> on zoom.
        /// Default is 1.1d.
        /// </summary>
        public static DependencyProperty ScaleFactorProperty = DependencyProperty.Register(nameof(ScaleFactor), typeof(double), typeof(RichItemsControl), new FrameworkPropertyMetadata(1.1d));

        /// <summary>
        /// Gets or sets the factor used to change <see cref="ScaleTransform"/> on zoom.
        /// Default is 1.1d.
        /// </summary>
        public double ScaleFactor
        {
            get => (double)GetValue(ScaleFactorProperty);
            set => SetValue(ScaleFactorProperty, value);
        }

        /// <summary>
        /// Gets or sets whether zooming operation is disabled.
        /// Default is enabled.
        /// </summary>
        public static DependencyProperty DisableZoomProperty = DependencyProperty.Register(nameof(DisableZoom), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether zooming operation is disabled.
        /// Default is enabled.
        /// </summary>
        public bool DisableZoom
        {
            get => (bool)GetValue(DisableZoomProperty);
            set => SetValue(DisableZoomProperty, value);
        }

        /// <summary>
        /// Gets or sets maximum scale for <see cref="RichItemsControl.ScaleTransform"/>.
        /// Default is 2.
        /// </summary>
        public static DependencyProperty MaxScaleProperty = DependencyProperty.Register(nameof(MaxScale), typeof(double), typeof(RichItemsControl), new FrameworkPropertyMetadata(2d, OnMaxScaleChanged, CoerceMaxScale));

        /// <summary>
        /// Gets or sets maximum scale for <see cref="RichItemsControl.ScaleTransform"/>.
        /// Default is 2.
        /// </summary>
        public double MaxScale
        {
            get => (double)GetValue(MaxScaleProperty);
            set => SetValue(MaxScaleProperty, value);
        }

        private static object CoerceMaxScale(DependencyObject d, object value)
        {
            var zoom = (RichItemsControl)d;
            double min = zoom.MinScale;

            return (double)value < min ? 2d : value;
        }

        private static void OnMaxScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zoom = (RichItemsControl)d;
            zoom.CoerceValue(ViewportZoomProperty);
        }

        /// <summary>
        /// Gets or sets minimum scale for <see cref="RichItemsControl.ScaleTransform"/>.
        /// Default is 0.1d.
        /// </summary>
        public static DependencyProperty MinScaleProperty = DependencyProperty.Register(nameof(MinScale), typeof(double), typeof(RichItemsControl), new FrameworkPropertyMetadata(0.1d, OnMinimumScaleChanged, CoerceMinimumScale));

        /// <summary>
        /// Gets or sets minimum scale for <see cref="RichItemsControl.ScaleTransform"/>.
        /// Default is 0.1d.
        /// </summary>
        public double MinScale
        {
            get => (double)GetValue(MinScaleProperty);
            set => SetValue(MinScaleProperty, value);
        }

        private static void OnMinimumScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zoom = (RichItemsControl)d;
            zoom.CoerceValue(MaxScaleProperty);
            zoom.CoerceValue(ViewportZoomProperty);
        }

        private static object CoerceMinimumScale(DependencyObject d, object value)
            => (double)value > 0 ? value : 0.1d;

        /// <summary>
        /// Gets or sets the current <see cref="RichItemsControl.ScaleTransform"/> value.
        /// Default is 1.
        /// </summary>
        public static DependencyProperty ViewportZoomProperty = DependencyProperty.Register(nameof(ViewportZoom), typeof(double), typeof(RichItemsControl), new FrameworkPropertyMetadata(1d, OnViewportZoomChanged, CoerceViewportZoom));

        /// <summary>
        /// Gets or sets the current <see cref="RichItemsControl.ScaleTransform"/> value.
        /// Default is 1.
        /// </summary>
        public double ViewportZoom
        {
            get => (double)GetValue(ViewportZoomProperty);
            set => SetValue(ViewportZoomProperty, value);
        }

        private static void OnViewportZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).OverrideScale((double)e.NewValue);

        private static object CoerceViewportZoom(DependencyObject d, object value)
        {
            var itemsControl = (RichItemsControl)d;

            if (itemsControl.DisableZoom)
            {
                return itemsControl.ViewportZoom;
            }

            double num = (double)value;
            double minimum = itemsControl.MinScale;
            if (num < minimum)
            {
                return minimum;
            }

            double maximum = itemsControl.MaxScale;
            if (num > maximum)
            {
                return maximum;
            }

            return value;
        }

        #endregion Dependency Properties

        /// <inheritdoc/>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (RichCanvasGestures.ZoomModifierKey == Keyboard.Modifiers)
            {
                Point position = e.GetPosition(ItemsHost);
                IsZooming = true;
                double scaleFactor = e.Delta > 0 ? ScaleFactor : 1 / ScaleFactor;
                ZoomAtPosition(position, scaleFactor);
                IsZooming = false;
                // handle the event so it won't trigger scrolling
                e.Handled = true;
            }
        }

        public void ZoomAtPosition(Point mousePosition, double delta)
        {
            if (!DisableZoom)
            {
                Point previouslyTransformedMousePosition = AppliedTransform.Transform(mousePosition);

                double previousZoom = ViewportZoom;
                ViewportZoom *= delta;

                if (Math.Abs(previousZoom - ViewportZoom) > 0.001)
                {
                    Point transformedMousePositionAfterScaling = AppliedTransform.Transform(mousePosition);

                    Vector translationAdjustment = previouslyTransformedMousePosition - transformedMousePositionAfterScaling;
                    Point newTranslation = new Point(TranslateTransform.X, TranslateTransform.Y) + translationAdjustment;

                    Vector viewportLocation = (new Vector(0, 0) - (Vector)newTranslation) / ViewportZoom;

                    ViewportLocation = (Point)viewportLocation;
                }
            }
        }

        public void ZoomIn() => ZoomAtPosition(MousePosition, ScaleFactor);

        public void ZoomOut() => ZoomAtPosition(MousePosition, 1 / ScaleFactor);

        private void OverrideScale(double zoom)
        {
            ScaleTransform.ScaleX = zoom;
            ScaleTransform.ScaleY = zoom;
            var zoomEventArgs = new RoutedEventArgs(ZoomingEvent, new Point(ScaleTransform.ScaleX, ScaleTransform.ScaleY));
            RaiseEvent(zoomEventArgs);

            ViewportSize = new Size(ActualWidth / ViewportZoom, ActualHeight / ViewportZoom);

            UpdateScrollbars();
            // apply any caching optimizations
        }
    }
}
