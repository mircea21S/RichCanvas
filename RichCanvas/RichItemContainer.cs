using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using RichCanvas.Helpers;
using RichCanvas.States.ContainerStates;

namespace RichCanvas
{
    /// <summary>
    /// Delegate used to notify when an <see cref="RichItemContainer"/> is dragged.
    /// </summary>
    /// <param name="newLocation">The new location.</param>
    public delegate void PreviewLocationChanged(Point newLocation);

    /// <summary>
    /// <see cref="RichItemsControl"/> items container.
    /// </summary>
    [TemplatePart(Name = ContentPresenterName, Type = typeof(ContentPresenter))]
    public class RichItemContainer : ContentControl
    {
        private const string ContentPresenterName = "PART_ContentPresenter";
        private Stack<ContainerState> _states;

        public const double DefaultWidth = 1d;
        public const double DefaultHeight = 1d;
        internal ScaleTransform? ScaleTransform => RenderTransform is TransformGroup group ? group.Children.OfType<ScaleTransform>().FirstOrDefault() : null;
        internal TranslateTransform? TranslateTransform => RenderTransform is TransformGroup group ? group.Children.OfType<TranslateTransform>().FirstOrDefault() : null;

        #region Properties API

        /// <inheritdoc/>
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
        /// Gets or sets whether this <see cref="RichItemContainer"/> has custom behavior handled out of dragging
        /// This tells <see cref="RichItemsControl"/> to stop handling mouse interaction when manipulating this <see cref="RichItemContainer"/>
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
        /// Gets or sets this <see cref="RichItemContainer"/> ScaleTransform in order to get direction.
        /// </summary>
        public Point Scale
        {
            get => (Point)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public static DependencyProperty AllowScaleChangeToUpdatePositionProperty = DependencyProperty.Register(nameof(AllowScaleChangeToUpdatePosition), typeof(bool), typeof(RichItemContainer), new FrameworkPropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether this <see cref="RichItemContainer"/> Left and Top can be updated while Drawing if the <see cref="Scale"/> is changed.
        /// </summary>
        public bool AllowScaleChangeToUpdatePosition
        {
            get => (bool)GetValue(AllowScaleChangeToUpdatePositionProperty);
            set => SetValue(AllowScaleChangeToUpdatePositionProperty, value);
        }

        /// <summary>
        /// Apply transforms on <see cref="RichItemContainer"/>
        /// </summary>
        public static DependencyProperty ApplyTransformProperty = DependencyProperty.RegisterAttached("ApplyTransform", typeof(Transform), typeof(RichItemContainer), new FrameworkPropertyMetadata(default(Transform), OnApplyTransformChanged));
        public static void SetApplyTransform(UIElement element, Transform value) => element.SetValue(ApplyTransformProperty, value);
        public static Transform GetApplyTransform(UIElement element) => (Transform)element.GetValue(ApplyTransformProperty);

        public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(RichItemContainer));
        /// <summary>
        /// Occurs when this <see cref="ItemContainer"/> is selected.
        /// </summary>
        public event RoutedEventHandler Selected
        {
            add => AddHandler(SelectedEvent, value);
            remove => RemoveHandler(SelectedEvent, value);
        }

        public static readonly RoutedEvent UnselectedEvent = Selector.UnselectedEvent.AddOwner(typeof(RichItemContainer));
        /// <summary>
        /// Occurs when this <see cref="ItemContainer"/> is unselected.
        /// </summary>
        public event RoutedEventHandler Unselected
        {
            add => AddHandler(UnselectedEvent, value);
            remove => RemoveHandler(UnselectedEvent, value);
        }

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

        public static readonly RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent(nameof(DragStarted), RoutingStrategy.Bubble, typeof(DragStartedEventHandler), typeof(RichItemContainer));
        /// <summary>
        /// Occurs when this <see cref="RichItemContainer"/> is the instigator of a drag operation.
        /// </summary>
        public event DragStartedEventHandler DragStarted
        {
            add => AddHandler(DragStartedEvent, value);
            remove => RemoveHandler(DragStartedEvent, value);
        }

        public static readonly RoutedEvent DragDeltaEvent = EventManager.RegisterRoutedEvent(nameof(DragDelta), RoutingStrategy.Bubble, typeof(DragDeltaEventHandler), typeof(RichItemContainer));
        /// <summary>
        /// Occurs when this <see cref="RichItemContainer"/> is being dragged.
        /// </summary>
        public event DragDeltaEventHandler DragDelta
        {
            add => AddHandler(DragDeltaEvent, value);
            remove => RemoveHandler(DragDeltaEvent, value);
        }

        public static readonly RoutedEvent DragCompletedEvent = EventManager.RegisterRoutedEvent(nameof(DragCompleted), RoutingStrategy.Bubble, typeof(DragCompletedEventHandler), typeof(RichItemContainer));
        /// <summary>
        /// Occurs when this <see cref="RichItemContainer"/> completed the drag operation.
        /// </summary>
        public event DragCompletedEventHandler DragCompleted
        {
            add => AddHandler(DragCompletedEvent, value);
            remove => RemoveHandler(DragCompletedEvent, value);
        }
        #endregion

        static RichItemContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichItemContainer), new FrameworkPropertyMetadata(typeof(RichItemContainer)));
        }

        /// <summary>
        /// Gets this <see cref="RichItemContainer"/> TransformBounds.
        /// </summary>
        public Rect BoundingBox { get; private set; }

        public ContainerState CurrentState => _states.Peek();

        private RichItemsControl? _host;
        /// <summary>
        /// The <see cref="RichItemsControl"/> that owns this <see cref="RichItemContainer"/>.
        /// </summary>
        public RichItemsControl Host => _host ??= (RichItemsControl)ItemsControl.ItemsControlFromItemContainer(this);

        internal bool TopPropertyInitalized { get; private set; }
        internal bool LeftPropertyInitialized { get; private set; }

        public RichItemContainer()
        {
            _states = new Stack<ContainerState>();
            _states.Push(GetDefaultState());
        }

        public void PushState(ContainerState state)
        {
            _states.Push(state);
            state.Enter();
        }

        public void PopState()
        {
            // Never remove the default state
            if (_states.Count > 1)
            {
                ContainerState prev = _states.Pop();
                prev.Exit();
                CurrentState.ReEnter();
            }
        }

        /// <summary>
        /// Calculates <see cref="RichItemContainer"/> bounding box based on applied transforms.
        /// </summary>
        public void CalculateBoundingBox()
        {
            GeneralTransform transform = TransformToVisual(Host.ItemsHost);
            if (double.IsNaN(Width) || double.IsNaN(Height))
            {
                Rect actualBounds = transform.TransformBounds(new Rect(0, 0, ActualWidth, ActualHeight));
                BoundingBox = actualBounds;
                return;
            }
            Rect bounds = transform.TransformBounds(new Rect(0, 0, Width, Height));
            BoundingBox = bounds;
        }

        protected virtual ContainerState GetDefaultState() => new ContainerDefaultState(this);

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Focus();
            if (Mouse.Captured == null || IsMouseCaptured)
            {
                CaptureMouse();
                CurrentState.HandleMouseDown(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                CurrentState.HandleMouseMove(e);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            // Release the mouse capture if all the mouse buttons are released
            if (IsMouseCaptured && e.RightButton == MouseButtonState.Released && e.LeftButton == MouseButtonState.Released && e.MiddleButton == MouseButtonState.Released)
            {
                CurrentState.HandleMouseUp(e);
                PopState();
                ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Occurs when the <see cref="RichItemContainer"/> is being dragged.
        /// </summary>
        public event PreviewLocationChanged? PreviewLocationChanged;

        /// <summary>
        /// Raises the <see cref="PreviewLocationChanged"/> event.
        /// </summary>
        /// <param name="location">The new location.</param>
        protected internal void OnPreviewLocationChanged(Point location)
        {
            PreviewLocationChanged?.Invoke(location);
        }

        internal bool IsValid()
        {
            return (Height != 0 || ActualHeight != 0) && (Width != 0 || ActualWidth != 0)
                && (!double.IsNaN(Height) || !double.IsNaN(ActualHeight)) && (!double.IsNaN(Width) || !double.IsNaN(ActualWidth));
        }

        internal void RaiseDragStartedEvent(Point position)
        {
            RaiseEvent(new DragStartedEventArgs(position.X, position.Y)
            {
                RoutedEvent = DragStartedEvent
            });

        }
        internal void RaiseDragDeltaEvent(Point position)
        {
            RaiseEvent(new DragDeltaEventArgs(position.X, position.Y)
            {
                RoutedEvent = DragDeltaEvent
            });
        }

        internal void RaiseDragCompletedEvent(Point position)
        {
            RaiseEvent(new DragCompletedEventArgs(position.X, position.Y, false)
            {
                RoutedEvent = DragCompletedEvent
            });
        }

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemContainer)d).OverrideScale((Point)e.NewValue);

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemContainer)d).UpdatePosition(e.Property);

        private static void OnApplyTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (VisualHelper.GetParentContainer(d)).ApplyTransform((Transform)e.NewValue);

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

        private void UpdatePosition(DependencyProperty prop)
        {
            if (prop.Name is nameof(Top) && !TopPropertyInitalized)
            {
                TopPropertyInitalized = true;
            }
            if (prop.Name is nameof(Left) && !LeftPropertyInitialized)
            {
                LeftPropertyInitialized = true;
            }
            RaiseEvent(new RoutedEventArgs(TopChangedEvent, Top));
            RaiseEvent(new RoutedEventArgs(LeftChangedEvent, Left));
            Host.ItemsHost.InvalidateArrange();
        }

        private void OnSelectedChanged(bool value)
        {
            // Raise event after the selection operation ended
            if (!Host.IsSelecting || Host.RealTimeSelectionEnabled)
            {
                // Add to base SelectedItems
                RaiseEvent(new RoutedEventArgs(value ? SelectedEvent : UnselectedEvent, this));
            }
        }

        private void ApplyTransform(Transform apply)
        {
            RenderTransform = apply.Clone();
            if (IsValid())
            {
                // Invalidate arrange to calculate correct BoundingBox
                Host.ItemsHost.InvalidateArrange();
            }
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
