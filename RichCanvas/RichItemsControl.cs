using RichCanvas.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using RichCanvas.States;
using RichCanvas.States.Dragging;
using RichCanvas.States.SelectionStates;
using RichCanvas.Gestures;
using RichCanvas.CustomEventArgs;

namespace RichCanvas
{
    /// <summary>
    /// ItemsControl hosting <see cref="RichCanvas"/>
    /// </summary>
    [TemplatePart(Name = DrawingPanelName, Type = typeof(Panel))]
    [TemplatePart(Name = SelectionRectangleName, Type = typeof(Rectangle))]
    [StyleTypedProperty(Property = nameof(SelectionRectangleStyle), StyleTargetType = typeof(Rectangle))]
    public class RichItemsControl : MultiSelector
    {
        #region Constants

        private const string DrawingPanelName = "PART_Panel";
        private const string SelectionRectangleName = "PART_SelectionRectangle";
        private const string CanvasContainerName = "CanvasContainer";

        #endregion

        #region Private Fields

        internal readonly ScaleTransform ScaleTransform = new ScaleTransform();
        internal readonly TranslateTransform TranslateTransform = new TranslateTransform();
        private RichCanvas? _mainPanel;
        private PanningGrid? _canvasContainer;
        private DispatcherTimer? _autoPanTimer;
        private bool _fromEvent;

        #endregion

        #region Properties API

        public CanvasState? CurrentState { get; private set; }

        public RichItemContainer? SelectedContainer { get; private set; }

        /// <summary>
        /// <see cref="Grid"/> control wrapping the scrolling logic.
        /// </summary>
        public PanningGrid? ScrollContainer => _canvasContainer;

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
        /// Gets or sets whether Auto-Panning is disabled.
        /// Default is enabled.
        /// </summary>
        public static DependencyProperty DisableAutoPanningProperty = DependencyProperty.Register(nameof(DisableAutoPanning), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(true, OnDisableAutoPanningChanged));
        /// <summary>
        /// Gets or sets whether Auto-Panning is disabled.
        /// Default is enabled.
        /// </summary>
        public bool DisableAutoPanning
        {
            get => (bool)GetValue(DisableAutoPanningProperty);
            set => SetValue(DisableAutoPanningProperty, value);
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
        /// Gets or sets whether Grid Drawing is enabled on <see cref="RichItemsControl.ItemsHost"/> background.
        /// Default is disabled.
        /// </summary>
        public static DependencyProperty EnableGridProperty = DependencyProperty.Register(nameof(EnableGrid), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Gets or sets whether Grid Drawing is enabled on <see cref="RichItemsControl.ItemsHost"/> background.
        /// Default is disabled.
        /// </summary>
        public bool EnableGrid
        {
            get => (bool)GetValue(EnableGridProperty);
            set => SetValue(EnableGridProperty, value);
        }

        /// <summary>
        /// Gets or sets grid drawing viewport size.
        /// Default is 10.
        /// </summary>
        public static DependencyProperty GridSpacingProperty = DependencyProperty.Register(nameof(GridSpacing), typeof(float), typeof(RichItemsControl), new FrameworkPropertyMetadata(10f));
        /// <summary>
        /// Gets or sets grid drawing viewport size.
        /// Default is 10.
        /// </summary>
        public float GridSpacing
        {
            get => (float)GetValue(GridSpacingProperty);
            set => SetValue(GridSpacingProperty, value);
        }

        internal static readonly DependencyPropertyKey ViewportRectPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ViewportRect), typeof(Rect), typeof(RichItemsControl), new FrameworkPropertyMetadata(Rect.Empty));
        /// <summary>
        /// Gets current viewport rectangle.
        /// </summary>
        public static readonly DependencyProperty ViewportRectProperty = ViewportRectPropertyKey.DependencyProperty;
        /// <summary>
        /// Gets current viewport rectangle.
        /// </summary>
        public Rect ViewportRect
        {
            get => (Rect)GetValue(ViewportRectProperty);
            internal set => SetValue(ViewportRectPropertyKey, value);
        }

