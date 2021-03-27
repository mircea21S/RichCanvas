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
        private Point _viewportTopLeft;
        private Point _viewportBottomRight;
        private Point _viewportBottomRightInitial;
        private Point _viewportTopLeftInitial;
        private double _lastTopOffset;
        private double _lastBottomOffset;
        private double _lastOffset;
        private double _lastBottom;
        private Size _lastExtent;
        private double _highestElement;
        private double _lowestElement;
        private double _offsetDifference;
        private bool _adjust;

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
                    //UpdateExtentHeight(-(BottomLimit - _parent.BottomLimit));
                    _lowestElement = _parent.BottomLimit;
                }
                return _parent.BottomLimit == 0 ? 0 : _parent.BottomLimit;
            }
        }
        private double TopLimit => TranslatePoint(_viewportTopLeftInitial, _parent.ItemsHost).Y;
        private double BottomLimit => TranslatePoint(_viewportBottomRightInitial, _parent.ItemsHost).Y;

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
                    ScrollVertically(-deltaHeight);
                    if (BottomLimit < LowestElement || TopLimit > HighestElement)
                    {
                        SetVerticalOffset(-deltaHeight);
                    }
                    else
                    {
                        SetVerticalOffset(0);
                    }
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
                    var offset = y - _lastTopOffset;
                    _lastTopOffset = y;
                    _offset.Y = y;

                    var x = Math.Abs(LowestElement - BottomLimit) * _scaleTransform.ScaleY;
                    var bottomOffset = x - _lastBottomOffset;
                    _lastBottomOffset = x;
                    //UpdateExtentHeight(_offset.Y + x);
                    _extent.Height = _initialExtent.Height + (_offset.Y + x);
                    Console.WriteLine("offset update height: ");
                    Console.WriteLine(bottomOffset);
                    Console.WriteLine(x);
                    Console.WriteLine("offset update height: ");
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

                        //if (_extent.Height + _offset.Y == _viewport.Height || _offset.Y == 0)
                        //{
                        //    UpdateExtentHeight(-offset);
                        //}
                        //else
                        //{
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
                _lastOffset = _offset.Y;
                _lastBottom = _extent.Height;
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
                ScrollVertically(scrollOffset);
                if (BottomLimit < LowestElement || TopLimit > HighestElement)
                {
                    SetVerticalOffset(scrollOffset);
                    CheckLimits(scrollOffset);
                }
                else
                {
                    SetVerticalOffset(0);
                    UpdateExtentHeight(0);
                }
                ScrollOwner.InvalidateScrollInfo();
            }
            Console.WriteLine(_offset.Y);
            Console.WriteLine(_extent.Height);
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

                ScrollVertically(-scrollOffset);
                if (TopLimit > HighestElement || BottomLimit < LowestElement)
                {
                    SetVerticalOffset(-scrollOffset);
                    CheckLimits(-scrollOffset);
                }
                else
                {
                    SetVerticalOffset(0);
                    UpdateExtentHeight(0);
                }
                ScrollOwner.InvalidateScrollInfo();
            }
            Console.WriteLine(_offset.Y);
            Console.WriteLine(_extent.Height);
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
                _lastTopOffset = 0;
                _lastBottomOffset = 0;
            }
            _offset.Y += offset;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            if (ScrollOwner != null)
            {
                if (_viewport != constraint)
                {
                    var x = LowestElement - BottomLimit;
                    _viewportTopLeftInitial = new Point(0, 0);
                    _viewportBottomRightInitial = new Point(ActualWidth, ActualHeight);
                    var previousViewport = _viewport;
                    _viewport = constraint;
                    _initialExtent = _viewport;
                    if (_offset.Y == 0)
                    {
                        _extent.Height = _viewport.Height;
                    }
                    if (_offset.X == 0)
                    {
                        _extent.Width = _viewport.Width;
                    }

                    if (_extent == previousViewport)
                    {
                        _extent = _viewport;
                    }
                    if (previousViewport.Height != constraint.Height)
                    {
                        bool extends = previousViewport.Height < constraint.Height;
                        if (BottomLimit < LowestElement && TopLimit > HighestElement)
                        {
                            //do nothing
                            if (extends)
                            {
                                //extent grows
                                Console.WriteLine(BottomLimit);
                                //_extent.Height -= x;
                                //_extent.Height += LowestElement - BottomLimit;
                                //_extent.Height -= (constraint.Height - previousViewport.Height);
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            //if (_offset.Y > 0)
                            //{
                            //    _offset.Y = TopLimit - HighestElement;
                            //    _extent.Height = constraint.Height + _offset.Y;
                            //}
                            //else if (_offset.Y < 0)
                            //{
                            //    _offset.Y = BottomLimit - LowestElement;
                            //}
                            if (BottomLimit < LowestElement)
                            {
                                if (Math.Round(_extent.Height + _offset.Y, 2) == _viewport.Height)
                                {
                                    _offset.Y = (BottomLimit - LowestElement) * _scaleTransform.ScaleY;
                                    _extent.Height = _viewport.Height + (Math.Abs(LowestElement - BottomLimit) * _scaleTransform.ScaleY);
                                }
                                else
                                {

                                }
                            }
                            if (TopLimit > HighestElement)
                            {
                                if (extends)
                                {
                                    _extent.Height += (constraint.Height - previousViewport.Height);
                                }
                                else
                                {
                                    _extent.Height -= (previousViewport.Height - constraint.Height);
                                }
                            }
                        }

                    }
                }
                Console.WriteLine(_extent.Height);
                Console.WriteLine(_offset.Y);
                Console.WriteLine(_viewport.Height);
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
                    var previousViewport = _viewport;
                    _viewport = arrangeSize;

                    _viewportTopLeftInitial = new Point(0, 0);
                    _viewportBottomRightInitial = new Point(ActualWidth, ActualHeight);
                    //if (_viewportTopLeft.X == 0 && _viewportTopLeft.Y == 0)
                    //{
                    //    _viewportTopLeft = _viewportTopLeftInitial;
                    //}
                    //if (_viewportBottomRight.X == previousViewport.Width && _viewportBottomRight.Y == previousViewport.Height)
                    //{
                    //    _viewportBottomRight = _viewportBottomRightInitial;
                    //}
                    if (_offset.Y == 0)
                    {
                        _extent.Height = _viewport.Height;
                    }
                    if (_offset.X == 0)
                    {
                        _extent.Width = _viewport.Width;
                    }
                    if (_extent == previousViewport)
                    {
                        _extent = _viewport;
                    }
                }
                ScrollOwner.InvalidateScrollInfo();
            }
            return base.ArrangeOverride(arrangeSize);
        }

        private void UpdateExtentWidth(double offset)
        {
            if (_offset.X > _viewport.Width || _offset.X <= 0)
            {
                if (_offset.X < 0)
                {
                    if (offset > 0)
                    {
                        _extent.Width += -offset;
                    }
                    else
                    {
                        _extent.Width += Math.Abs(offset);
                    }
                }
                else
                {
                    _extent.Width += offset;
                }
            }
            else
            {
                _extent.Width += _viewport.Width - _extent.Width;
            }
        }
        private void ScrollVertically(double offset)
        {
            _translateTransform.Y += -offset;
        }

        private void UpdateExtentHeight(double offset)
        {
            if (offset == 0)
            {
                _extent.Height = _viewport.Height;
                return;
            }

            if (_offset.Y < 0)
            {
                _extent.Height += -offset;
            }
            else
            {
                _extent.Height += offset;
            }
        }
        private void CheckLimits(double offset)
        {
            if ((TopLimit > HighestElement && BottomLimit < LowestElement) && !_parent.IsZooming)
            {
                if (_extent.Height - _lastExtent.Height == 10)
                {
                    UpdateExtentHeight(-Math.Abs(offset));
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
    }
}
