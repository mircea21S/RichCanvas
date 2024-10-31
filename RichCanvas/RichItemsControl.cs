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
using RichCanvas.Gestures;
using RichCanvas.CustomEventArgs;
using System.Windows.Automation.Peers;
using RichCanvas.Automation;

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

        #endregion

        #region Private Fields

        internal readonly ScaleTransform ScaleTransform = new ScaleTransform();
        internal readonly TranslateTransform TranslateTransform = new TranslateTransform();
        private RichCanvas? _mainPanel;
        private DispatcherTimer? _autoPanTimer;
        private bool _fromEvent;
        private Stack<CanvasState> _states;

        #endregion

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

        public static readonly DependencyProperty ViewportSizeProperty = DependencyProperty.Register(nameof(ViewportSize), typeof(Size), typeof(RichItemsControl), new FrameworkPropertyMetadata(Size.Empty));
        /// <summary>
        /// Gets the size of the viewport.
        /// </summary>
        public Size ViewportSize
        {
            get => (Size)GetValue(ViewportSizeProperty);
            set => SetValue(ViewportSizeProperty, value);
        }
        #endregion

        #region Internal Properties

        internal RichCanvas? ItemsHost => _mainPanel;
        internal bool IsPanning { get; set; }
        internal bool IsZooming { get; set; }
        internal bool InitializedScrollBarVisiblity { get; private set; }
        internal IList BaseSelectedItems => base.SelectedItems;
        internal List<int> CurrentDrawingIndexes { get; } = [];

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
        #endregion

        #region Override Methods

        public virtual CanvasState GetDefaultState() => new DefaultState(this);

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            _mainPanel = (RichCanvas)GetTemplateChild(DrawingPanelName);
            _mainPanel.ItemsOwner = this;
            SetCachingMode(DisableCache);

            ScaleTransform.Changed += OnScaleChanged;
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
                    base.SelectedItems?.Clear();
                    SelectedItems?.Clear();
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
                var oldValue = CurrentDrawingIndexes[e.OldStartingIndex];
                CurrentDrawingIndexes.Remove(oldValue);
                CurrentDrawingIndexes.Insert(e.NewStartingIndex, oldValue);
            }
            // Replace event not implemented because the index doesn't change
        }

        /// <inheritdoc />
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            ViewportSize = new Size(ActualWidth / Scale, ActualHeight / Scale);

            UpdateScrollbars();
        }

        #endregion

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

            //if (!DisableScroll)
            //{
            //    SetCurrentScroll();
            //}
        }

        public void ZoomIn()
        {
            // delta isn't used as we have the ScaleFactor
            // it's used just to define the direction of zoom
            var delta = Math.Pow(2.0, 120.0 / 3.0 / Mouse.MouseWheelDeltaForOneLine);
            ZoomAtPosition(MousePosition, delta, ScaleFactor);
        }

        public void ZoomOut()
        {
            var delta = Math.Pow(2.0, 120.0 / 3.0 / Mouse.MouseWheelDeltaForOneLine);
            ZoomAtPosition(MousePosition, -delta, ScaleFactor);
        }

        #endregion

        #region Properties Callbacks

        private static void OnDisableCacheChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).SetCachingMode((bool)e.NewValue);

        private static void OnDisableScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).OnDisableScrollChanged((bool)e.NewValue);

        private void OnDisableScrollChanged(bool disabled)
        {
            //if (!disabled)
            //{
            //    SetCurrentScroll();
            //}
            if (ScrollOwner != null)
            {
                var scrollBarVisibllity = disabled ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto;
                ScrollOwner.HorizontalScrollBarVisibility = scrollBarVisibllity;
                ScrollOwner.VerticalScrollBarVisibility = scrollBarVisibllity;
                ScrollOwner.InvalidateScrollInfo();
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

        private static void OnEnableAutoPanningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((RichItemsControl)d).OnEnableAutoPanningChanged((bool)e.NewValue);

        private static void OnAutoPanTickRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).UpdateTimerInterval();

        private static object CoerceScrollFactor(DependencyObject d, object value)
            => (double)value == 0 ? 10d : value;

        private static object CoerceScaleFactor(DependencyObject d, object value)
            => (double)value == 0 ? 1.1d : value;

        private static void OnCanSelectMultipleItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).CanSelectMultipleItemsUpdated((bool)e.NewValue);

        #endregion

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
                    for (var i = 0; i < added.Count; i++)
                    {
                        // Ensure no duplicates are added
                        if (!selected.Contains(added[i]))
                        {
                            selected.Add(added[i]);
                        }
                    }

                    IList removed = e.RemovedItems;
                    for (var i = 0; i < removed.Count; i++)
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
                        var container = VisualHelper.GetParentContainer(geometryHitTestResult.VisualHit);
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

        #endregion

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

        private void OnScaleChanged(object? sender, EventArgs e)
        {
            _fromEvent = true;
            Scale = ScaleTransform.ScaleX;
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ZoomingEvent, new Point(ScaleTransform.ScaleX, ScaleTransform.ScaleY));
            RaiseEvent(newEventArgs);
            _fromEvent = false;
        }

        private static void OnViewportLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var host = (RichItemsControl)d;
            var translate = (Point)e.NewValue;

            host.TranslateTransform.X = -translate.X;
            host.TranslateTransform.Y = -translate.Y;
            host.UpdateScrollbars();
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

        private void OverrideScale(double newValue)
        {
            if (!_fromEvent)
            {
                ScaleTransform.ScaleX = newValue;
                ScaleTransform.ScaleY = newValue;
                CoerceValue(ScaleProperty);
                //SetCurrentScroll();
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
                var mousePosition = Mouse.GetPosition(this);
                var x = ViewportLocation.X;
                var y = ViewportLocation.Y;

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

                //todo: update scroll when autopan
                CurrentState?.HandleAutoPanning(new MouseEventArgs(Mouse.PrimaryDevice, 0));
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
        #endregion
    }
}
