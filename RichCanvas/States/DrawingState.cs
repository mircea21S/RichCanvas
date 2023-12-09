﻿using RichCanvas.Helpers;
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

        public DrawingState(RichItemsControl parent) : base(parent)
        {
        }

        public override void Enter()
        {
            //TODO: fix setting the value correct in mouse move on click as there is the where the click happens
            //set here the container if it's null then mouse move can't work and _isDrawing is true there
            var drawingContainersIndexes = Parent.CurrentDrawingIndexes;
            if (drawingContainersIndexes.Count == 0)
            {
                return;
            }

            var currentDrawingContainerIndex = drawingContainersIndexes[drawingContainersIndexes.Count - 1];
            var container = (RichItemContainer)Parent.ItemContainerGenerator.ContainerFromIndex(currentDrawingContainerIndex);
            _currentDrawingContainer = container;
        }

        public override void HandleMouseDown(MouseButtonEventArgs e)
        {
            var position = e.GetPosition(Parent.ItemsHost);
            if (VisualHelper.HasScrollBarParent((DependencyObject)e.OriginalSource))
            {
                return;
            }

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

            _isDrawing = true;

            //if (!container.TopPropertySet)
            //{
            container.Top = position.Y;
            //}
            //if (!container.LeftPropertySet)
            //{
            container.Left = position.X;
            //}

            _currentDrawingContainer = container;
            _currentDrawingContainer.CoerceValue(RichItemContainer.TopProperty);
            _currentDrawingContainer.CoerceValue(RichItemContainer.LeftProperty);
            drawingContainersIndexes.Remove(currentDrawingContainerIndex);
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

            SetItemPosition();
            SnapToGrid();
            _currentDrawingContainer.CoerceValue(RichItemContainer.TopProperty);
            _currentDrawingContainer.CoerceValue(RichItemContainer.LeftProperty);
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

        // ScaleTransform used, because drawing involves anything that the User adds inside the ContentPresenter
        // so we can't change the position while drawing and we need to keep the scaling to display correctly.
        private void SetItemPosition()
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
            if (Parent.EnableGrid && Parent.EnableSnapping)
            {
                _currentDrawingContainer.Left = Math.Round(_currentDrawingContainer.Left / Parent.GridSpacing) * Parent.GridSpacing;
                _currentDrawingContainer.Top = Math.Round(_currentDrawingContainer.Top / Parent.GridSpacing) * Parent.GridSpacing;
            }
        }
    }
}