        /// <summary>
        /// Gets or sets current <see cref="RichItemsControl.TranslateTransform"/>.
        /// </summary>
        public static DependencyProperty TranslateOffsetProperty = DependencyProperty.Register(nameof(TranslateOffset), typeof(Point), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(Point), OnOffsetChanged));
        /// <summary>
        /// Gets or sets current <see cref="RichItemsControl.TranslateTransform"/>.
        /// </summary>
        public Point TranslateOffset
        {
            get => (Point)GetValue(TranslateOffsetProperty);
            set => SetValue(TranslateOffsetProperty, value);
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
        /// Gets or sets the background grid style.
        /// </summary>
        public static DependencyProperty GridStyleProperty = DependencyProperty.Register(nameof(GridStyle), typeof(Drawing), typeof(RichItemsControl));
        /// <summary>
        /// Gets or sets the background grid style.
        /// </summary>
        public Drawing GridStyle
        {
            get => (Drawing)GetValue(GridStyleProperty);
            set => SetValue(GridStyleProperty, value);
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
        /// Gets or sets the factor used to change <see cref="RichItemsControl.ScaleTransform"/> on zoom.
        /// Default is 1.1d.
        /// </summary>
        public static DependencyProperty ScaleFactorProperty = DependencyProperty.Register(nameof(ScaleFactor), typeof(double), typeof(RichItemsControl), new FrameworkPropertyMetadata(1.1d, null, CoerceScaleFactor));
        /// <summary>
        /// Gets or sets the factor used to change <see cref="RichItemsControl.ScaleTransform"/> on zoom.
        /// Default is 1.1d.
        /// </summary>
        public double ScaleFactor
        {
            get => (double)GetValue(ScaleFactorProperty);
            set => SetValue(ScaleFactorProperty, value);
        }

        /// <summary>
        /// Gets or sets whether scrolling operation is disabled.
        /// Default is enabled.f
        /// </summary>
        public static DependencyProperty DisableScrollProperty = DependencyProperty.Register(nameof(DisableScroll), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false, OnDisableScrollChanged));
        /// <summary>
        /// Gets or sets whether scrolling operation is disabled.
        /// Default is enabled.f
        /// </summary>
        public bool DisableScroll
        {
            get => (bool)GetValue(DisableScrollProperty);
            set => SetValue(DisableScrollProperty, value);
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

        /// <summary>
        /// Gets or sets the current <see cref="RichItemsControl.ScaleTransform"/> value.
        /// Default is 1.
        /// </summary>
        public static DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(RichItemsControl), new FrameworkPropertyMetadata(1d, OnScaleChanged, ConstarainScaleToRange));
        /// <summary>
        /// Gets or sets the current <see cref="RichItemsControl.ScaleTransform"/> value.
        /// Default is 1.
        /// </summary>
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
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

        /// <summary>
        /// Occurs whenever <see cref="RichItemsControl.TranslateTransform"/> changes.
        /// </summary>
        public static readonly RoutedEvent ScrollingEvent = EventManager.RegisterRoutedEvent(nameof(Scrolling), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RichItemsControl));
        /// <summary>
        /// Occurs whenever <see cref="RichItemsControl.TranslateTransform"/> changes.
        /// </summary>
        public event RoutedEventHandler Scrolling
        {
            add { AddHandler(ScrollingEvent, value); }
            remove { RemoveHandler(ScrollingEvent, value); }
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
        /// Gets or sets scroll Extent maximum size. Controls maximum offset of scroll.
        /// Default is <see cref="Size.Empty"/>.
        /// </summary>
        public static DependencyProperty ExtentSizeProperty = DependencyProperty.Register(nameof(ExtentSize), typeof(Size), typeof(RichItemsControl), new FrameworkPropertyMetadata(Size.Empty));
        /// <summary>
        /// Gets or sets scroll Extent maximum size. Controls maximum offset of scroll.
        /// Default is <see cref="Size.Empty"/>.
        /// </summary>
        public Size ExtentSize
        {
            get => (Size)GetValue(ExtentSizeProperty);
            set => SetValue(ExtentSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets whether <see cref="RichCanvas"/> has negative scrolling and panning.
        /// Default is <see langword="true"/>.
        /// </summary>
        public static DependencyProperty EnableNegativeScrollingProperty = DependencyProperty.Register(nameof(EnableNegativeScrolling), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether <see cref="RichCanvas"/> has negative scrolling and panning.
        /// Default is <see langword="true"/>.
        /// </summary>
        public bool EnableNegativeScrolling
        {
            get => (bool)GetValue(EnableNegativeScrollingProperty);
            set => SetValue(EnableNegativeScrollingProperty, value);
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

        /// <summary>
        /// Gets or sets whether <see cref="RichCanvas"/> has selection enabled.
        /// Default is <see langword="true"/>.
        /// </summary>
        public static DependencyProperty SelectionEnabledProperty = DependencyProperty.Register(nameof(SelectionEnabled), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether <see cref="RichCanvas"/> has selection enabled.
        /// Default is <see langword="true"/>.
        /// </summary>
        public bool SelectionEnabled
        {
            get => (bool)GetValue(SelectionEnabledProperty);
            set => SetValue(SelectionEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets whether <see cref="PanningGrid.ScrollOwner"/> vertical scrollbar visibility.
        /// Default is <see cref="ScrollBarVisibility.Visible"/>.
        /// </summary>
        public static DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(RichItemsControl), new FrameworkPropertyMetadata(ScrollBarVisibility.Visible, OnVerticalScrollBarVisiblityChanged));
        /// <summary>
        /// Gets or sets whether <see cref="PanningGrid.ScrollOwner"/> vertical scrollbar visibility.
        /// Default is <see cref="ScrollBarVisibility.Visible"/>.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            set => SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets whether <see cref="PanningGrid.ScrollOwner"/> horizontal scrollbar visibility.
        /// Default is <see cref="ScrollBarVisibility.Visible"/>.
        /// </summary>
        public static DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyProperty.Register(nameof(HorizontalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(RichItemsControl), new FrameworkPropertyMetadata(ScrollBarVisibility.Visible, OnHorizontalScrollBarVisiblityChanged));
        /// <summary>
        /// Gets or sets whether <see cref="PanningGrid.ScrollOwner"/> horizontal scrollbar visibility.
        /// Default is <see cref="ScrollBarVisibility.Visible"/>.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            set => SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty PanGestureProperty = DependencyProperty.Register(nameof(PanGesture), typeof(InputGesture), typeof(RichItemsControl), new FrameworkPropertyMetadata(RichCanvasGestures.Pan, OnPanGestureChanged));
        public InputGesture PanGesture
        {
            get => (InputGesture)GetValue(PanGestureProperty);
            set => SetValue(PanGestureProperty, value);
        }
        #endregion

        #region Internal Properties
        internal RichCanvas? ItemsHost => _mainPanel;
        internal bool IsPanning { get; set; }
        internal bool IsZooming { get; set; }
        internal bool InitializedScrollBarVisiblity { get; private set; }
        internal IList BaseSelectedItems => base.SelectedItems;
        internal List<int> CurrentDrawingIndexes { get; } = new List<int>();
        #endregion

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
            AddHandler(RichItemContainer.DragStartedEvent, new DragStartedEventHandler(OnItemsDragStarted));
            AddHandler(RichItemContainer.DragDeltaEvent, new DragDeltaEventHandler(OnItemsDragDelta));
            AddHandler(RichItemContainer.DragCompletedEvent, new DragCompletedEventHandler(OnItemsDragCompleted));

            AppliedTransform = new TransformGroup()
            {
                Children = new TransformCollection
                {
                    ScaleTransform, TranslateTransform
                }
            };

            // call this to initialize Dragging and Selecting strategies
            CanSelectMultipleItemsUpdated(CanSelectMultipleItems);

            //TODO: move this to xml comment on Register method
            // order matters as you may have multiple states that can be executed simultanously so RichCanvas picks them in order
            // try to avoid that
            // also the order matters as you migh have custom gesture with custom key gesture and mouse gesture so
            // RichCanvas searches the new state on MouseDown event and any key down besides KeyModifiers is ignored when Matching the Gesture 
            StateManager.RegisterCanvasState<PanningState>(RichCanvasGestures.Pan);
            StateManager.RegisterCanvasState<DrawingState>(RichCanvasGestures.Drawing, () => CurrentDrawingIndexes.Count > 0);
            StateManager.RegisterCanvasState<SelectingState>(RichCanvasGestures.Select, () => SelectionEnabled);
        }

        #endregion

        #region Override Methods

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            _mainPanel = (RichCanvas)GetTemplateChild(DrawingPanelName);
            _mainPanel.ItemsOwner = this;
            SetCachingMode(DisableCache);

            _canvasContainer = (PanningGrid)GetTemplateChild(CanvasContainerName);
            _canvasContainer.Initialize(this);

            TranslateTransform.Changed += OnTranslateChanged;
            ScaleTransform.Changed += OnScaleChanged;
        }

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item) => item is RichItemContainer;

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride() => new RichItemContainer
        {
            RenderTransform = new TransformGroup
            {
                Children = new TransformCollection(new Transform[] { new ScaleTransform(), new TranslateTransform() })
            }
        };

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (RichCanvasGestures.Zoom == Keyboard.Modifiers)
            {
                var position = e.GetPosition(this);
                IsZooming = true;
                ZoomAtPosition(position, e.Delta, ScaleFactor);

                //TODO: test this maybe add UnitTests
                if (_mainPanel.HasTouchedExtentSizeLimit(position) || _mainPanel.HasTouchedNegativeLimit(position))
                {
                    ZoomAtPosition(position, -e.Delta, ScaleFactor);
                }

                IsZooming = false;
                // handle the event so it won't trigger scrolling
                e.Handled = true;
            }
        }

        /// <inheritdoc/>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            CurrentState = StateManager.GetMatchingCanvasState(e, this);
            CurrentState?.Enter();
        }

        /// <inheritdoc/>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (Mouse.Captured == null || IsMouseCaptured)
            {
                CaptureMouse();
                CurrentState?.HandleMouseDown(e);
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            MousePosition = e.GetPosition(_mainPanel);
            if (IsMouseCaptured)
            {
                CurrentState?.HandleMouseMove(e);
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                CurrentState?.HandleMouseUp(e);
                if (e.HasAllButtonsReleased())
                {
                    ReleaseMouseCapture();
                }
            }
            Focus();
        }

        /// <inheritdoc/>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            CurrentState?.Cancel();
        }

        /// <inheritdoc/>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewStartingIndex != -1 && e.Action == NotifyCollectionChangedAction.Add)
            {
                var container = (RichItemContainer)ItemContainerGenerator.ContainerFromIndex(e.NewStartingIndex);
                if (!container.IsValid())
                {
                    CurrentDrawingIndexes.Add(e.NewStartingIndex);
                }
                else
                {
                    container.IsDrawn = true;
                }
            }
        }

        #endregion

        #region Public Api

        public void ZoomAtPosition(Point mousePosition, double delta, double? factor)
        {
            var previousScaleX = ScaleTransform.ScaleX;
            var previousScaleY = ScaleTransform.ScaleY;
            var originX = (mousePosition.X - TranslateTransform.X) / ScaleTransform.ScaleX;
            var originY = (mousePosition.Y - TranslateTransform.Y) / ScaleTransform.ScaleY;

            if (delta > 0 && factor.HasValue)
            {
                var zoom = ScaleTransform.ScaleX * factor.Value;
                ScaleTransform.ScaleX = zoom;
                ScaleTransform.ScaleY = zoom;
            }
            else if (delta < 0 && factor.HasValue)
            {
                var zoom = ScaleTransform.ScaleX / factor.Value;
                ScaleTransform.ScaleX = zoom;
                ScaleTransform.ScaleY = zoom;
            }

            if (ScaleTransform.ScaleX <= MinScale)
            {
                ScaleTransform.ScaleX = MinScale;
            }
            if (ScaleTransform.ScaleY <= MinScale)
            {
                ScaleTransform.ScaleY = MinScale;
            }
            if (ScaleTransform.ScaleX >= MaxScale)
            {
                ScaleTransform.ScaleX = MaxScale;
            }
            if (ScaleTransform.ScaleY >= MaxScale)
            {
                ScaleTransform.ScaleY = MaxScale;
            }

            if (previousScaleX != ScaleTransform.ScaleX)
            {
                TranslateTransform.X = mousePosition.X - originX * ScaleTransform.ScaleX;
            }
            if (previousScaleY != ScaleTransform.ScaleY)
            {
                TranslateTransform.Y = mousePosition.Y - originY * ScaleTransform.ScaleY;
            }

            if (!DisableScroll)
            {
                ScrollContainer.SetCurrentScroll();
            }
        }

        public void ZoomIn()
        {
            var delta = Math.Pow(2.0, 120.0 / 3.0 / Mouse.MouseWheelDeltaForOneLine);
            ZoomAtPosition(MousePosition, delta, ScaleFactor);
        }

        public void ZoomOut()
        {
            var delta = Math.Pow(2.0, -120.0 / 3.0 / Mouse.MouseWheelDeltaForOneLine);
            ZoomAtPosition(MousePosition, delta, ScaleFactor);
        }

        #endregion

        #region Properties Callbacks

        private static void OnPanGestureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichCanvasGestures.Pan = (InputGesture)e.NewValue;
            StateManager.RegisterCanvasState<PanningState>(RichCanvasGestures.Pan);
        }

