using RichCanvas.Helpers;
using RichCanvas.Gestures;
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

        private RichCanvas _mainPanel;
        private PanningGrid _canvasContainer;
        private bool _isDrawing;
        private readonly Gestures.Drawing _drawingGesture;
        private readonly Selecting _selectingGesture;
        private DispatcherTimer _autoPanTimer;
        private readonly List<int> _currentDrawingIndexes = new List<int>();

        #endregion

        #region Properties API

        public static DependencyProperty MousePositionProperty = DependencyProperty.Register(nameof(MousePosition), typeof(Point), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(Point)));
        /// <summary>
        /// Gets or sets mouse position relative to <see cref="RichItemsControl.ItemsHost"/>.
        /// </summary>
        public Point MousePosition
        {
            get => (Point)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }

        protected static readonly DependencyPropertyKey SelectionRectanglePropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectionRectangle), typeof(Rect), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(Rect)));
        public static readonly DependencyProperty SelectionRectangleProperty = SelectionRectanglePropertyKey.DependencyProperty;
        /// <summary>
        /// Gets the selection area as <see cref="Rect"/>.
        /// </summary>
        public Rect SelectionRectangle
        {
            get => (Rect)GetValue(SelectionRectangleProperty);
            internal set => SetValue(SelectionRectanglePropertyKey, value);
        }

        protected static readonly DependencyPropertyKey IsSelectingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsSelecting), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsSelectingProperty = IsSelectingPropertyKey.DependencyProperty;
        /// <summary>
        /// Gets whether the operation in progress is selection.
        /// </summary>
        public bool IsSelecting
        {
            get => (bool)GetValue(IsSelectingProperty);
            internal set => SetValue(IsSelectingPropertyKey, value);
        }

        protected static readonly DependencyPropertyKey AppliedTransformPropertyKey = DependencyProperty.RegisterReadOnly(nameof(AppliedTransform), typeof(TransformGroup), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(TransformGroup)));
        public static DependencyProperty AppliedTransformProperty = AppliedTransformPropertyKey.DependencyProperty;
        /// <summary>
        /// Gets the transform that is applied to all child controls.
        /// </summary>
        public TransformGroup AppliedTransform
        {
            get => (TransformGroup)GetValue(AppliedTransformProperty);
            internal set => SetValue(AppliedTransformPropertyKey, value);
        }

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
        public static readonly DependencyProperty ViewportRectProperty = ViewportRectPropertyKey.DependencyProperty;
        /// <summary>
        /// Gets current viewport rectangle.
        /// </summary>
        public Rect ViewportRect
        {
            get => (Rect)GetValue(ViewportRectProperty);
            internal set => SetValue(ViewportRectPropertyKey, value);
        }

        public static DependencyProperty TranslateOffsetProperty = DependencyProperty.Register(nameof(TranslateOffset), typeof(Point), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(Point), OnOffsetChanged));
        /// <summary>
        /// Gets or sets current <see cref="RichItemsControl.TranslateTransform"/>.
        /// </summary>
        public Point TranslateOffset
        {
            get => (Point)GetValue(TranslateOffsetProperty);
            set => SetValue(TranslateOffsetProperty, value);
        }

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

        public static DependencyProperty GridStyleProperty = DependencyProperty.Register(nameof(GridStyle), typeof(System.Windows.Media.Drawing), typeof(RichItemsControl));
        /// <summary>
        /// Gets or sets the background grid style.
        /// </summary>
        public System.Windows.Media.Drawing GridStyle
        {
            get => (System.Windows.Media.Drawing)GetValue(GridStyleProperty);
            set => SetValue(GridStyleProperty, value);
        }

        public static DependencyProperty SelectionRectangleStyleProperty = DependencyProperty.Register(nameof(SelectionRectangleStyle), typeof(Style), typeof(RichItemsControl));
        /// <summary>
        /// Gets or sets selection <see cref="Rectangle"/> style.
        /// </summary>
        public Style SelectionRectangleStyle
        {
            get => (Style)GetValue(SelectionRectangleStyleProperty);
            set => SetValue(SelectionRectangleStyleProperty, value);
        }

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

        public static DependencyProperty DisableScrollProperty = DependencyProperty.Register(nameof(DisableScroll), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Gets or sets whether scrolling operation is disabled.
        /// Default is enabled.
        /// </summary>
        public bool DisableScroll
        {
            get => (bool)GetValue(DisableScrollProperty);
            set => SetValue(DisableScrollProperty, value);
        }

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

        public static DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(IList)));
        /// <summary>
        /// Gets or sets the items in the <see cref="RichItemsControl"/> that are selected.
        /// </summary>
        public new IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public static readonly RoutedEvent DrawingEndedEvent = EventManager.RegisterRoutedEvent(nameof(DrawingEnded), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RichItemsControl));
        /// <summary>
        /// Occurs whenever <see cref="RichItemsControl.OnMouseLeftButtonUp(MouseButtonEventArgs)"/> is triggered and the drawing operation finished.
        /// </summary>
        public event RoutedEventHandler DrawingEnded
        {
            add { AddHandler(DrawingEndedEvent, value); }
            remove { RemoveHandler(DrawingEndedEvent, value); }
        }

        public static readonly RoutedEvent ScrollingEvent = EventManager.RegisterRoutedEvent(nameof(Scrolling), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RichItemsControl));
        /// <summary>
        /// Occurs whenever <see cref="RichItemsControl.TranslateTransform"/> changes.
        /// </summary>
        public event RoutedEventHandler Scrolling
        {
            add { AddHandler(ScrollingEvent, value); }
            remove { RemoveHandler(ScrollingEvent, value); }
        }

        public static DependencyProperty DisableCacheProperty = DependencyProperty.Register(nameof(DisableCache), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(true, OnDisableCacheChanged));
        /// <summary>
        /// Gets or sets whether caching is disabled.
        /// Default is true
        /// </summary>
        public bool DisableCache
        {
            get => (bool)GetValue(DisableCacheProperty);
            set => SetValue(DisableCacheProperty, value);
        }

        protected static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsDragging), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
        /// <summary>
        /// Gets whether the operation in progress is dragging.
        /// </summary>
        public bool IsDragging
        {
            get => (bool)GetValue(IsDraggingProperty);
            internal set => SetValue(IsDraggingPropertyKey, value);
        }

        public static DependencyProperty ZoomKeyProperty = DependencyProperty.Register(nameof(ZoomKey), typeof(Key), typeof(RichItemsControl), new FrameworkPropertyMetadata(Key.LeftCtrl));
        /// <summary>
        /// Gets or sets current key used to zoom.
        /// Default is LeftCtrl
        /// </summary>
        public Key ZoomKey
        {
            get => (Key)GetValue(ZoomKeyProperty);
            set => SetValue(ZoomKeyProperty, value);
        }

        public static DependencyProperty PanningKeyProperty = DependencyProperty.Register(nameof(PanningKey), typeof(Key), typeof(RichItemsControl), new FrameworkPropertyMetadata(Key.Space));
        /// <summary>
        /// Gets or sets current key used for panning.
        /// Default is Space
        /// </summary>
        public Key PanningKey
        {
            get => (Key)GetValue(PanningKeyProperty);
            set => SetValue(PanningKeyProperty, value);
        }

        public static readonly RoutedEvent ZoomingEvent = EventManager.RegisterRoutedEvent(nameof(Zooming), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RichItemsControl));
        /// <summary>
        /// Occurs whenever <see cref="RichItemsControl.ScaleTransform"/> changes.
        /// </summary>
        public event RoutedEventHandler Zooming
        {
            add { AddHandler(ZoomingEvent, value); }
            remove { RemoveHandler(ZoomingEvent, value); }
        }

        public static DependencyProperty RealTimeSelectionEnabledProperty = DependencyProperty.Register(nameof(RealTimeSelectionEnabled), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Gets or sets whether real-time selection is enabled.
        /// Default is false
        /// </summary>
        public bool RealTimeSelectionEnabled
        {
            get => (bool)GetValue(RealTimeSelectionEnabledProperty);
            set => SetValue(RealTimeSelectionEnabledProperty, value);
        }

        #endregion

        #region Internal Properties
        /// <summary>
        /// Gets whether at least one item is selected.
        /// </summary>
        public bool HasSelections => base.SelectedItems.Count > 1;
        internal RichCanvas ItemsHost => _mainPanel;
        internal PanningGrid ScrollContainer => _canvasContainer;
        internal TransformGroup SelectionRectangleTransform { get; private set; }
        internal double TopLimit { get; set; }
        internal double RightLimit { get; set; }
        internal double BottomLimit { get; set; }
        internal double LeftLimit { get; set; }
        internal bool IsPanning => Keyboard.IsKeyDown(PanningKey);
        internal bool IsZooming => Keyboard.IsKeyDown(ZoomKey);
        internal bool IsDrawing => _isDrawing;
        internal bool NeedMeasure { get; set; }
        internal RichItemContainer CurrentDrawingItem => _drawingGesture.CurrentItem;
        internal bool HasCustomBehavior { get; set; }
        #endregion

        #region Constructors

        static RichItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichItemsControl), new FrameworkPropertyMetadata(typeof(RichItemsControl)));
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
            DragBehavior.ItemsControl = this;
            _selectingGesture = new Selecting(this);
            _drawingGesture = new Gestures.Drawing(this);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Selects all elements inside <see cref="SelectionRectangle"/>
        /// </summary>
        public void SelectBySelectionRectangle()
        {
            RectangleGeometry geom = GetSelectionRectangleCurrentGeometry();

            UnselectAll();
            SelectedItems.Clear();
            _selectingGesture.UnselectAll();

            BeginUpdateSelectedItems();

            VisualTreeHelper.HitTest(_mainPanel, null,
                new HitTestResultCallback(OnHitTestResultCallback),
                new GeometryHitTestParameters(geom));

            EndUpdateSelectedItems();
        }

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

        #endregion

        #region Override Methods

        public override void OnApplyTemplate()
        {
            var selectionRectangle = (Rectangle)GetTemplateChild(SelectionRectangleName);
            selectionRectangle.RenderTransform = new TransformGroup
            {
                Children = new TransformCollection
                {
                    new ScaleTransform()
                }
            };
            SelectionRectangleTransform = (TransformGroup)selectionRectangle.RenderTransform;

            _mainPanel = (RichCanvas)GetTemplateChild(DrawingPanelName);
            _mainPanel.ItemsOwner = this;
            SetCachingMode(DisableCache);

            _canvasContainer = (PanningGrid)GetTemplateChild(CanvasContainerName);
            _canvasContainer.Initalize(this);

            TranslateTransform.Changed += OnTranslateChanged;
            ScaleTransform.Changed += OnScaleChanged;
        }

        protected override bool IsItemItsOwnContainerOverride(object item) => item is RichItemContainer;

        protected override DependencyObject GetContainerForItemOverride() => new RichItemContainer
        {
            RenderTransform = new TransformGroup
            {
                Children = new TransformCollection(new Transform[] { new ScaleTransform(), new TranslateTransform() })
            }
        };

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsPanning)
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Point position = e.GetPosition(ItemsHost);
                if (!VisualHelper.HasScrollBarParent((DependencyObject)e.OriginalSource))
                {
                    if (_currentDrawingIndexes.Count > 0)
                    {
                        for (int i = 0; i < _currentDrawingIndexes.Count; i++)
                        {
                            var container = (RichItemContainer)ItemContainerGenerator.ContainerFromIndex(_currentDrawingIndexes[i]);
                            if (container != null)
                            {
                                if (container.IsValid())
                                {
                                    container.IsDrawn = true;

                                    _currentDrawingIndexes.Remove(_currentDrawingIndexes[i]);
                                }
                                else
                                {
                                    CaptureMouse();
                                    _isDrawing = true;
                                    _drawingGesture.OnMouseDown(container, position);
                                    _currentDrawingIndexes.Remove(_currentDrawingIndexes[i]);
                                    break;
                                }
                            }
                        }
                    }

                    if (!_isDrawing && !DragBehavior.IsDragging && !IsPanning && !HasCustomBehavior)
                    {
                        IsSelecting = true;
                        _selectingGesture.OnMouseDown(position);
                        CaptureMouse();
                    }
                }
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            MousePosition = new Point(e.GetPosition(_mainPanel).X, e.GetPosition(_mainPanel).Y);

            if (_isDrawing)
            {
                NeedMeasure = _drawingGesture.IsMeasureNeeded();
                _drawingGesture.OnMouseMove(MousePosition);
                if (DisableAutoPanning)
                {
                    TopLimit = Math.Min(ItemsHost.TopLimit, _drawingGesture.GetCurrentTop());
                    BottomLimit = Math.Max(ItemsHost.BottomLimit, _drawingGesture.GetCurrentBottom());
                    RightLimit = Math.Max(ItemsHost.RightLimit, _drawingGesture.GetCurrentRight());
                    LeftLimit = Math.Min(ItemsHost.LeftLimit, _drawingGesture.GetCurrentLeft());
                    AdjustScroll();
                }
            }
            else if (IsSelecting)
            {
                _selectingGesture.OnMouseMove(MousePosition);

                if (RealTimeSelectionEnabled)
                {
                    SelectBySelectionRectangle();
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_isDrawing)
            {
                _isDrawing = false;
                NeedMeasure = true;
                var drawnItem = _drawingGesture.OnMouseUp();

                RaiseDrawEndedEvent(drawnItem.DataContext);
                _drawingGesture.Dispose();

                ItemsHost.InvalidateMeasure();
            }
            else if (!DragBehavior.IsDragging && IsSelecting)
            {
                IsSelecting = false;

                if (!RealTimeSelectionEnabled)
                {
                    SelectBySelectionRectangle();

                    IList selected = SelectedItems;

                    if (selected != null)
                    {
                        IList added = base.SelectedItems;
                        for (var i = 0; i < added.Count; i++)
                        {
                            // Ensure no duplicates are added
                            if (!selected.Contains(added[i]))
                            {
                                selected.Add(added[i]);
                            }
                        }
                    }
                }

            }
            if (IsPanning)
            {
                Cursor = Cursors.Arrow;
            }
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
            Focus();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewStartingIndex != -1 && e.Action == NotifyCollectionChangedAction.Add)
            {
                _currentDrawingIndexes.Add(e.NewStartingIndex);
            }
        }

        #endregion

        #region Properties Callbacks

        private static void OnDisableCacheChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).SetCachingMode((bool)e.NewValue);

        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).OverrideTranslate((Point)e.NewValue);

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

        #endregion

        #region Internal Methods
        internal void AddSelection(RichItemContainer container)
        {
            _selectingGesture.AddSelection(container);
            base.SelectedItems.Add(container.DataContext);
        }

        internal void RemoveSelection(RichItemContainer container)
        {
            _selectingGesture.RemoveSelection(container);
            base.SelectedItems.Remove(container.DataContext);
        }

        internal void UpdateSelections(bool snap = false)
        {
            _selectingGesture.UpdateSelectionsPosition(snap);
            AdjustScroll();
        }

        internal void AdjustScroll()
        {
            ScrollContainer.AdjustScrollVertically();
            ScrollContainer.AdjustScrollHorizontally();
        }

        internal void RaiseScrollingEvent(object context)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ScrollingEvent, context);
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region Handlers And Private Methods

        private void OnScaleChanged(object sender, EventArgs e)
        {
            Scale = ScaleTransform.ScaleX;
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ZoomingEvent, new Point(ScaleTransform.ScaleX, ScaleTransform.ScaleY));
            RaiseEvent(newEventArgs);
        }

        private void OnTranslateChanged(object sender, EventArgs e)
        {
            TranslateOffset = new Point(TranslateTransform.X, TranslateTransform.Y);
            RaiseScrollingEvent(e);
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
            TranslateTransform.X = newValue.X;
            TranslateTransform.Y = newValue.Y;
        }

        private void OverrideScale(double newValue)
        {
            CoerceValue(ScaleProperty);
            ScaleTransform.ScaleX = newValue;
            ScaleTransform.ScaleY = newValue;
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

        private void HandleAutoPanning(object sender, EventArgs e)
        {
            if (IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed && Mouse.Captured != null && !IsMouseCapturedByScrollBar() && !IsPanning)
            {
                NeedMeasure = false;
                var mousePosition = Mouse.GetPosition(ScrollContainer);
                var transformedPosition = Mouse.GetPosition(ItemsHost);
                if (mousePosition.Y <= 0)
                {
                    if (_isDrawing)
                    {
                        TopLimit = Math.Min(ItemsHost.TopLimit, _drawingGesture.GetCurrentTop());
                        // top is not set yet so after drawing the bottom limit will become the intial top
                        BottomLimit = Math.Max(ItemsHost.BottomLimit, CurrentDrawingItem.Top);

                        CurrentDrawingItem.Height = Math.Abs(transformedPosition.Y - CurrentDrawingItem.Top);
                    }
                    ScrollContainer.PanVertically(-AutoPanSpeed);
                }
                else if (mousePosition.Y >= ScrollContainer.ViewportHeight)
                {
                    if (_isDrawing)
                    {
                        BottomLimit = Math.Max(ItemsHost.BottomLimit, _drawingGesture.GetCurrentBottom());
                        TopLimit = Math.Min(ItemsHost.TopLimit, _drawingGesture.GetCurrentTop());

                        CurrentDrawingItem.Height = Math.Abs(transformedPosition.Y - CurrentDrawingItem.Top);
                    }
                    ScrollContainer.PanVertically(AutoPanSpeed);
                }

                if (mousePosition.X <= 0)
                {
                    if (_isDrawing)
                    {
                        LeftLimit = Math.Min(ItemsHost.LeftLimit, _drawingGesture.GetCurrentLeft());
                        // left is not set yet so after drawing the right limit will become the intial left
                        RightLimit = Math.Max(ItemsHost.RightLimit, CurrentDrawingItem.Left);

                        CurrentDrawingItem.Width = Math.Abs(transformedPosition.X - CurrentDrawingItem.Left);
                    }
                    ScrollContainer.PanHorizontally(-AutoPanSpeed);
                }
                else if (mousePosition.X >= ScrollContainer.ViewportWidth)
                {
                    if (_isDrawing)
                    {
                        LeftLimit = Math.Min(ItemsHost.LeftLimit, _drawingGesture.GetCurrentLeft());
                        RightLimit = Math.Max(ItemsHost.RightLimit, _drawingGesture.GetCurrentRight());

                        CurrentDrawingItem.Width = Math.Abs(transformedPosition.X - CurrentDrawingItem.Left);
                    }
                    ScrollContainer.PanHorizontally(AutoPanSpeed);
                }

                if (_isDrawing)
                {
                    if (Items.Count > 1)
                    {
                        TopLimit = Math.Min(ItemsHost.TopLimit, _drawingGesture.GetCurrentTop());
                        BottomLimit = Math.Max(ItemsHost.BottomLimit, _drawingGesture.GetCurrentBottom());
                        RightLimit = Math.Max(ItemsHost.RightLimit, _drawingGesture.GetCurrentRight());
                        LeftLimit = Math.Min(ItemsHost.LeftLimit, _drawingGesture.GetCurrentLeft());
                        AdjustScroll();
                    }
                }
                if (IsSelecting)
                {
                    _selectingGesture.Update(transformedPosition);
                }
                // overwrites the true from panning (virtualization disabled on autopanning = selecting + drawing)
                NeedMeasure = false;
            }
        }

        private HitTestResultBehavior OnHitTestResultCallback(HitTestResult result)
        {
            var geometryHitTestResult = (GeometryHitTestResult)result;
            if (geometryHitTestResult.IntersectionDetail != IntersectionDetail.Empty)
            {
                var container = VisualHelper.GetParentContainer(geometryHitTestResult.VisualHit);
                if (container.IsSelectable)
                {
                    container.IsSelected = true;
                }
            }
            return HitTestResultBehavior.Continue;
        }

        private bool IsMouseCapturedByScrollBar()
        {
            return Mouse.Captured.GetType() == typeof(Thumb) || Mouse.Captured.GetType() == typeof(RepeatButton);
        }

        private RectangleGeometry GetSelectionRectangleCurrentGeometry()
        {
            var scaleTransform = (ScaleTransform)SelectionRectangleTransform.Children[0];
            var currentSelectionTop = scaleTransform.ScaleY < 0 ? SelectionRectangle.Top - SelectionRectangle.Height : SelectionRectangle.Top;
            var currentSelectionLeft = scaleTransform.ScaleX < 0 ? SelectionRectangle.Left - SelectionRectangle.Width : SelectionRectangle.Left;
            return new RectangleGeometry(new Rect(currentSelectionLeft, currentSelectionTop, SelectionRectangle.Width, SelectionRectangle.Height));
        }

        private void UpdateTimerInterval()
        {
            if (_autoPanTimer != null)
            {
                _autoPanTimer.Interval = TimeSpan.FromMilliseconds(AutoPanTickRate);
            }
        }
        private void RaiseDrawEndedEvent(object context)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(DrawingEndedEvent, context);
            RaiseEvent(newEventArgs);
        }

        #endregion

    }
}
