using RichCanvas.Gestures;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace RichCanvas
{
    internal class PanningGrid : Grid, IScrollInfo
    {
        private const double DEFAULT_DELTA = 10;

        private TranslateTransform _translateTransform;
        private ScaleTransform _scaleTransform;
        private Zoom _zoomGesture;
        private Vector _offset;
        private Size _extent;
        private Size _initialExtent;
        private Size _viewport;
        private Point _panInitialPosition;
        private RichItemsControl _parent;
        private Point _viewportBottomRightInitial;
        private Point _viewportTopLeftInitial;
        private double _lastBottomOffset;
        private Size _lastExtent;
        private double _highestElement;
        private double _lowestElement;

        private double HighestElement
        {
            get
            {
                if (_highestElement != _parent.TopLimit)
                {
                    if (_offset.Y != 0)
                    {
                        _offset.Y = (TopLimit - _parent.TopLimit) * _scaleTransform.ScaleY;
                    }
                    _highestElement = _parent.TopLimit;
                }
                return _parent.TopLimit == 0 ? _viewport.Height : _parent.TopLimit;
            }
        }
        private double LowestElement
        {
            get
            {
                if (_lowestElement != _parent.BottomLimit)
                {
                    _lowestElement = _parent.BottomLimit;
                }
                return _parent.BottomLimit == 0 ? 0 : _parent.BottomLimit;
            }
        }
        internal double TopLimit => TranslatePoint(_viewportTopLeftInitial, _parent.ItemsHost).Y;
        internal double BottomLimit => TranslatePoint(_viewportBottomRightInitial, _parent.ItemsHost).Y;

        public bool CanHorizontallyScroll { get; set; }
        public bool CanVerticallyScroll { get; set; }

        public double ExtentHeight => _extent.Height;

        public double ExtentWidth => _extent.Width;

        public double HorizontalOffset => _offset.X;

        public ScrollViewer ScrollOwner { get; set; }

        public double VerticalOffset => _offset.Y;

        public double ViewportHeight => _viewport.Height;

        public double ViewportWidth => _viewport.Width;

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Space) && _parent.IsPanning)
            {
                _panInitialPosition = e.GetPosition(this);
                CaptureMouse();
            }
        }
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Space) && Mouse.LeftButton == MouseButtonState.Pressed && _parent.IsPanning)
            {
                var currentPosition = e.GetPosition(this);
                var deltaHeight = currentPosition.Y - _panInitialPosition.Y;
                var deltaWidth = currentPosition.X - _panInitialPosition.X;

                if (deltaWidth != 0)
                {
                    SetHorizontalOffset(-deltaWidth);
                }

                if (deltaHeight != 0)
                {
                    Pan(-deltaHeight);
                }
                ScrollOwner.InvalidateScrollInfo();
                _panInitialPosition = currentPosition;
            }
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
        }
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var position = e.GetPosition(this);
                _zoomGesture.ZoomToPosition(position, e.Delta);

                if (TopLimit > HighestElement && BottomLimit < LowestElement)
                {
                    var y = Math.Abs(TopLimit - HighestElement) * _scaleTransform.ScaleY;
                    _offset.Y = y;

                    var x = Math.Abs(LowestElement - BottomLimit) * _scaleTransform.ScaleY;
                    _extent.Height = _initialExtent.Height + (_offset.Y + x);
                }
                else
                {
                    if (TopLimit > HighestElement)
                    {
                        var y = Math.Abs(TopLimit - HighestElement) * _scaleTransform.ScaleY;
                        _offset.Y = y;
                        _extent.Height = _initialExtent.Height + y;
                    }
                    if (BottomLimit < LowestElement)
                    {
                        _lastBottomOffset = _offset.Y;
                        var y = Math.Abs(LowestElement - BottomLimit) * _scaleTransform.ScaleY;
                        var offset = y - Math.Abs(_lastBottomOffset);

                        if (Math.Round(_extent.Height + _offset.Y, 2) == _viewport.Height)
                        {
                            SetVerticalOffset(-offset);
                            _extent.Height = _initialExtent.Height + (-_offset.Y);
                        }
                        else
                        {
                            var x = Math.Abs(TopLimit - HighestElement) * _scaleTransform.ScaleY;
                            _offset.Y = -x;
                            _extent.Height = _initialExtent.Height + y;
                        }
                    }
                    if (TopLimit < HighestElement && BottomLimit > LowestElement)
                    {
                        SetVerticalOffset(0);
                        UpdateExtentHeight(0);
                    }
                }
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void LineDown()
        {
            SetVerticalOffset(10);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(-10);
        }

        public void LineRight()
        {
            SetHorizontalOffset(10);
        }

        internal void Initalize(RichItemsControl richItemsControl)
        {
            _parent = richItemsControl;
            _translateTransform = (TranslateTransform)((TransformGroup)richItemsControl.AppliedTransform).Children[1];
            _scaleTransform = (ScaleTransform)((TransformGroup)richItemsControl.AppliedTransform).Children[0];
            _zoomGesture = new Zoom(_scaleTransform, _translateTransform);
        }

        public void LineUp()
        {
            SetVerticalOffset(-10);
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            throw new System.NotImplementedException();
        }

        public void MouseWheelDown()
        {
            if (!_parent.IsZooming)
            {
                var scrollOffset = 10;
                Pan(scrollOffset);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void MouseWheelLeft()
        {
            throw new System.NotImplementedException();
        }

        public void MouseWheelRight()
        {
            throw new System.NotImplementedException();
        }

        public void MouseWheelUp()
        {
            if (!_parent.IsZooming)
            {
                var scrollOffset = 10;

                Pan(-scrollOffset);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void PageDown()
        {
            SetVerticalOffset(10);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(-10);
        }

        public void PageRight()
        {
            SetHorizontalOffset(10);
        }

        public void PageUp()
        {
            SetVerticalOffset(-10);
        }

        public void SetHorizontalOffset(double offset)
        {
            _offset.X += offset;
            _translateTransform.X += -offset;
            UpdateExtentWidth(offset);
            ScrollOwner.InvalidateScrollInfo();
        }

        public void SetVerticalOffset(double offset)
        {
            if (offset == 0)
            {
                _offset.Y = 0;
                _lastBottomOffset = 0;
            }
            _offset.Y += offset;
        }

        public void AdjustScrollVertically()
        {
            Console.WriteLine(TopLimit + " " + HighestElement);
            Console.WriteLine(BottomLimit + " " + LowestElement);
            if (!(BottomLimit < LowestElement && TopLimit > HighestElement))
            {
                if (BottomLimit < LowestElement)
                {
                    if (_extent.Height - _offset.Y >= _viewport.Height)
                    {
                        _extent.Height = _viewport.Height + (Math.Abs(LowestElement - BottomLimit) * _scaleTransform.ScaleY);
                    }
                    _offset.Y = (TopLimit - HighestElement) * _scaleTransform.ScaleY;
                }
                if (TopLimit > HighestElement)
                {
                    var offset = Math.Abs(TopLimit - HighestElement) * _scaleTransform.ScaleY;
                    if (_extent.Height - _offset.Y >= _viewport.Height)
                    {
                        _extent.Height += offset;
                    }
                    _offset.Y = offset;
                }
            }
            else
            {
                _offset.Y = Math.Abs(TopLimit - HighestElement) * _scaleTransform.ScaleY;
                var x = Math.Abs(LowestElement - BottomLimit) * _scaleTransform.ScaleY;
                _extent.Height = _initialExtent.Height + (_offset.Y + x);
            }
            ScrollOwner.InvalidateScrollInfo();
        }
        internal void Pan2(double v)
        {
            if (BottomLimit < v)
            {
                _lastBottomOffset = _offset.Y;
                var y = Math.Abs(v - BottomLimit) * _scaleTransform.ScaleY;
                var offset = y - Math.Abs(_lastBottomOffset);

                if (v > LowestElement && offset != 0)
                {
                    SetVerticalOffset(-offset);
                    _extent.Height = _initialExtent.Height + (-_offset.Y);
                }
            }
            ScrollOwner.InvalidateScrollInfo();
        }
        internal void Pan(double offset)
        {
            ScrollVertically(offset);
            if (TopLimit > HighestElement || BottomLimit < LowestElement)
            {
                SetVerticalOffset(offset);
                CheckVerticalLimits(offset);
            }
            else
            {
                SetVerticalOffset(0);
                UpdateExtentHeight(0);
            }
            ScrollOwner.InvalidateScrollInfo();
        }
        internal void UpdateScrollExplictly(double scrollOffset, double offset)
        {
            ScrollVertically(-scrollOffset);
            _offset.Y = offset * _scaleTransform.ScaleY;
            _extent.Height += scrollOffset;
            ScrollOwner.InvalidateScrollInfo();
        }
        protected override Size MeasureOverride(Size constraint)
        {
            if (ScrollOwner != null)
            {
                if (_viewport != constraint)
                {
                    _viewportTopLeftInitial = new Point(0, 0);
                    var previousViewport = _viewport;
                    _viewport = constraint;
                    _viewportBottomRightInitial = new Point(_viewport.Width, _viewport.Height);
                    _initialExtent = _viewport;

                    AdjustExtentHeight(previousViewport);
                }
                ScrollOwner.InvalidateScrollInfo();
            }
            return base.MeasureOverride(constraint);
        }
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (ScrollOwner != null)
            {
                if (_viewport != arrangeSize)
                {
                    _viewportTopLeftInitial = new Point(0, 0);
                    var previousViewport = _viewport;
                    _viewport = arrangeSize;
                    _viewportBottomRightInitial = new Point(_viewport.Width, _viewport.Height);
                    _initialExtent = _viewport;

                    AdjustExtentHeight(previousViewport);
                }
                ScrollOwner.InvalidateScrollInfo();
            }
            return base.ArrangeOverride(arrangeSize);
        }

        private void UpdateExtentWidth(double offset)
        {
            if (offset == 0)
            {
                _extent.Width = _viewport.Width;
                return;
            }

            if (_offset.X < 0)
            {
                _extent.Width += -offset;
            }
            else
            {
                _extent.Width += offset;
            }
        }
        private void ScrollVertically(double offset)
        {
            _translateTransform.Y += -offset;
        }

        private void ScrollHorizontally(double offset)
        {
            _translateTransform.X += -offset;
        }

        private void UpdateExtentHeight(double offset)
        {
            if (offset == 0)
            {
                _extent.Height = _viewport.Height;
                return;
            }

            if (_offset.Y <= 0)
            {
                _extent.Height += -offset;
            }
            else
            {
                _extent.Height += offset;
            }
        }
        private void CheckVerticalLimits(double offset)
        {
            if ((TopLimit > HighestElement && BottomLimit < LowestElement) && !_parent.IsZooming)
            {
                if (Math.Round(_extent.Height - _lastExtent.Height) == 10)
                {
                    UpdateExtentHeight(-Math.Abs(offset));
                }
                else if (Math.Round(_extent.Height - _lastExtent.Height) == -10)
                {
                    UpdateExtentHeight(Math.Abs(offset));
                }
                _lastExtent = _extent;
            }
            else
            {
                if (_offset.Y > 0)
                {
                    if (_extent.Height - (_offset.Y - offset) > _viewport.Height)
                    {
                        UpdateExtentHeight(-Math.Abs((_extent.Height - (_offset.Y - offset)) - _viewport.Height));
                        return;
                    }
                }
                UpdateExtentHeight(offset);
            }
        }
        private void AdjustExtentHeight(Size previousViewport)
        {
            if (_offset.Y == 0)
            {
                _extent.Height = _viewport.Height;
            }
            if (_offset.X == 0)
            {
                _extent.Width = _viewport.Width;
            }
            if (previousViewport.Height != _viewport.Height)
            {
                bool extends = previousViewport.Height < _viewport.Height;
                if (!(BottomLimit < LowestElement && TopLimit > HighestElement))
                {
                    if (BottomLimit < LowestElement)
                    {
                        _offset.Y = (TopLimit - HighestElement) * _scaleTransform.ScaleY;
                        _extent.Height = _viewport.Height + (Math.Abs(LowestElement - BottomLimit) * _scaleTransform.ScaleY);
                    }
                    if (TopLimit > HighestElement)
                    {
                        if (extends)
                        {
                            _extent.Height += (_viewport.Height - previousViewport.Height);
                        }
                        else
                        {
                            _extent.Height -= (previousViewport.Height - _viewport.Height);
                        }
                        if (BottomLimit > LowestElement && _extent.Height + _offset.Y > _viewport.Height)
                        {
                            _extent.Height = _viewport.Height + _offset.Y;
                        }
                    }
                }
                else
                {
                    _offset.Y = Math.Abs(TopLimit - HighestElement) * _scaleTransform.ScaleY;
                    var x = Math.Abs(LowestElement - BottomLimit) * _scaleTransform.ScaleY;
                    _extent.Height = _initialExtent.Height + (_offset.Y + x);
                }
            }
        }
    }
}
