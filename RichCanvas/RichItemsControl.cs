using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using RichCanvas.Automation;
using RichCanvas.CustomEventArgs;
using RichCanvas.Helpers;
using RichCanvas.States;

namespace RichCanvas
{
    /// <summary>
    /// ItemsControl hosting <see cref="RichCanvas"/>
    /// </summary>
    [TemplatePart(Name = DrawingPanelName, Type = typeof(Panel))]
    [TemplatePart(Name = SelectionRectangleName, Type = typeof(Rectangle))]
    [StyleTypedProperty(Property = nameof(SelectionRectangleStyle), StyleTargetType = typeof(Rectangle))]
    public partial class RichItemsControl : MultiSelector
    {
        #region Constants

        private const string DrawingPanelName = "PART_Panel";
        private const string SelectionRectangleName = "PART_SelectionRectangle";

        #endregion Constants

        #region Private Fields

        internal readonly ScaleTransform ScaleTransform = new ScaleTransform();
        internal readonly TranslateTransform TranslateTransform = new TranslateTransform();
        private RichCanvas _mainPanel;
        private DispatcherTimer _autoPanTimer;
        private Stack<CanvasState> _states;

        #endregion Private Fields

        #region Properties API

        public CanvasState CurrentState => _states.Peek();

        /// <summary>
        /// Gets or sets mouse position relative to <see cref="RichItemsControl.ItemsHost"/>.
        /// </summary>
        public static DependencyProperty MousePositionProperty = DependencyProperty.Register(nameof(MousePosition), typeof(Point), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(Point)));

        /// <summary>
        /// Gets or sets mouse position relative to <see cref="RichItemsControl.ItemsHost"/>.
        /// </summary>
        public Point MousePosition
        {
            get => (Point)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }

