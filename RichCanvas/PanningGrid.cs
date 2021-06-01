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
        #region Private Fields

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

        private double HighestElement => _parent.IsDrawing ? _parent.TopLimit : _parent.ItemsHost.TopLimit;

        private double LowestElement => _parent.IsDrawing ? _parent.BottomLimit : _parent.ItemsHost.BottomLimit;

        private double MostLeftElement => _parent.IsDrawing ? _parent.LeftLimit : _parent.ItemsHost.LeftLimit;

        private double MostRightElement => _parent.IsDrawing ? _parent.RightLimit : _parent.ItemsHost.RightLimit;

        #endregion

        #region Internal Properties

        internal double TopOffset => Math.Abs(TopLimit - HighestElement) * _scaleTransform.ScaleY;

        internal double BottomOffset => (BottomLimit - LowestElement) * _scaleTransform.ScaleY;

        internal double LeftOffset => Math.Abs(LeftLimit - MostLeftElement) * _scaleTransform.ScaleY;

        internal double RightOffset => (RightLimit - MostRightElement) * _scaleTransform.ScaleY;

        internal double TopLimit => TranslatePoint(_viewportTopLeftInitial, _parent.ItemsHost).Y;

        internal double BottomLimit => TranslatePoint(_viewportBottomRightInitial, _parent.ItemsHost).Y;

        internal double LeftLimit => TranslatePoint(_viewportTopLeftInitial, _parent.ItemsHost).X;

        internal double RightLimit => TranslatePoint(_viewportBottomRightInitial, _parent.ItemsHost).X;

        internal bool TranslatedVertically { get; private set; }

        internal bool TranslatedHorizontally { get; private set; }

        #endregion

        #region IScrollInfo

        public bool CanHorizontallyScroll { get; set; }

        public bool CanVerticallyScroll { get; set; }

        public double ExtentHeight => _extent.Height;

        public double ExtentWidth => _extent.Width;

        public double HorizontalOffset => _offset.X;

        public ScrollViewer ScrollOwner { get; set; }

        public double VerticalOffset => _offset.Y;

        public double ViewportHeight => _viewport.Height;

        public double ViewportWidth => _viewport.Width;


        public void LineDown()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void LineLeft()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void LineRight()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void LineUp()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            //not implemented
            if (visual is RichItemContainer container)
            {
                return new Rect(container.Left, container.Top, container.Width, container.Height);
            }
            return new Rect(ScrollOwner.RenderSize);
        }

        public void MouseWheelDown()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void MouseWheelLeft()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void MouseWheelRight()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void MouseWheelUp()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void PageDown()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void PageLeft()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void PageRight()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void PageUp()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void SetHorizontalOffset(double offset)
        {
            if (!_parent.DisableScroll)
            {
                if (!TranslatedHorizontally)
                {
                    if (offset != _offset.X)
                    {
                        if (offset > _offset.X)
                        {
                            if (LeftLimit < MostLeftElement)
                            {
                                ScrollHorizontally(_parent.ScrollFactor);
                            }
                            else
                            {
                                ScrollHorizontally(offset - _offset.X);
                            }
                        }
                        else
                        {
                            ScrollHorizontally(offset - _offset.X);
                        }
                        _offset.X = offset;
                    }
                }
                else
                {
                    offset = CoerceHorizontalOffset(offset);
                    _offset.X = offset;
                }

                if (_offset.X == 0 && LeftLimit < MostLeftElement && RightLimit > MostRightElement)
                {
                    _extent.Width = _viewport.Width;
                }

                TranslatedHorizontally = false;
            }
        }

        public void SetVerticalOffset(double offset)
        {
            if (!_parent.DisableScroll)
            {
                if (!TranslatedVertically)
                {
                    if (offset != _offset.Y)
                    {
                        Console.WriteLine(offset);
                        if (offset > _offset.Y)
                        {
                            if (TopLimit < HighestElement)
                            {
                                ScrollVertically(_parent.ScrollFactor);
                            }
                            else
                            {
                                ScrollVertically(offset - _offset.Y);
                            }
                        }
                        else
                        {
                            ScrollVertically(offset - _offset.Y);
                        }
                        _offset.Y = offset;
                    }
                }
                else
                {
                    offset = CoerceVerticalOffset(offset);
                    _offset.Y = offset;
                }

                if (_offset.Y == 0 && TopLimit < HighestElement && BottomLimit > LowestElement)
                {
                    _extent.Height = _viewport.Height;
                }

                TranslatedVertically = false;
            }
        }

        #endregion

        #region Override Methods

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
                    PanHorizontally(-deltaWidth);
                }

                if (deltaHeight != 0)
                {
                    PanVertically(-deltaHeight);
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
                if (!_parent.DisableZoom)
                {
                    var position = e.GetPosition(this);
                    _zoomGesture.ZoomToPosition(position, e.Delta, _parent.ScaleFactor);
                    TranslatedVertically = true;
                    TranslatedHorizontally = true;

                    SetVerticalOffset(TopOffset);
                    UpdateExtentHeight();

                    SetHorizontalOffset(LeftOffset);
                    UpdateExtentWidth();

                    ScrollOwner.InvalidateScrollInfo();
                }
            }
        }
        protected override Size MeasureOverride(Size constraint)
        {
            if (ScrollOwner != null)
            {

                if (_viewport != constraint)
                {
                    _viewportTopLeftInitial = new Point(0, 0);
                    _viewport = constraint;
                    _viewportBottomRightInitial = new Point(_viewport.Width, _viewport.Height);
                    _initialExtent = _viewport;

                    if (TopLimit < HighestElement && BottomLimit > LowestElement)
                    {
                        _extent.Height = _viewport.Height;
                    }
                    if (LeftLimit < MostLeftElement && RightLimit > MostRightElement)
                    {
                        _extent.Width = _viewport.Width;
                    }

                    AdjustScrollVertically();
                    AdjustScrollHorizontally();
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
                    _viewport = arrangeSize;
                    _viewportBottomRightInitial = new Point(_viewport.Width, _viewport.Height);
                    _initialExtent = _viewport;

                    if (TopLimit > HighestElement && BottomLimit > LowestElement)
                    {
                        _extent.Height = _viewport.Height;
                    }
                    if (LeftLimit < MostLeftElement && RightLimit > MostRightElement)
                    {
                        _extent.Width = _viewport.Width;
                    }

                    AdjustScrollVertically();
                    AdjustScrollHorizontally();
                }
                ScrollOwner.InvalidateScrollInfo();
            }
            return base.ArrangeOverride(arrangeSize);
        }

        #endregion

        #region Internal Methods

        internal void AdjustScrollVertically()
        {
            TranslatedVertically = true;
            SetVerticalOffset(TopOffset);
            UpdateExtentHeight();

            ScrollOwner.InvalidateScrollInfo();
        }

        internal void AdjustScrollHorizontally()
        {
            TranslatedHorizontally = true;
            SetHorizontalOffset(LeftOffset);
            UpdateExtentWidth();

            ScrollOwner.InvalidateScrollInfo();
        }

        internal void PanVertically(double offset, bool reverseScroll = false)
        {
            TranslatedVertically = false;
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
                SetVerticalOffset(VerticalOffset + offset);
                UpdateExtentHeight();
            }
            else
            {
                SetVerticalOffset(0);
            }
            ScrollOwner.InvalidateScrollInfo();
        }

        internal void PanHorizontally(double offset, bool reverseScroll = false)
        {
            TranslatedHorizontally = false;
            if (reverseScroll)
            {
                ScrollHorizontally(-offset);
            }
            else
            {
                ScrollHorizontally(offset);
            }
            if (LeftLimit > MostLeftElement || RightLimit < MostRightElement)
            {
                SetHorizontalOffset(offset);
                UpdateExtentWidth();
            }
            else
            {
                SetHorizontalOffset(0);
            }
            ScrollOwner.InvalidateScrollInfo();
        }

        internal void Initalize(RichItemsControl richItemsControl)
        {
            _parent = richItemsControl;
            _translateTransform = _parent.TranslateTransform;
            _scaleTransform = _parent.ScaleTransform;
            _zoomGesture = new Zoom(_scaleTransform, _translateTransform)
            {
                MinScale = _parent.MinScale,
                MaxScale = _parent.MaxScale,
            };
        }

        #endregion

        #region Private Methods
        private double CoerceVerticalOffset(double offset)
        {
            if (double.IsNaN(offset) || double.IsInfinity(offset))
            {
                offset = 0;
            }
            if (TopLimit > HighestElement)
            {
                offset = TopOffset;
            }
            else if (offset > 0)
            {
                offset = Math.Min(_offset.Y + offset, BottomOffset);
            }
            if (TopLimit < HighestElement && BottomLimit > LowestElement)
            {
                offset = 0;
            }

            return offset;
        }

        private double CoerceHorizontalOffset(double offset)
        {
            if (double.IsNaN(offset) || double.IsInfinity(offset))
            {
                offset = 0;
            }
            if (LeftLimit > MostLeftElement)
            {
                offset = LeftOffset;
            }
            else if (offset > 0)
            {
                offset = Math.Min(_offset.X + offset, RightOffset);
            }
            if (LeftLimit < MostLeftElement && RightLimit > MostRightElement)
            {
                offset = 0;
            }

            return offset;
        }

        private void UpdateExtentWidth()
        {
            if (!_parent.DisableScroll)
            {
                if (LeftLimit > MostLeftElement && RightLimit > MostRightElement)
                {
                    _extent.Width = _initialExtent.Width + Math.Abs(LeftOffset);
                }
                else if (RightLimit < MostRightElement && LeftLimit < MostLeftElement)
                {
                    _extent.Width = _initialExtent.Width + Math.Abs(RightOffset);
                }
                else if (LeftLimit > MostLeftElement && RightLimit < MostRightElement)
                {
                    _extent.Width = _initialExtent.Width + LeftOffset + Math.Abs(RightOffset);
                }
            }
        }
        private void ScrollVertically(double offset)
        {
            _translateTransform.Y += -offset;
            TranslatedVertically = true;
        }

        private void ScrollHorizontally(double offset)
        {
            _translateTransform.X += -offset;
            TranslatedHorizontally = true;
        }

        private void UpdateExtentHeight()
        {
            if (!_parent.DisableScroll)
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
        }

        #endregion

    }
}
