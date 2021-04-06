using RichCanvas.Helpers;
using RichCanvas.Gestures;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections;
using System.Linq;
using System.Collections.Specialized;

namespace RichCanvas
{
    [TemplatePart(Name = ElementPanel, Type = typeof(Panel))]
    [TemplatePart(Name = SelectionRectangleName, Type = typeof(Rectangle))]
    public class RichItemsControl : ItemsControl
    {
        public delegate void DrawEndedEventHandler(object context);
        public event DrawEndedEventHandler OnDrawEnded;

        private const string ElementPanel = "PART_Panel";
        private const string SelectionRectangleName = "PART_SelectionRectangle";
        private const string CanvasContainerName = "CanvasContainer";

        private RichCanvas _mainPanel;
        private PanningGrid _canvasContainer;

        public Canvas ContainerDrawing { get; private set; }

        private ICollection<RichItemContainer> _selections = new List<RichItemContainer>();
        private bool _isDrawing;
        private Gestures.Drawing _drawingGesture;
        private Selecting _selectingGesture;
        internal bool HasSelections => _selections.Count > 0;
        internal RichCanvas ItemsHost => _mainPanel;

        public static DependencyProperty MousePositionProperty = DependencyProperty.Register("MousePosition", typeof(Point), typeof(RichItemsControl));

        public Point MousePosition
        {
            get => (Point)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }

        public static DependencyProperty SelectionRectangleProperty = DependencyProperty.Register("SelectionRectangle", typeof(Rect), typeof(RichItemsControl));
        public Rect SelectionRectangle
        {
            get => (Rect)GetValue(SelectionRectangleProperty);
            set => SetValue(SelectionRectangleProperty, value);
        }
        public static DependencyProperty IsSelectingProperty = DependencyProperty.Register("IsSelecting", typeof(bool), typeof(RichItemsControl));
        public bool IsSelecting
        {
            get => (bool)GetValue(IsSelectingProperty);
            set => SetValue(IsSelectingProperty, value);
        }
        public static DependencyProperty AppliedTransformProperty = DependencyProperty.Register("AppliedTransform", typeof(Transform), typeof(RichItemsControl));

        public double TopLimit { get; set; }
        public double RightLimit { get; set; }
        public double BottomLimit { get; set; }
        public double LeftLimit { get; set; }


        public Transform AppliedTransform
        {
            get => (Transform)GetValue(AppliedTransformProperty);
            set => SetValue(AppliedTransformProperty, value);
        }

        public bool IsPanning { get; private set; }
        public bool IsZooming { get; private set; }

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
                    new ScaleTransform(), new TranslateTransform()
                }
            };
            DragBehavior.ItemsControl = this;
            DragBehavior.DragDelta += OnDragDeltaChanged;
            _selectingGesture = new Selecting
            {
                Context = this
            };
            _drawingGesture = new Gestures.Drawing(this);
        }
        private void OnDragDeltaChanged(Point point)
        {
            foreach (var item in _selections)
            {
                var transformGroup = (TransformGroup)item.RenderTransform;
                var translateTransform = (TranslateTransform)transformGroup.Children[1];

                translateTransform.X += point.X;
                translateTransform.Y += point.Y;
            }
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
                Children = new TransformCollection { new ScaleTransform(), new TranslateTransform() }
            },
            IsHitTestVisible = true
        };
        public override void OnApplyTemplate()
        {
            _mainPanel = GetTemplateChild(ElementPanel) as RichCanvas;
            _canvasContainer = GetTemplateChild(CanvasContainerName) as PanningGrid;
            ContainerDrawing = GetTemplateChild("containerDrawing") as Canvas;
            _canvasContainer.Initalize(this);
        }
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Space))
            {
                IsPanning = true;
                Cursor = Cursors.Hand;
            }
            else
            {
                if (!VisualHelper.IsScrollBarParent((DependencyObject)e.OriginalSource))
                {
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        RichItemContainer container = (RichItemContainer)this.ItemContainerGenerator.ContainerFromIndex(i);

                        // already drawn
                        if (container.Height != 0 && container.Width != 0)
                        {
                            container.IsDrawn = true;
                        }

                        if (!container.IsDrawn)
                        {
                            _drawingGesture.OnMouseDown(container, e);
                            _isDrawing = true;
                            CaptureMouse();
                            break;
                        }
                    }
                    if (!_isDrawing && !DragBehavior.IsDragging && !IsPanning)
                    {
                        IsSelecting = true;
                        _selectingGesture.OnMouseDown(e);
                        CaptureMouse();
                    }
                }
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_isDrawing)
            {
                _isDrawing = false;
                var drawnItem = _drawingGesture.OnMouseUp();
                OnDrawEnded?.Invoke(drawnItem.DataContext);
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
            ReleaseMouseCapture();
            Focus();
        }
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            MousePosition = new Point(e.GetPosition(_mainPanel).X, e.GetPosition(_mainPanel).Y);

            if (_isDrawing)
            {
                _drawingGesture.OnMouseMove(e);
            }

            if (IsSelecting)
            {
                _selectingGesture.OnMouseMove(e);

                var geom = new RectangleGeometry(new Rect(SelectionRectangle.Left, SelectionRectangle.Top, SelectionRectangle.Width, SelectionRectangle.Height));
                //Unselect all
                foreach (var selection in _selections)
                {
                    selection.IsSelected = false;
                }
                _selections.Clear();

                VisualTreeHelper.HitTest(_mainPanel, null,
                    new HitTestResultCallback(OnHitTestResultCallback),
                    new GeometryHitTestParameters(geom));
            }
        }
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (Items.Count > 0)
            {
                var items = Items.Cast<object>().Select(i => (RichItemContainer)ItemContainerGenerator.ContainerFromItem(i));
                BottomLimit = items.Select(c => c.Height + c.Top).Max();
                RightLimit = items.Select(c => c.Width + c.Left).Max();
                TopLimit = items.Select(c => c.Top).Min();
                LeftLimit = items.Select(c => c.Left).Min();
                _canvasContainer.AdjustScrollVertically();
            }
        }

        private HitTestResultBehavior OnHitTestResultCallback(HitTestResult result)
        {
            if (result.VisualHit is RichItemContainer container)
            {
                container.IsSelected = true;
                _selections.Add(container);
            }
            return HitTestResultBehavior.Continue;
        }
    }
}
