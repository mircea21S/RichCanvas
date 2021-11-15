using RichCanvas.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace RichCanvas
{
    [TemplatePart(Name = ContentPresenterName, Type = typeof(ContentPresenter))]
    public class RichItemContainer : ContentControl
    {
        private const string ContentPresenterName = "PART_ContentPresenter";
        private RichItemsControl _host;

        internal ScaleTransform ScaleTransform => RenderTransform is TransformGroup group ? group.Children.OfType<ScaleTransform>().FirstOrDefault() : null;
        internal TranslateTransform TranslateTransform => RenderTransform is TransformGroup group ? group.Children.OfType<TranslateTransform>().FirstOrDefault() : null;

        public static DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(RichItemContainer), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsSelectedChanged));
        /// <summary>
        /// Gets or sets a value that indicates whether this item is selected.
        /// Can only be set if <see cref="IsSelectable"/> is true.
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static DependencyProperty TopProperty = DependencyProperty.Register(nameof(Top), typeof(double), typeof(RichItemContainer), new FrameworkPropertyMetadata(OnPositionChanged));
        /// <summary>
        /// Gets or sets the Top position of this <see cref="RichItemContainer"/> on <see cref="RichItemsControl.ItemsHost"/>
        /// </summary>
        public double Top
        {
            get => (double)GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }

        public static DependencyProperty LeftProperty = DependencyProperty.Register(nameof(Left), typeof(double), typeof(RichItemContainer), new FrameworkPropertyMetadata(OnPositionChanged));
        /// <summary>
        /// Gets or sets the Left position of this <see cref="RichItemContainer"/> on <see cref="RichItemsControl.ItemsHost"/>
        /// </summary>
        public double Left
        {
            get => (double)GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }

        public static DependencyProperty IsSelectableProperty = DependencyProperty.Register(nameof(IsSelectable), typeof(bool), typeof(RichItemContainer), new FrameworkPropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether this <see cref="RichItemContainer"/> can be selected.
        /// True by default
        /// </summary>
        public bool IsSelectable
        {
            get => (bool)GetValue(IsSelectableProperty);
            set => SetValue(IsSelectableProperty, value);
        }

        public static DependencyProperty IsDraggableProperty = DependencyProperty.Register(nameof(IsDraggable), typeof(bool), typeof(RichItemContainer), new FrameworkPropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether this <see cref="RichItemContainer"/> can be dragged on <see cref="RichItemsControl.ItemsHost"/>
        /// True by default
        /// </summary>
        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, value);
        }


        public static DependencyProperty HasCustomBehaviorProperty = DependencyProperty.Register(nameof(HasCustomBehavior), typeof(bool), typeof(RichItemContainer), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Gets or sets whether this <see cref="RichItemContainer"/> can be dragged on <see cref="RichItemsControl.ItemsHost"/>
        /// True by default
        /// </summary>
        public bool HasCustomBehavior
        {
            get => (bool)GetValue(HasCustomBehaviorProperty);
            set => SetValue(HasCustomBehaviorProperty, value);
        }

        public static DependencyProperty ShouldBringIntoViewProperty = DependencyProperty.Register(nameof(ShouldBringIntoView), typeof(bool), typeof(RichItemContainer), new FrameworkPropertyMetadata(false, OnBringIntoViewChanged));
        /// <summary>
        /// Gets or sets whether this <see cref="RichItemContainer"/> should be centered inside <see cref="RichItemsControl.ScrollContainer"/> viewport
        /// </summary>
        public bool ShouldBringIntoView
        {
            get => (bool)GetValue(ShouldBringIntoViewProperty);
            set => SetValue(ShouldBringIntoViewProperty, value);
        }

        public static DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(Point), typeof(RichItemContainer), new FrameworkPropertyMetadata(new Point(1, 1), OnScaleChanged));
        /// <summary>
        /// Gets this <see cref="RichItemContainer"/> ScaleTransform in order to get direction.
        /// </summary>
        public Point Scale
        {
            get => (Point)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public static DependencyProperty ApplyTransformProperty = DependencyProperty.RegisterAttached("ApplyTransform", typeof(Transform), typeof(RichItemContainer), new FrameworkPropertyMetadata(default(Transform), OnApplyTransformChanged));

        public static void SetApplyTransform(UIElement element, Transform value) => element.SetValue(ApplyTransformProperty, value);

        public static Transform GetApplyTransform(UIElement element) => (Transform)element.GetValue(ApplyTransformProperty);

        public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(RichItemContainer));
        public static readonly RoutedEvent UnselectedEvent = Selector.UnselectedEvent.AddOwner(typeof(RichItemContainer));

        public static readonly RoutedEvent TopChangedEvent = EventManager.RegisterRoutedEvent(nameof(TopChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RichItemContainer));
        /// <summary>
        /// Occurs whenever <see cref="RichItemContainer.Top"/> changes.
        /// </summary>
        public event RoutedEventHandler TopChanged
        {
            add { AddHandler(TopChangedEvent, value); }
            remove { RemoveHandler(TopChangedEvent, value); }
        }

        public static readonly RoutedEvent LeftChangedEvent = EventManager.RegisterRoutedEvent(nameof(LeftChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RichItemContainer));
        /// <summary>
        /// Occurs whenever <see cref="RichItemContainer.Left"/> changes.
        /// </summary>
        public event RoutedEventHandler LeftChanged
        {
            add { AddHandler(LeftChangedEvent, value); }
            remove { RemoveHandler(LeftChangedEvent, value); }
        }

        static RichItemContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichItemContainer), new FrameworkPropertyMetadata(typeof(RichItemContainer)));
        }

        /// <summary>
        /// The <see cref="RichItemsControl"/> that owns this <see cref="ItemContainer"/>.
        /// </summary>
        public RichItemsControl Host => _host ??= ItemsControl.ItemsControlFromItemContainer(this) as RichItemsControl;

        /// <summary>
        /// The Top position based on current <see cref="Scale"/>
        /// </summary>
        public double TransformedTop => ScaleTransform.ScaleY < 0 ? Top - Height : Top;

        /// <summary>
        /// The Left position based on current <see cref="Scale"/>
        /// </summary>
        public double TransformedLeft => ScaleTransform.ScaleX < 0 ? Left - Width : Left;

        internal bool IsDrawn { get; set; }

        internal bool TopPropertySet { get; private set; }

        internal bool LeftPropertySet { get; private set; }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            Host.HasCustomBehavior = HasCustomBehavior;
            if (IsDraggable)
            {
                DragBehavior.SetIsDragging((RichItemContainer)e.OriginalSource, true);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            Host.HasCustomBehavior = false;
            if (IsDraggable)
            {
                DragBehavior.SetIsDragging((RichItemContainer)e.OriginalSource, false);
            }
        }

        internal bool IsValid()
        {
            return Height != 0 && Width != 0 && !double.IsNaN(Height) && !double.IsNaN(Width);
        }

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemContainer)d).OverrideScale((Point)e.NewValue);

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemContainer)d).UpdatePosition(e.Property, (double)e.OldValue - (double)e.NewValue);

        private static void OnApplyTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemContainer)VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(d))).ApplyTransform((Transform)e.NewValue);

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var elem = (RichItemContainer)d;
            bool result = elem.IsSelectable && (bool)e.NewValue;
            elem.OnSelectedChanged(result);
            elem.IsSelected = result;
        }

        private static void OnBringIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ((RichItemContainer)d).BringIntoView();
            }
        }

        private void UpdatePosition(DependencyProperty prop, double delta)
        {
            if (prop.Name is nameof(Top))
            {
                TopPropertySet = true;
                RaiseEvent(new RoutedEventArgs(TopChangedEvent, delta));
            }
            if (prop.Name is nameof(Left))
            {
                LeftPropertySet = true;
                RaiseEvent(new RoutedEventArgs(LeftChangedEvent, delta));
            }
            Host?.ItemsHost.InvalidateMeasure();
        }

        private void OnSelectedChanged(bool value)
        {
            // Raise event after the selection operation ended
            if (!(Host?.IsSelecting ?? false) || (Host?.RealTimeSelectionEnabled ?? false))
            {
                // Add to base SelectedItems
                RaiseEvent(new RoutedEventArgs(value ? SelectedEvent : UnselectedEvent, this));
                if (value)
                {
                    Host.AddSelection(this);
                }
                else
                {
                    Host.RemoveSelection(this);
                }
            }
        }

        private void ApplyTransform(Transform apply)
        {
            RenderTransform = apply.Clone();
        }

        private void OverrideScale(Point value)
        {
            if (ScaleTransform != null)
            {
                ScaleTransform.ScaleX = value.X;
                ScaleTransform.ScaleY = value.Y;
            }
        }
    }
}
