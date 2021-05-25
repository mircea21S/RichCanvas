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
using System.Linq;

namespace RichCanvas
{
    [TemplatePart(Name = DrawingPanelName, Type = typeof(Panel))]
    [TemplatePart(Name = SelectionRectangleName, Type = typeof(Rectangle))]
    public class RichItemsControl : ItemsControl
    {
        private const string DrawingPanelName = "PART_Panel";
        private const string SelectionRectangleName = "PART_SelectionRectangle";
        private const string CanvasContainerName = "CanvasContainer";

        internal readonly ScaleTransform ScaleTransform = new ScaleTransform();
        internal readonly TranslateTransform TranslateTransform = new TranslateTransform();

        public TransformGroup SelectionRectanlgeTransform { get; private set; }

        private RichCanvas _mainPanel;
        private PanningGrid _canvasContainer;
        private bool _isDrawing;
        private Gestures.Drawing _drawingGesture;
        private Selecting _selectingGesture;
        private DispatcherTimer _autoPanTimer;
        private List<int> _currentDrawingIndexes = new List<int>();

        internal bool HasSelections => _selectingGesture.HasSelections;
        internal RichCanvas ItemsHost => _mainPanel;
        internal PanningGrid ScrollContainer => _canvasContainer;

        #region Properties API

        public static DependencyProperty MousePositionProperty = DependencyProperty.Register(nameof(MousePosition), typeof(Point), typeof(RichItemsControl));
        public Point MousePosition
        {
            get => (Point)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }

        public static DependencyProperty SelectionRectangleProperty = DependencyProperty.Register(nameof(SelectionRectangle), typeof(Rect), typeof(RichItemsControl));
        public Rect SelectionRectangle
        {
            get => (Rect)GetValue(SelectionRectangleProperty);
            set => SetValue(SelectionRectangleProperty, value);
        }


        public static DependencyProperty IsSelectingProperty = DependencyProperty.Register(nameof(IsSelecting), typeof(bool), typeof(RichItemsControl));
        public bool IsSelecting
        {
            get => (bool)GetValue(IsSelectingProperty);
            set => SetValue(IsSelectingProperty, value);
        }

        public static DependencyProperty AppliedTransformProperty = DependencyProperty.Register(nameof(AppliedTransform), typeof(TransformGroup), typeof(RichItemsControl));
        public TransformGroup AppliedTransform
        {
            get => (TransformGroup)GetValue(AppliedTransformProperty);
            set => SetValue(AppliedTransformProperty, value);
        }

        public static DependencyProperty DisableAutoPanningProperty = DependencyProperty.Register(nameof(DisableAutoPanning), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(true, OnDisableAutoPanningChanged));

        public bool DisableAutoPanning
        {
            get => (bool)GetValue(DisableAutoPanningProperty);
            set => SetValue(DisableAutoPanningProperty, value);
        }

        public static DependencyProperty AutoPanTickRateProperty = DependencyProperty.Register(nameof(AutoPanTickRate), typeof(float), typeof(RichItemsControl), new FrameworkPropertyMetadata(1f, OnAutoPanTickRateChanged));

        public float AutoPanTickRate
        {
            get => (float)GetValue(AutoPanTickRateProperty);
            set => SetValue(AutoPanTickRateProperty, value);
        }

        public static DependencyProperty AutoPanSpeedProperty = DependencyProperty.Register(nameof(AutoPanSpeed), typeof(float), typeof(RichItemsControl), new FrameworkPropertyMetadata(1f));

        public float AutoPanSpeed
        {
            get => (float)GetValue(AutoPanSpeedProperty);
            set => SetValue(AutoPanSpeedProperty, value);
        }

        public static DependencyProperty EnableVirtualizationProperty = DependencyProperty.Register(nameof(EnableVirtualization), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(true, OnEnableVirtualizationChanged));

        private static void OnEnableVirtualizationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).ItemsHost?.InvalidateMeasure();

        public bool EnableVirtualization
        {
            get => (bool)GetValue(EnableVirtualizationProperty);
            set => SetValue(EnableVirtualizationProperty, value);
        }

        public static DependencyProperty EnableGridProperty = DependencyProperty.Register(nameof(EnableGrid), typeof(bool), typeof(RichItemsControl), new FrameworkPropertyMetadata(false));

