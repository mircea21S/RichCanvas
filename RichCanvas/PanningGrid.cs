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
        private double _highestElement;
        private double _lowestElement;

        private double HighestElement
        {
            get
            {
                if (_highestElement != _parent.TopLimit)
                {
                    _highestElement = _parent.TopLimit;
                }
                return _parent.TopLimit == 0 ? TopLimit : _parent.TopLimit;
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

        internal double TopOffset => Math.Abs(TopLimit - HighestElement) * _scaleTransform.ScaleY;

        internal double BottomOffset => (BottomLimit - LowestElement) * _scaleTransform.ScaleY;

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
                    SetVerticalOffset(TopOffset);
                    _extent.Height = _initialExtent.Height + TopOffset + Math.Abs(BottomOffset);
                }
                else
                {
                    if (TopLimit > HighestElement)
                    {
                        SetVerticalOffset(TopOffset);
                        _extent.Height = _initialExtent.Height + TopOffset;
                    }
                    if (BottomLimit < LowestElement)
                    {
                        SetVerticalOffset(BottomOffset);
                        _extent.Height = _initialExtent.Height + Math.Abs(BottomOffset);
                    }
                    if (TopLimit < HighestElement && BottomLimit > LowestElement)
                    {
                        SetVerticalOffset(0);
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
                // reset
                _offset.Y = 0;
                _extent.Height = _viewport.Height;
            }
            if (TopLimit > HighestElement)
            {
                _offset.Y = TopOffset;
            }
            else
            {
                _offset.Y = Math.Min(_offset.Y + offset, BottomOffset);
            }
        }

        public void AdjustScrollVertically()
        {
            if (!(BottomLimit < LowestElement && TopLimit > HighestElement))
            {
                if (BottomLimit < LowestElement)
                {
                    SetVerticalOffset(BottomOffset);
                    _extent.Height = _initialExtent.Height + Math.Abs(BottomOffset);
                }
                if (TopLimit > HighestElement)
                {
                    _extent.Height = _initialExtent.Height + TopOffset;
                    SetVerticalOffset(TopOffset);
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

        internal void Pan(double offset, bool reverseScroll = false)
        {
            if (reverseScroll)
            {
                ScrollVertically(-offset);
            }
            else
            {
                ScrollVertically(offset);
            }
            if (TopLimit > HighestElement || BottomLimit < LowestElement)
            {
                SetVerticalOffset(offset);
                CheckVerticalLimits();
            }
            else
            {
                SetVerticalOffset(0);
            }
            ScrollOwner.InvalidateScrollInfo();
        }

        internal void ResetScroll()
        {
            SetVerticalOffset(0);
            ScrollOwner?.InvalidateScrollInfo();
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

        private void UpdateExtentHeight()
        {
            if (TopLimit > HighestElement && BottomLimit > LowestElement)
            {
                _extent.Height = _initialExtent.Height + Math.Abs(TopOffset);
            }
            else if (BottomLimit < LowestElement && TopLimit < HighestElement)
            {
                _extent.Height = _initialExtent.Height + Math.Abs(BottomOffset);
            }
            else if (TopLimit > HighestElement && BottomLimit < LowestElement)
            {
                _extent.Height = _initialExtent.Height + TopOffset + Math.Abs(BottomOffset);
            }
        }
        private void CheckVerticalLimits()
        {
            if ((TopLimit > HighestElement && BottomLimit < LowestElement) && !_parent.IsZooming)
            {
                SetVerticalOffset(TopOffset);
                UpdateExtentHeight();
            }
            else
            {
                UpdateExtentHeight();
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
                    SetVerticalOffset(TopOffset);
                    _offset.Y = Math.Abs(TopLimit - HighestElement) * _scaleTransform.ScaleY;
                    var x = Math.Abs(LowestElement - BottomLimit) * _scaleTransform.ScaleY;
                    _extent.Height = _initialExtent.Height + (_offset.Y + x);
                }
            }
        }
    }
}