        private static void OnDisableCacheChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).SetCachingMode((bool)e.NewValue);

        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).OverrideTranslate((Point)e.NewValue);

        private static void OnDisableScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).OnDisableScrollChanged((bool)e.NewValue);

        private void OnDisableScrollChanged(bool disabled)
        {
            if (!disabled)
            {
                ScrollContainer?.SetCurrentScroll();
            }
            if (ScrollContainer != null && ScrollContainer.ScrollOwner != null)
            {
                var scrollBarVisibllity = disabled ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto;
                ScrollContainer.ScrollOwner.HorizontalScrollBarVisibility = scrollBarVisibllity;
                ScrollContainer.ScrollOwner.VerticalScrollBarVisibility = scrollBarVisibllity;
                ScrollContainer.ScrollOwner.InvalidateScrollInfo();
            }
        }

        private static object ConstarainScaleToRange(DependencyObject d, object value)
        {
            var itemsControl = (RichItemsControl)d;

            if (itemsControl.DisableZoom)
            {
                return itemsControl.Scale;
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
        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).OverrideScale((double)e.NewValue);

        private static object CoerceMinimumScale(DependencyObject d, object value)
            => (double)value > 0 ? value : 0.1;

        private static void OnMinimumScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zoom = (RichItemsControl)d;
            zoom.CoerceValue(MaxScaleProperty);
        }

        private static void OnMaxScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zoom = (RichItemsControl)d;
            zoom.CoerceValue(MinScaleProperty);
        }

        private static object CoerceMaxScale(DependencyObject d, object value)
        {
            var zoom = (RichItemsControl)d;
            var min = zoom.MinScale;

            return (double)value < min ? 2d : value;
        }

        private static void OnDisableAutoPanningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((RichItemsControl)d).OnDisableAutoPanningChanged((bool)e.NewValue);

        private static void OnAutoPanTickRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).UpdateTimerInterval();

        private static object CoerceScrollFactor(DependencyObject d, object value)
            => (double)value == 0 ? 10d : value;

        private static object CoerceScaleFactor(DependencyObject d, object value)
            => (double)value == 0 ? 1.1d : value;

        private static void OnCanSelectMultipleItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).CanSelectMultipleItemsUpdated((bool)e.NewValue);

        private static void OnVerticalScrollBarVisiblityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).OnScrollBarVisiblityChanged((ScrollBarVisibility)e.NewValue, true);

        private static void OnHorizontalScrollBarVisiblityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).OnScrollBarVisiblityChanged((ScrollBarVisibility)e.NewValue);

        #endregion

        //TODO: test all selection itemssource modifications
        #region Selection

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
                        var container = VisualHelper.GetParentContainer(geometryHitTestResult.VisualHit);
                        intersectedElements.Add(container.DataContext);
                    }
                    return HitTestResultBehavior.Continue;
                }),
                new GeometryHitTestParameters(rectangleGeometry));
            return intersectedElements;
        }

        /// <inheritdoc/>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (!IsSelecting && CanSelectMultipleItems)
            {
                IList selected = SelectedItems;

                if (selected != null)
                {
                    IList added = e.AddedItems;
                    IList removed = e.RemovedItems;
                    for (var i = 0; i < added.Count; i++)
                    {
                        // Ensure no duplicates are added
                        if (!selected.Contains(added[i]))
                        {
                            selected.Add(added[i]);
                        }
                    }

                    for (var i = 0; i < removed.Count; i++)
                    {
                        selected.Remove(removed[i]);
                    }
                }
            }
            else if (!IsSelecting && !CanSelectMultipleItems)
            {
                var added = e.AddedItems;
                if (added.Count == 1)
                {
                    if (SelectedContainer != null && added[0] != SelectedContainer.DataContext)
                    {
                        SelectedContainer.IsSelected = false;
                    }
                }
            }
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
                    for (var i = 0; i < newValue.Count; i++)
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
                            for (var i = 0; i < newItems.Count; i++)
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
                            for (var i = 0; i < oldItems.Count; i++)
                            {
                                selectedItems.Remove(oldItems[i]);
                            }
                        }
                    }
                    break;
            }
        }

        internal void UpdateSelectedItem(RichItemContainer container)
        {
            if (SelectedContainer != null)
            {
                SelectedContainer.IsSelected = false;
            }
            SelectedContainer = container;
        }

        #endregion

        #region Handlers And Private Methods

        private void OnItemsDragCompleted(object sender, DragCompletedEventArgs e)
        {
            SelectionHelper.GetDraggingStrategy()?.OnItemsDragCompleted(sender, e);
            IsDragging = false;
        }

        private void OnItemsDragDelta(object sender, DragDeltaEventArgs e)
        {
            SelectionHelper.GetDraggingStrategy()?.OnItemsDragDelta(sender, e);
        }

        private void OnItemsDragStarted(object sender, DragStartedEventArgs e)
        {
            IsDragging = true;
            SelectionHelper.GetDraggingStrategy()?.OnItemsDragStarted(sender, e);
        }

        private void CanSelectMultipleItemsUpdated(bool value)
        {
            //TODO: test this
            base.CanSelectMultipleItems = value;
            if (value)
            {
                SelectionHelper.SetSelectionStrategy(new MultipleSelectionStrategy(this));
                SelectionHelper.SetDraggingStrategy(new MultipleDraggingStrategy(this));
            }
            else
            {
                SelectionHelper.SetSelectionStrategy(new SingleSelectionStrategy(this));
                SelectionHelper.SetDraggingStrategy(new SingleDraggingStrategy(this));
            }
        }

        private void OnScaleChanged(object? sender, EventArgs e)
        {
            _fromEvent = true;
            Scale = ScaleTransform.ScaleX;
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ZoomingEvent, new Point(ScaleTransform.ScaleX, ScaleTransform.ScaleY));
            RaiseEvent(newEventArgs);
            _fromEvent = false;
        }

        private void OnTranslateChanged(object? sender, EventArgs e)
        {
            _fromEvent = true;
            TranslateOffset = new Point(TranslateTransform.X, TranslateTransform.Y);
            RaiseScrollingEvent(e);
            _fromEvent = false;
        }

        private void RaiseScrollingEvent(object context)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ScrollingEvent, context);
            RaiseEvent(newEventArgs);
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
                        RenderAtScale = Scale
                    };
                }
                else
                {
                    _mainPanel.CacheMode = null;
                }
            }
        }

        private void OverrideTranslate(Point newValue)
        {
            if (!_fromEvent)
            {
                TranslateTransform.X = newValue.X;
                TranslateTransform.Y = newValue.Y;
                ScrollContainer?.SetCurrentScroll();
            }
        }

        private void OverrideScale(double newValue)
        {
            if (!_fromEvent)
            {
                ScaleTransform.ScaleX = newValue;
                ScaleTransform.ScaleY = newValue;
                CoerceValue(ScaleProperty);
                ScrollContainer?.SetCurrentScroll();
            }
        }

        private void OnDisableAutoPanningChanged(bool shouldDisable)
        {
            if (!shouldDisable)
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
                if (_autoPanTimer != null)
                {
                    _autoPanTimer.Stop();
                }
            }
        }

        private void HandleAutoPanning(object? sender, EventArgs e)
        {
            if (IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed && Mouse.Captured != null && !IsMouseCapturedByScrollBar() && !IsPanning && ScrollContainer != null)
            {
                var mousePosition = Mouse.GetPosition(ScrollContainer);
                var transformedPosition = Mouse.GetPosition(ItemsHost);

                if (mousePosition.Y <= 0)
                {
                    if (CurrentState is DrawingState)
                    {
                        CurrentState?.HandleAutoPanning(transformedPosition, true);
                    }
                    ScrollContainer.PanVertically(-AutoPanSpeed);
                }
                else if (mousePosition.Y >= ScrollContainer.ViewportHeight)
                {
                    if (CurrentState is DrawingState)
                    {
                        CurrentState?.HandleAutoPanning(transformedPosition, true);
                    }
                    ScrollContainer.PanVertically(AutoPanSpeed);
                }

                if (mousePosition.X <= 0)
                {
                    if (CurrentState is DrawingState)
                    {
                        CurrentState?.HandleAutoPanning(transformedPosition);
                    }
                    ScrollContainer.PanHorizontally(-AutoPanSpeed);
                }
                else if (mousePosition.X >= ScrollContainer.ViewportWidth)
                {
                    if (CurrentState is DrawingState)
                    {
                        CurrentState?.HandleAutoPanning(transformedPosition);
                    }
                    ScrollContainer.PanHorizontally(AutoPanSpeed);
                }

                if (IsSelecting)
                {
                    CurrentState?.HandleAutoPanning(transformedPosition);
                }
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
            RoutedEventArgs newEventArgs = new RoutedEventArgs(DrawingEndedEvent, new DrawEndedEventArgs(context, mousePosition));
            RaiseEvent(newEventArgs);
        }

        internal void OnScrollBarVisiblityChanged(ScrollBarVisibility scrollBarVisibility, bool isVertical = false, bool initalized = false)
        {
            if (initalized)
            {
                InitializedScrollBarVisiblity = true;
            }

            if (ScrollContainer != null && ScrollContainer.ScrollOwner != null)
            {
                if (isVertical)
                {
                    ScrollContainer.ScrollOwner.VerticalScrollBarVisibility = scrollBarVisibility;
                }
                else
                {
                    ScrollContainer.ScrollOwner.HorizontalScrollBarVisibility = scrollBarVisibility;
                }
                ScrollContainer.ScrollOwner.InvalidateScrollInfo();
            }
        }
        #endregion
    }
}