        public bool EnableGrid
        {
            get => (bool)GetValue(EnableGridProperty);
            set => SetValue(EnableGridProperty, value);
        }

        public static DependencyProperty GridSpacingProperty = DependencyProperty.Register(nameof(GridSpacing), typeof(float), typeof(RichItemsControl), new FrameworkPropertyMetadata(10f));

        public float GridSpacing
        {
            get => (float)GetValue(GridSpacingProperty);
            set => SetValue(GridSpacingProperty, value);
        }

        internal static readonly DependencyPropertyKey ViewportRectPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ViewportRect), typeof(Rect), typeof(RichItemsControl), new FrameworkPropertyMetadata(Rect.Empty));
        public static readonly DependencyProperty ViewportRectProperty = ViewportRectPropertyKey.DependencyProperty;

        public Rect ViewportRect
        {
            get => (Rect)GetValue(ViewportRectProperty);
        }

        //readonly todo
        public static DependencyProperty VisibleElementsProperty = DependencyProperty.Register(nameof(VisibleElementsCount), typeof(int), typeof(RichItemsControl));

        public int VisibleElementsCount
        {
            get => (int)GetValue(VisibleElementsProperty);
            set => SetValue(VisibleElementsProperty, value);
        }

        public static readonly DependencyProperty HighlightTemplateProperty = DependencyProperty.Register(nameof(HighlightTemplate), typeof(DataTemplate), typeof(RichItemsControl));
        public DataTemplate HighlightTemplate
        {
            get => (DataTemplate)GetValue(HighlightTemplateProperty);
            set => SetValue(HighlightTemplateProperty, value);
        }

        //disable scroll, disable zoom, scrollSpeed, SelectionRectangleStyle, GridStyle TBD, EnableSnapping

        public static DependencyProperty MaxScaleProperty = DependencyProperty.Register(nameof(MaxScale), typeof(double), typeof(RichItemsControl), new FrameworkPropertyMetadata(0.1d, OnMaxScaleChanged, CoerceMaxScale));

        public double MaxScale
        {
            get => (double)GetValue(MaxScaleProperty);
            set => SetValue(MaxScaleProperty, value);
        }

        private static void OnMaxScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zoom = (RichItemsControl)d;
            zoom.CoerceValue(ScaleProperty);
        }
        private static object CoerceMaxScale(DependencyObject d, object value)
        {
            var zoom = (RichItemsControl)d;
            var min = zoom.MinScale;

            return (double)value < min ? min : value;
        }

        public static DependencyProperty MinScaleProperty = DependencyProperty.Register(nameof(MinScale), typeof(double), typeof(RichItemsControl), new FrameworkPropertyMetadata(0.1d, OnMinimumScaleChanged, CoerceMinimumScale));
        public double MinScale
        {
            get => (double)GetValue(MinScaleProperty);
            set => SetValue(MinScaleProperty, value);
        }
        private static object CoerceMinimumScale(DependencyObject d, object value)
            => (double)value > 0 ? value : 0.1;

        private static void OnMinimumScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zoom = (RichItemsControl)d;
            zoom.CoerceValue(MaxScaleProperty);
            zoom.CoerceValue(ScaleProperty);
        }

        public static DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(RichItemsControl), new FrameworkPropertyMetadata(1d, OnScaleChanged, ConstarainScaleToRange));
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        private static object ConstarainScaleToRange(DependencyObject d, object value)
        {
            var itemsControl = (RichItemsControl)d;

            //if (itemsControl.DisableZooming)
            //{
            //    return itemsControl.Scale;
            //}

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
        private void OverrideScale(double newValue)
        {
            CoerceValue(ScaleProperty);
            ScaleTransform.ScaleX = newValue;
            ScaleTransform.ScaleY = newValue;
        }

        public static DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(IList)));
        private bool _isResizing;

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public static readonly RoutedEvent DrawingEndedEvent = EventManager.RegisterRoutedEvent(nameof(DrawingEnded), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RichItemsControl));

        public event RoutedEventHandler DrawingEnded
        {
            add { AddHandler(DrawingEndedEvent, value); }
            remove { RemoveHandler(DrawingEndedEvent, value); }
        }

        #endregion

        public double TopLimit { get; set; }
        public double RightLimit { get; set; }
        public double BottomLimit { get; set; }
        public double LeftLimit { get; set; }
        public bool IsPanning { get; private set; }
        public bool IsZooming { get; private set; }
        internal bool IsDrawing => _isDrawing;
        internal bool NeedMeasure { get; set; }
        internal RichItemContainer CurrentDrawingItem => _drawingGesture.CurrentItem;

        static RichItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichItemsControl), new FrameworkPropertyMetadata(typeof(RichItemsControl)));
        }
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
        internal void AddSelection(RichItemContainer container) => _selectingGesture.AddSelection(container);

        internal void ClearSelections()
        {
            _selectingGesture.UnselectAll();
        }
        internal void UpdateSelections()
        {
            // TODO: snap all selections on release
            _selectingGesture.UpdateSelectionsPosition();
            AdjustScroll();
        }

        internal void AdjustScroll()
        {
            ScrollContainer.AdjustScrollVertically();
            ScrollContainer.AdjustScrollHorizontally();
        }

        internal void AddVirtualizableItem(RichItemContainer container)
        {
            var index = ItemContainerGenerator.IndexFromContainer(container);
        }

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
            SelectionRectanlgeTransform = (TransformGroup)selectionRectangle.RenderTransform;

            _mainPanel = (RichCanvas)GetTemplateChild(DrawingPanelName);
            _mainPanel.ItemsOwner = this;

            _canvasContainer = (PanningGrid)GetTemplateChild(CanvasContainerName);
            _canvasContainer.Initalize(this);
            HighlightTemplate = (DataTemplate)FindResource("HighlightAdornerStyle");
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                IsZooming = true;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                IsZooming = false;
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item) => item is RichItemContainer;

        protected override DependencyObject GetContainerForItemOverride() => new RichItemContainer
        {
            RenderTransform = new TransformGroup()
            {
                Children = new TransformCollection { new ScaleTransform(), new TranslateTransform(), new RotateTransform() }
            },
            IsHitTestVisible = true
        };

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Space))
            {
                IsPanning = true;
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
                            RichItemContainer container = (RichItemContainer)ItemContainerGenerator.ContainerFromIndex(_currentDrawingIndexes[i]);
                            container.Host = this;

                            if (container.IsValid())
                            {
                                container.IsDrawn = true;
                            }
                            else
                            {
                                CaptureMouse();
                                _drawingGesture.OnMouseDown(container, position);
                                _isDrawing = true;
                                break;
                            }
                        }
                        _currentDrawingIndexes.Clear();
                    }
                    if (VisualHelper.HasAdornerThumbParent((DependencyObject)e.OriginalSource))
                    {
                        _isResizing = true;
                    }

                    if (!_isDrawing && !DragBehavior.IsDragging && !IsPanning && !_isResizing)
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
                var geom = GetSelectionRectangleCurrentGeometry();
                _selectingGesture.UnselectAll();

                VisualTreeHelper.HitTest(_mainPanel, null,
                    new HitTestResultCallback(OnHitTestResultCallback),
                    new GeometryHitTestParameters(geom));
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_isDrawing)
            {
                _isDrawing = false;
                NeedMeasure = true;
                var drawnItem = _drawingGesture.OnMouseUp();

                ItemsHost.InvalidateMeasure();
                ItemsHost.InvalidateArrange();

                RaiseDrawEndedEvent(drawnItem.DataContext);
                _drawingGesture.Dispose();

                ItemsHost.InvalidateMeasure();
            }
            else if (_isResizing)
            {
                _isResizing = false;
            }
            else if (!DragBehavior.IsDragging && IsSelecting)
            {
                IsSelecting = false;
            }
            if (IsPanning)
            {
                Cursor = Cursors.Arrow;
                IsPanning = false;
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

        private static void OnDisableAutoPanningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((RichItemsControl)d).OnDisableAutoPanningChanged((bool)e.NewValue);

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
        }

        private void HandleAutoPanning(object sender, EventArgs e)
        {
            if (IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed && Mouse.Captured != null && !IsMouseCapturedByScrollBar() && !IsPanning)
            {
                NeedMeasure = false;
                var mousePosition = Mouse.GetPosition(ScrollContainer);
                var transformedPosition = Mouse.GetPosition(ItemsHost);
                if (mousePosition.Y < 0)
                {
                    if (_isDrawing)
                    {
                        TopLimit = Math.Min(ItemsHost.TopLimit, _drawingGesture.GetCurrentTop());
                        // top is not set yet so after drawing the bottom limit will become the intial top
                        BottomLimit = Math.Max(ItemsHost.BottomLimit, CurrentDrawingItem.Top);

                        CurrentDrawingItem.Height = Math.Abs(transformedPosition.Y - CurrentDrawingItem.Top);
                    }
                    ScrollContainer.PanVertically(AutoPanSpeed, true);
                }
                else if (mousePosition.Y > ScrollContainer.ViewportHeight)
                {
                    if (_isDrawing)
                    {
                        if (Items.Count == 1)
                        {
                            TopLimit = _drawingGesture.GetCurrentTop();
                            BottomLimit = _drawingGesture.GetCurrentBottom();
                        }
                        else
                        {
                            BottomLimit = Math.Max(ItemsHost.BottomLimit, _drawingGesture.GetCurrentBottom());
                            TopLimit = Math.Min(ItemsHost.TopLimit, _drawingGesture.GetCurrentTop());
                        }

                        CurrentDrawingItem.Height = Math.Abs(transformedPosition.Y - CurrentDrawingItem.Top);
                    }
                    ScrollContainer.PanVertically(-AutoPanSpeed, true);
                }

                if (mousePosition.X < 0)
                {
                    if (_isDrawing)
                    {
                        LeftLimit = Math.Min(ItemsHost.LeftLimit, _drawingGesture.GetCurrentLeft());
                        // left is not set yet so after drawing the right limit will become the intial left
                        RightLimit = Math.Max(ItemsHost.RightLimit, CurrentDrawingItem.Left);
                        CurrentDrawingItem.Width = Math.Abs(transformedPosition.X - CurrentDrawingItem.Left);
                    }
                    ScrollContainer.PanHorizontally(AutoPanSpeed, true);
                }
                else if (mousePosition.X > ScrollContainer.ViewportWidth)
                {
                    if (_isDrawing)
                    {
                        if (Items.Count == 1)
                        {
                            LeftLimit = _drawingGesture.GetCurrentLeft();
                            RightLimit = _drawingGesture.GetCurrentRight();
                        }
                        else
                        {
                            LeftLimit = Math.Min(ItemsHost.LeftLimit, _drawingGesture.GetCurrentLeft());
                            RightLimit = Math.Max(ItemsHost.RightLimit, _drawingGesture.GetCurrentRight());
                        }
                        CurrentDrawingItem.Width = Math.Abs(transformedPosition.X - CurrentDrawingItem.Left);
                    }
                    ScrollContainer.PanHorizontally(-AutoPanSpeed, true);
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
                _selectingGesture.AddSelection(container);
            }
            return HitTestResultBehavior.Continue;
        }

        private bool IsMouseCapturedByScrollBar()
        {
            return Mouse.Captured.GetType() == typeof(Thumb) || Mouse.Captured.GetType() == typeof(RepeatButton);
        }

        private RectangleGeometry GetSelectionRectangleCurrentGeometry()
        {
            var scaleTransform = (ScaleTransform)SelectionRectanlgeTransform.Children[0];
            var currentSelectionTop = scaleTransform.ScaleY < 0 ? SelectionRectangle.Top - SelectionRectangle.Height : SelectionRectangle.Top;
            var currentSelectionLeft = scaleTransform.ScaleX < 0 ? SelectionRectangle.Left - SelectionRectangle.Width : SelectionRectangle.Left;
            return new RectangleGeometry(new Rect(currentSelectionLeft, currentSelectionTop, SelectionRectangle.Width, SelectionRectangle.Height));
        }
        private static void OnAutoPanTickRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemsControl)d).UpdateTimerInterval();

        private void UpdateTimerInterval()
        {
            _autoPanTimer.Interval = TimeSpan.FromMilliseconds(AutoPanTickRate);
        }
        internal void RaiseDrawEndedEvent(object context)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(DrawingEndedEvent, context);
            RaiseEvent(newEventArgs);
        }
    }
}
