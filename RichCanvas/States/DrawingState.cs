using RichCanvas.Helpers;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RichCanvas.States
{
    public class DrawingState : CanvasState
    {
        private RichItemContainer _currentDrawingContainer;
        private bool _isDrawing;
        private Point _initialPosition;

        public DrawingState(RichItemsControl parent) : base(parent)
        {
        }

        public override void Enter(MouseEventArgs e)
        {
            var drawingContainersIndexes = Parent.CurrentDrawingIndexes;
            if (drawingContainersIndexes.Count == 0)
            {
                return;
            }

            var currentDrawingContainerIndex = drawingContainersIndexes[drawingContainersIndexes.Count - 1];
            var container = (RichItemContainer)Parent.ItemContainerGenerator.ContainerFromIndex(currentDrawingContainerIndex);
            if (container == null)
            {
                return;
            }

            _initialPosition = e.GetPosition(Parent.ItemsHost);
            _currentDrawingContainer = container;
        }

        public override void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (VisualHelper.HasScrollBarParent((DependencyObject)e.OriginalSource))
            {
                return;
            }
            _isDrawing = true;

            if (!_currentDrawingContainer.TopPropertyInitalized)
            {
                _currentDrawingContainer.Top = _initialPosition.Y;
            }
            if (!_currentDrawingContainer.LeftPropertyInitialized)
            {
                _currentDrawingContainer.Left = _initialPosition.X;
            }

            ScaleTransform? scaleTransform = _currentDrawingContainer.ScaleTransform;

            double width = _initialPosition.X - _currentDrawingContainer.Left;
            double height = _initialPosition.Y - _currentDrawingContainer.Top;

            _currentDrawingContainer.Width = width == 0 ? RichItemContainer.DefaultWidth : Math.Abs(width);
            _currentDrawingContainer.Height = height == 0 ? RichItemContainer.DefaultHeight : Math.Abs(height);

            if (scaleTransform != null)
            {
                if (width < 0 && scaleTransform.ScaleX == 1)
                {
                    scaleTransform.ScaleX = -1;
                }

                if (height < 0 && scaleTransform.ScaleY == 1)
                {
                    scaleTransform.ScaleY = -1;
                }

                if (height > 0 && scaleTransform.ScaleY == -1)
                {
                    scaleTransform.ScaleY = 1;
                }
                if (width > 0 && scaleTransform.ScaleX == -1)
                {
                    scaleTransform.ScaleX = 1;
                }
            }

            var drawingContainersIndexes = Parent.CurrentDrawingIndexes;
            drawingContainersIndexes.Remove(drawingContainersIndexes[drawingContainersIndexes.Count - 1]);
        }

        public override void HandleMouseMove(MouseEventArgs e)
        {
            if (!_isDrawing)
            {
                return;
            }

            var mousePosition = e.GetPosition(Parent.ItemsHost);
            ScaleTransform? scaleTransform = _currentDrawingContainer.ScaleTransform;

            double width = mousePosition.X - _currentDrawingContainer.Left;
            double height = mousePosition.Y - _currentDrawingContainer.Top;

            _currentDrawingContainer.Width = width == 0 ? 1 : Math.Abs(width);
            _currentDrawingContainer.Height = height == 0 ? 1 : Math.Abs(height);

            if (scaleTransform != null)
            {
                if (width < 0 && scaleTransform.ScaleX == 1)
                {
                    scaleTransform.ScaleX = -1;
                }

                if (height < 0 && scaleTransform.ScaleY == 1)
                {
                    scaleTransform.ScaleY = -1;
                }

                if (height > 0 && scaleTransform.ScaleY == -1)
                {
                    scaleTransform.ScaleY = 1;
                }
                if (width > 0 && scaleTransform.ScaleX == -1)
                {
                    scaleTransform.ScaleX = 1;
                }
            }
        }

        public override void HandleMouseUp(MouseButtonEventArgs e)
        {
            if (!_isDrawing)
            {
                return;
            }

            if (_currentDrawingContainer.AllowScaleChangeToUpdatePosition)
            {
                UpdateItemPositionByScale();
            }
            if (Parent.EnableGrid && Parent.EnableSnapping)
            {
                SnapToGrid();
            }
            _currentDrawingContainer.Scale = new Point(_currentDrawingContainer.ScaleTransform.ScaleX, _currentDrawingContainer.ScaleTransform.ScaleY);

            var mousePosition = e.GetPosition(Parent.ItemsHost);
            Parent.RaiseDrawEndedEvent(_currentDrawingContainer.DataContext, mousePosition);

            Parent.ItemsHost?.InvalidateMeasure();
            _isDrawing = false;
        }

        public override void HandleAutoPanning(Point mousePosition, bool heightChanged = false)
        {
            if (heightChanged)
            {
                _currentDrawingContainer.Height = Math.Abs(mousePosition.Y - _currentDrawingContainer.Top);
            }
            else
            {
                _currentDrawingContainer.Width = Math.Abs(mousePosition.X - _currentDrawingContainer.Left);
            }
        }

        // ScaleTransform is used, because drawing involves anything that the User adds inside the ContentPresenter
        // so we can't change the position while drawing, therefore we need to keep the scaling to display correctly OnMouseUp.
        private void UpdateItemPositionByScale()
        {
            ScaleTransform? scaleTransformItem = _currentDrawingContainer.ScaleTransform;

            if (scaleTransformItem != null)
            {
                _currentDrawingContainer.RenderTransformOrigin = new Point(0.5, 0.5);
                if (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY > 0)
                {
                    _currentDrawingContainer.Left -= _currentDrawingContainer.Width;
                }
                else if (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY < 0)
                {
                    _currentDrawingContainer.Left -= _currentDrawingContainer.Width;
                    _currentDrawingContainer.Top -= _currentDrawingContainer.Height;
                }
                else if (scaleTransformItem.ScaleX > 0 && scaleTransformItem.ScaleY < 0)
                {
                    _currentDrawingContainer.Top -= _currentDrawingContainer.Height;
                }
            }
        }

        private void SnapToGrid()
        {
            _currentDrawingContainer.Left = Math.Round(_currentDrawingContainer.Left / Parent.GridSpacing) * Parent.GridSpacing;
            _currentDrawingContainer.Top = Math.Round(_currentDrawingContainer.Top / Parent.GridSpacing) * Parent.GridSpacing;
        }
    }
}