        /// <summary>
        /// Get only key of <see cref="SelectionRectangleProperty"/>.
        /// </summary>
        protected static readonly DependencyPropertyKey SelectionRectanglePropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectionRectangle), typeof(Rect), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(Rect)));

        /// <summary>
        /// Gets the selection area as <see cref="Rect"/>.
        /// </summary>
        public static readonly DependencyProperty SelectionRectangleProperty = SelectionRectanglePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the selection area as <see cref="Rect"/>.
        /// </summary>
        public Rect SelectionRectangle
        {
            get => (Rect)GetValue(SelectionRectangleProperty);
            internal set => SetValue(SelectionRectanglePropertyKey, value);
        }

        /// <summary>
        /// Get only key of <see cref="IsSelectingProperty"/>
        /// </summary>
        protected static readonly DependencyPropertyKey IsSelectingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsSelecting), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets whether the operation in progress is selection.
        /// </summary>
        public static readonly DependencyProperty IsSelectingProperty = IsSelectingPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether the operation in progress is selection.
        /// </summary>
        public bool IsSelecting
        {
            get => (bool)GetValue(IsSelectingProperty);
            internal set => SetValue(IsSelectingPropertyKey, value);
        }

        /// <summary>
        /// Get only key of <see cref="AppliedTransformProperty"/>.
        /// </summary>
        protected static readonly DependencyPropertyKey AppliedTransformPropertyKey = DependencyProperty.RegisterReadOnly(nameof(AppliedTransform), typeof(TransformGroup), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(TransformGroup)));

        /// <summary>
        /// Gets the transform that is applied to all child controls.
        /// </summary>
        public static DependencyProperty AppliedTransformProperty = AppliedTransformPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the transform that is applied to all child controls.
        /// </summary>
        public TransformGroup AppliedTransform
        {
            get => (TransformGroup)GetValue(AppliedTransformProperty);
            internal set => SetValue(AppliedTransformPropertyKey, value);
        }

        /// <summary>
        /// Gets or sets whether Auto-Panning is enabled.
        /// Default is enabled.
        /// </summary>
        public static DependencyProperty EnableAutoPanningProperty = DependencyProperty.Register(nameof(EnableAutoPanning), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false, OnEnableAutoPanningChanged));

        /// <summary>
        /// Gets or sets whether Auto-Panning is enabled.
        /// Default is disabled.
        /// </summary>
        public bool EnableAutoPanning
        {
            get => (bool)GetValue(EnableAutoPanningProperty);
            set => SetValue(EnableAutoPanningProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="DispatcherTimer"/> interval value.
        /// Default is 1.
        /// </summary>
        public static DependencyProperty AutoPanTickRateProperty = DependencyProperty.Register(nameof(AutoPanTickRate), typeof(float), typeof(RichItemsControl), new FrameworkPropertyMetadata(1f, OnAutoPanTickRateChanged));

        /// <summary>
        /// Gets or sets <see cref="DispatcherTimer"/> interval value.
        /// Default is 1.
        /// </summary>
        public float AutoPanTickRate
        {
            get => (float)GetValue(AutoPanTickRateProperty);
            set => SetValue(AutoPanTickRateProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="RichItemsControl.ItemsHost"/> translate speed.
        /// Default is 1.
        /// </summary>
        public static DependencyProperty AutoPanSpeedProperty = DependencyProperty.Register(nameof(AutoPanSpeed), typeof(float), typeof(RichItemsControl), new FrameworkPropertyMetadata(1f));

        /// <summary>
        /// Gets or sets the <see cref="RichItemsControl.ItemsHost"/> translate speed.
        /// Default is 1.
        /// </summary>
        public float AutoPanSpeed
        {
            get => (float)GetValue(AutoPanSpeedProperty);
            set => SetValue(AutoPanSpeedProperty, value);
        }

        /// <summary>
        /// Gets or sets grid drawing viewport size.
        /// Default is 10.
        /// </summary>
        public static DependencyProperty GridSpacingProperty = DependencyProperty.Register(nameof(GridSpacing), typeof(float), typeof(RichItemsControl), new FrameworkPropertyMetadata(20f));

        /// <summary>
        /// Gets or sets grid drawing viewport size.
        /// Default is 10.
        /// </summary>
        public float GridSpacing
        {
            get => (float)GetValue(GridSpacingProperty);
            set => SetValue(GridSpacingProperty, value);
        }

        public static DependencyProperty ViewportLocationProperty = DependencyProperty.Register(nameof(ViewportLocation), typeof(Point), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(Point), OnViewportLocationChanged));

        /// <summary>
        /// Gets current viewport rectangle.
        /// </summary>
        public Point ViewportLocation
        {
            get => (Point)GetValue(ViewportLocationProperty);
            set => SetValue(ViewportLocationProperty, value);
        }

        /// <summary>
        /// Gets or sets whether grid snap correction on <see cref="RichItemContainer"/> is applied.
        /// Default is disabled.
        /// </summary>
        public static DependencyProperty EnableSnappingProperty = DependencyProperty.Register(nameof(EnableSnapping), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether grid snap correction on <see cref="RichItemContainer"/> is applied.
        /// Default is disabled.
        /// </summary>
        public bool EnableSnapping
        {
            get => (bool)GetValue(EnableSnappingProperty);
            set => SetValue(EnableSnappingProperty, value);
        }

        /// <summary>
        /// Gets or sets selection <see cref="Rectangle"/> style.
        /// </summary>
        public static DependencyProperty SelectionRectangleStyleProperty = DependencyProperty.Register(nameof(SelectionRectangleStyle), typeof(Style), typeof(RichItemsControl));

        /// <summary>
        /// Gets or sets selection <see cref="Rectangle"/> style.
        /// </summary>
        public Style SelectionRectangleStyle
        {
            get => (Style)GetValue(SelectionRectangleStyleProperty);
            set => SetValue(SelectionRectangleStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the scrolling factor applied when scrolling.
        /// Default is 10.
        /// </summary>
        public static DependencyProperty ScrollFactorProperty = DependencyProperty.Register(nameof(ScrollFactor), typeof(double), typeof(RichItemsControl), new FrameworkPropertyMetadata(10d, null, CoerceScrollFactor));

        /// <summary>
        /// Gets or sets the scrolling factor applied when scrolling.
        /// Default is 10.
        /// </summary>
        public double ScrollFactor
        {
            get => (double)GetValue(ScrollFactorProperty);
            set => SetValue(ScrollFactorProperty, value);
        }

        /// <summary>
        /// Gets or sets the items in the <see cref="RichItemsControl"/> that are selected.
        /// </summary>
        public static DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(IList), OnSelectedItemsSourceChanged));

        /// <summary>
        /// Gets or sets the items in the <see cref="RichItemsControl"/> that are selected.
        /// </summary>
        public new IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// Occurs whenever <see cref="RichItemsControl.OnMouseLeftButtonUp(MouseButtonEventArgs)"/> is triggered and the drawing operation finished.
        /// </summary>
        public static readonly RoutedEvent DrawingEndedEvent = EventManager.RegisterRoutedEvent(nameof(DrawingEnded), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RichItemsControl));

        /// <summary>
        /// Occurs whenever <see cref="RichItemsControl.OnMouseLeftButtonUp(MouseButtonEventArgs)"/> is triggered and the drawing operation finished.
        /// </summary>
        public event RoutedEventHandler DrawingEnded
        {
            add { AddHandler(DrawingEndedEvent, value); }
            remove { RemoveHandler(DrawingEndedEvent, value); }
        }

        public static readonly DependencyProperty DrawingEndedCommandProperty = DependencyProperty.Register(nameof(DrawingEndedCommand), typeof(ICommand), typeof(RichItemsControl));

        /// <summary>
        /// Invoked when drawing opertation is completed. <br />
        /// Parameter is <see cref="Point"/>, representing the mouse position when drawing has finished.
        /// </summary>
        public ICommand? DrawingEndedCommand
        {
            get => (ICommand?)GetValue(DrawingEndedCommandProperty);
            set => SetValue(DrawingEndedCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets whether caching is disabled.
        /// Default is <see langword="true"/>.
        /// </summary>
        public static DependencyProperty DisableCacheProperty = DependencyProperty.Register(nameof(DisableCache), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(true, OnDisableCacheChanged));

        /// <summary>
        /// Gets or sets whether caching is disabled.
        /// Default is <see langword="true"/>.
        /// </summary>
        public bool DisableCache
        {
            get => (bool)GetValue(DisableCacheProperty);
            set => SetValue(DisableCacheProperty, value);
        }

        /// <summary>
        /// Get only key of <see cref="IsDraggingProperty"/>
        /// </summary>
        protected static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsDragging), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets whether the operation in progress is dragging.
        /// </summary>
        public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether the operation in progress is dragging.
        /// </summary>
        public bool IsDragging
        {
            get => (bool)GetValue(IsDraggingProperty);
            internal set => SetValue(IsDraggingPropertyKey, value);
        }

        /// <summary>
        /// Occurs whenever <see cref="RichItemsControl.ScaleTransform"/> changes.
        /// </summary>
        public static readonly RoutedEvent ZoomingEvent = EventManager.RegisterRoutedEvent(nameof(Zooming), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RichItemsControl));

        /// <summary>
        /// Occurs whenever <see cref="RichItemsControl.ScaleTransform"/> changes.
        /// </summary>
        public event RoutedEventHandler Zooming
        {
            add { AddHandler(ZoomingEvent, value); }
            remove { RemoveHandler(ZoomingEvent, value); }
        }

        /// <summary>
        /// Gets or sets whether real-time selection is enabled.
        /// Default is <see langword="false"/>.
        /// </summary>
        public static DependencyProperty RealTimeSelectionEnabledProperty = DependencyProperty.Register(nameof(RealTimeSelectionEnabled), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether real-time selection is enabled.
        /// Default is <see langword="false"/>.
        /// </summary>
        public bool RealTimeSelectionEnabled
        {
            get => (bool)GetValue(RealTimeSelectionEnabledProperty);
            set => SetValue(RealTimeSelectionEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets whether real-time selection is enabled.
        /// Default is <see langword="false"/>.
        /// </summary>
        public static DependencyProperty RealTimeDraggingEnabledProperty = DependencyProperty.Register(nameof(RealTimeDraggingEnabled), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether real-time selection is enabled.
        /// Default is <see langword="false"/>.
        /// </summary>
        public bool RealTimeDraggingEnabled
        {
            get => (bool)GetValue(RealTimeDraggingEnabledProperty);
            set => SetValue(RealTimeDraggingEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets whether you can select multiple elements or not.
        /// Default is <see langword="true"/>.
        /// </summary>
        public static DependencyProperty CanSelectMultipleItemsProperty = DependencyProperty.Register(nameof(CanSelectMultipleItems), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(true, OnCanSelectMultipleItemsChanged));

        /// <summary>
        /// Gets or sets whether you can select multiple elements or not.
        /// Default is <see langword="true"/>.
        /// </summary>
        public new bool CanSelectMultipleItems
        {
            get => (bool)GetValue(CanSelectMultipleItemsProperty);
            set => SetValue(CanSelectMultipleItemsProperty, value);
        }

        public static readonly DependencyProperty ViewportSizeProperty = DependencyProperty.Register(nameof(ViewportSize), typeof(Size), typeof(RichItemsControl), new FrameworkPropertyMetadata(Size.Empty));

        /// <summary>
        /// Gets the size of the viewport.
        /// </summary>
        public Size ViewportSize
        {
            get => (Size)GetValue(ViewportSizeProperty);
            set => SetValue(ViewportSizeProperty, value);
        }

        public static readonly DependencyProperty ItemsExtentProperty = DependencyProperty.Register(nameof(ItemsExtent), typeof(Rect), typeof(RichItemsControl), new FrameworkPropertyMetadata(Rect.Empty, OnItemsExtentChanged));

        /// <summary>
        /// The area covered by the <see cref="RichItemContainer"/>s.
        /// </summary>
        public Rect ItemsExtent
        {
            get => (Rect)GetValue(ItemsExtentProperty);
            set => SetValue(ItemsExtentProperty, value);
        }

        #endregion Properties API

        #region Internal Properties

        internal RichCanvas ItemsHost => _mainPanel;
        internal bool IsPanning { get; set; }
        internal bool IsZooming { get; set; }
        internal IList BaseSelectedItems => base.SelectedItems;
        internal List<int> CurrentDrawingIndexes { get; } = [];

        #endregion Internal Properties

        #region Constructors

        static RichItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichItemsControl), new FrameworkPropertyMetadata(typeof(RichItemsControl)));
            RichCanvasCommands.Register(typeof(RichItemsControl));
        }

        /// <summary>
        /// Creates a new instance of <see cref="RichItemsControl"/>
        /// </summary>
        public RichItemsControl()
        {
            AppliedTransform = new TransformGroup()
            {
                Children = new TransformCollection
                {
                    ScaleTransform, TranslateTransform
                }
            };

            _states = new Stack<CanvasState>();
            _states.Push(GetDefaultState());
        }

        #endregion Constructors

        #region Override Methods

        public virtual CanvasState GetDefaultState() => new DefaultState(this);

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            _mainPanel = (RichCanvas)GetTemplateChild(DrawingPanelName);
            _mainPanel.ItemsOwner = this;
            SetCachingMode(DisableCache);
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new RichItemsControlAutomationPeer(this);

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item) => item is RichItemContainer;

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride() => new RichItemContainer
        {
            RenderTransform = new TransformGroup
            {
                Children = new TransformCollection([new ScaleTransform(), new TranslateTransform()])
            }
        };

        /// <inheritdoc/>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if ((Mouse.Captured == null || IsMouseCaptured) && e.HasAnyButtonPressed())
            {
                if (CurrentState.MatchesPreviewMouseDownState(e, out CanvasState? matchingState))
                {
                    CaptureMouse();
                    PushState(matchingState!);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if ((Mouse.Captured == null || IsMouseCaptured) && e.HasAnyButtonPressed())
            {
                CaptureMouse();
                CurrentState.HandleMouseDown(e);
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            MousePosition = e.GetPosition(_mainPanel);
            if (IsMouseCaptured)
            {
                CurrentState.HandleMouseMove(e);
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                CurrentState.HandleMouseUp(e);
                PopState();
                if (e.HasAllButtonsReleased())
                {
                    ReleaseMouseCapture();
                }
            }
            Focus();
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            CurrentState.HandleKeyDown(e);
        }

        /// <inheritdoc/>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            CurrentState.HandleKeyUp(e);
            PopState();
        }

        /// <inheritdoc/>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                CurrentDrawingIndexes.Clear();
                if (CanSelectMultipleItems)
                {
                    base.SelectedItems.Clear();
                    SelectedItems.Clear();
                }
                else
                {
                    SelectedItem = null;
                }
            }
            else if (e.NewStartingIndex != -1 && e.Action == NotifyCollectionChangedAction.Add)
            {
                // a container is not able to be drawn if it has Width or Height already
                var container = (RichItemContainer)ItemContainerGenerator.ContainerFromIndex(e.NewStartingIndex);
                if (!container.IsValid())
                {
                    CurrentDrawingIndexes.Add(e.NewStartingIndex);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                CurrentDrawingIndexes.Remove(e.OldStartingIndex);
                for (int i = e.OldStartingIndex; i < CurrentDrawingIndexes.Count; i++)
                {
                    CurrentDrawingIndexes[i]--;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                int oldValue = CurrentDrawingIndexes[e.OldStartingIndex];
                CurrentDrawingIndexes.Remove(oldValue);
                CurrentDrawingIndexes.Insert(e.NewStartingIndex, oldValue);
            }
            // Replace event not implemented because the index doesn't change
        }

        /// <inheritdoc />
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            ViewportSize = new Size(ActualWidth / ViewportZoom, ActualHeight / ViewportZoom);
            UpdateScrollbars();
        }

        #endregion Override Methods

        #region Public Api

        public void PushState(CanvasState state)
        {
            _states.Push(state);
            state.Enter();
        }

        public void PopState()
        {
            // Never remove the default state
            if (_states.Count > 1)
            {
                CanvasState prev = _states.Pop();
                prev.Exit();
                CurrentState.ReEnter();
            }
        }

        #endregion Public Api

        #region Properties Callbacks

        private static void OnDisableCacheChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).SetCachingMode((bool)e.NewValue);

        private static void OnEnableAutoPanningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((RichItemsControl)d).OnEnableAutoPanningChanged((bool)e.NewValue);

        private static void OnAutoPanTickRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).UpdateTimerInterval();

        private static object CoerceScrollFactor(DependencyObject d, object value)
            => (double)value == 0 ? 10d : value;

        private static void OnCanSelectMultipleItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).CanSelectMultipleItemsUpdated((bool)e.NewValue);

        private static void OnItemsExtentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (RichItemsControl)d;
            editor.UpdateScrollbars();
        }

        #endregion Properties Callbacks

        #region Selection

        /// <inheritdoc/>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (CanSelectMultipleItems)
            {
                IList? selected = SelectedItems;
                if (selected != null)
                {
                    IList added = e.AddedItems;
                    for (int i = 0; i < added.Count; i++)
                    {
                        // Ensure no duplicates are added
                        if (!selected.Contains(added[i]))
                        {
                            selected.Add(added[i]);
                        }
                    }

                    IList removed = e.RemovedItems;
                    for (int i = 0; i < removed.Count; i++)
                    {
                        selected.Remove(removed[i]);
                    }
                }
            }
            else
            {
                if (e.AddedItems.Count == 1)
                {
                    SelectedItem = e.AddedItems[0];
                    SelectedItems.Add(e.AddedItems[0]);
                }
                else if (e.AddedItems.Count > 1)
                {
                    throw new ArgumentOutOfRangeException($"Cannot select more than 1 item when {nameof(CanSelectMultipleItems)} is set to false.");
                }
                if (e.RemovedItems.Count == 1 && SelectedItems.Count > 0 && e.RemovedItems[0] == SelectedItems[0])
                {
                    SelectedItems.Remove(e.RemovedItems[0]);
                }
            }
        }

        internal void BeginSelectionTransaction() => BeginUpdateSelectedItems();

        internal void EndSelectionTransaction() => EndUpdateSelectedItems();

        /// <summary>
        /// Returns the elements that intersect with <paramref name="area"/>
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public List<object> GetElementsInArea(Rect area)
        {
            var intersectedElements = new List<object>();
            var rectangleGeometry = new RectangleGeometry(area);
            VisualTreeHelper.HitTest(_mainPanel, null,
                new HitTestResultCallback((HitTestResult result) =>
                {
                    var geometryHitTestResult = (GeometryHitTestResult)result;
                    if (geometryHitTestResult.IntersectionDetail != IntersectionDetail.Empty)
                    {
                        RichItemContainer container = VisualHelper.GetParentContainer(geometryHitTestResult.VisualHit);
                        intersectedElements.Add(container.DataContext);
                    }
                    return HitTestResultBehavior.Continue;
                }),
                new GeometryHitTestParameters(rectangleGeometry));
            return intersectedElements;
        }

        private static void OnSelectedItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((RichItemsControl)d).OnSelectedItemsSourceChanged((IList)e.OldValue, (IList)e.NewValue);

        private void OnSelectedItemsSourceChanged(IList oldValue, IList newValue)
        {
            if (oldValue is INotifyCollectionChanged oc)
            {
                oc.CollectionChanged -= OnSelectedItemsChanged;
            }

            if (newValue is INotifyCollectionChanged nc)
            {
                nc.CollectionChanged += OnSelectedItemsChanged;
            }

            if (CanSelectMultipleItems)
            {
                IList selectedItems = base.SelectedItems;

                BeginUpdateSelectedItems();
                selectedItems.Clear();
                if (newValue != null)
                {
                    for (int i = 0; i < newValue.Count; i++)
                    {
                        selectedItems.Add(newValue[i]);
                    }
                }
                EndUpdateSelectedItems();
            }
        }

        private void OnSelectedItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    if (CanSelectMultipleItems)
                    {
                        base.SelectedItems.Clear();
                    }
                    break;

                case NotifyCollectionChangedAction.Add:
                    if (CanSelectMultipleItems)
                    {
                        IList? newItems = e.NewItems;
                        if (newItems != null)
                        {
                            IList selectedItems = base.SelectedItems;
                            for (int i = 0; i < newItems.Count; i++)
                            {
                                selectedItems.Add(newItems[i]);
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (CanSelectMultipleItems)
                    {
                        IList? oldItems = e.OldItems;
                        if (oldItems != null)
                        {
                            IList selectedItems = base.SelectedItems;
                            for (int i = 0; i < oldItems.Count; i++)
                            {
                                selectedItems.Remove(oldItems[i]);
                            }
                        }
                    }
                    break;
            }
        }

        public void UpdateSingleSelectedItem(RichItemContainer selectedContainer)
        {
            if (SelectedItem == null)
            {
                selectedContainer.IsSelected = true;
            }
            else
            {
                SelectedItem = null;
                selectedContainer.IsSelected = true;
            }
        }

        #endregion Selection

        #region Handlers And Private Methods

        private void CanSelectMultipleItemsUpdated(bool value)
        {
            base.CanSelectMultipleItems = value;
            if (value)
            {
                if (SelectedItem != null)
                {
                    SelectedItem = null;
                }
            }
            else
            {
                if (SelectedItems?.Count > 1)
                {
                    SelectedItems?.Clear();
                    SelectedItem = null;
                }
                else if (SelectedItems?.Count == 1)
                {
                    SelectedItem = SelectedItems?[0];
                }
            }
        }

        private static void OnViewportLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var host = (RichItemsControl)d;
            var translate = (Point)e.NewValue;

            host.TranslateTransform.X = -translate.X * host.ViewportZoom;
            host.TranslateTransform.Y = -translate.Y * host.ViewportZoom;

            host.UpdateScrollbars();
        }

        private void SetCachingMode(bool disable)
        {
            if (_mainPanel != null)
            {
                if (!disable)
                {
                    _mainPanel.CacheMode = new BitmapCache()
                    {
                        EnableClearType = false,
                        SnapsToDevicePixels = false,
                        RenderAtScale = ViewportZoom
                    };
                }
                else
                {
                    _mainPanel.CacheMode = null;
                }
            }
        }

        private void OnEnableAutoPanningChanged(bool enableAutoPanning)
        {
            if (enableAutoPanning)
            {
                if (_autoPanTimer == null)
                {
                    _autoPanTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(AutoPanTickRate), DispatcherPriority.Background, new EventHandler(HandleAutoPanning), Dispatcher);
                    _autoPanTimer.Start();
                }
                else
                {
                    _autoPanTimer.Interval = TimeSpan.FromMilliseconds(AutoPanTickRate);
                    _autoPanTimer.Start();
                }
            }
            else
            {
                _autoPanTimer?.Stop();
            }
        }

        private void HandleAutoPanning(object? sender, EventArgs e)
        {
            if (IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed && Mouse.Captured != null && !IsMouseCapturedByScrollBar() && !IsPanning)
            {
                Point mousePosition = Mouse.GetPosition(this);
                double x = ViewportLocation.X;
                double y = ViewportLocation.Y;

                if (mousePosition.Y <= 0)
                {
                    y -= AutoPanSpeed;
                }
                else if (mousePosition.Y >= ViewportHeight)
                {
                    y += AutoPanSpeed;
                }

                if (mousePosition.X <= 0)
                {
                    x -= AutoPanSpeed;
                }
                else if (mousePosition.X >= ViewportWidth)
                {
                    x += AutoPanSpeed;
                }

                ViewportLocation = new Point(x, y);
                MousePosition = Mouse.GetPosition(ItemsHost);

                CurrentState.HandleAutoPanning(new MouseEventArgs(Mouse.PrimaryDevice, 0));
            }
        }

        private static bool IsMouseCapturedByScrollBar()
        {
            return Mouse.Captured.GetType() == typeof(Thumb) || Mouse.Captured.GetType() == typeof(RepeatButton);
        }

        private void UpdateTimerInterval()
        {
            if (_autoPanTimer != null)
            {
                _autoPanTimer.Interval = TimeSpan.FromMilliseconds(AutoPanTickRate);
            }
        }

        internal void RaiseDrawEndedEvent(object context, Point mousePosition)
        {
            var newEventArgs = new RoutedEventArgs(DrawingEndedEvent, new DrawEndedEventArgs(context, mousePosition));
            RaiseEvent(newEventArgs);
        }

        #endregion Handlers And Private Methods
    }
}
