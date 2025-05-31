using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using RichCanvas.Gestures;
using RichCanvas.Helpers;

namespace RichCanvas.States.CanvasStates
{
    public class SingleSelectionState : CanvasState
    {
        private Point _selectionRectangleInitialPosition;
        private RichItemContainer _selectedContainer;
        private List<RichItemContainer> _selectedContainers = [];

        public SingleSelectionState(RichItemsControl parent) : base(parent)
        {
        }

        public override void Enter()
        {
            Parent.SelectionRectangle = new Rect();
            Parent.IsSelecting = true;
            _selectionRectangleInitialPosition = Mouse.GetPosition(Parent.ItemsHost);
            Parent.SelectedItem = null;
        }

        public override void ReEnter()
        {
            SelectItem(Parent.RealTimeSelectionEnabled);
        }

        public override void HandleKeyDown(KeyEventArgs e)
        {
            if (RichCanvasGestures.Pan.Matches(e.Source, e))
            {
                PushState(new PanningState(Parent));
            }
        }

        public override void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (RichCanvasGestures.Pan.Matches(e.Source, e))
            {
                PushState(new PanningState(Parent));
            }
        }

        public override void HandleMouseMove(MouseEventArgs e)
        {
            if (!Parent.IsSelecting)
            {
                return;
            }

            Parent.SelectionRectangle = SelectionHelper.DrawSelectionRectangle(e.GetPosition(Parent.ItemsHost), _selectionRectangleInitialPosition);

            if (Parent.RealTimeSelectionEnabled)
            {
                SelectItem();
            }
        }

        public override void HandleMouseUp(MouseButtonEventArgs e)
        {
            if (!Parent.IsSelecting)
            {
                return;
            }
            Parent.IsSelecting = false;
            if (!Parent.RealTimeSelectionEnabled)
            {
                SelectItem(true);
            }
        }

        public override void HandleAutoPanning(MouseEventArgs e) => HandleMouseMove(e);

        private void SelectItem(bool defferedSelection = false)
        {
            if (!defferedSelection)
            {
                _selectedContainers.Clear();
            }

            var geom = new RectangleGeometry(Parent.SelectionRectangle);

            VisualTreeHelper.HitTest(Parent.ItemsHost, null,
                new HitTestResultCallback(OnHitTestResultCallback),
                new GeometryHitTestParameters(geom));

            if (!defferedSelection)
            {
                if (Parent.SelectedItem == null && _selectedContainers.Count > 0)
                {
                    UpdateSelectedItem();
                }
                if ((_selectedContainers.Count > 0 && !_selectedContainers.Contains(_selectedContainer)) || _selectedContainers.Count == 0)
                {
                    Parent.SelectedItem = null;
                    if (_selectedContainer != null)
                    {
                        _selectedContainer.IsSelected = false;
                    }
                    if (_selectedContainers.Count > 0)
                    {
                        UpdateSelectedItem();
                    }
                }
            }

            if (defferedSelection && _selectedContainers.Count > 0)
            {
                _selectedContainers[0].IsSelected = true;
            }
        }

        private HitTestResultBehavior OnHitTestResultCallback(HitTestResult result)
        {
            var geometryHitTestResult = (GeometryHitTestResult)result;
            if (geometryHitTestResult.VisualHit.DependencyObjectType.SystemType != typeof(RichItemContainer) && geometryHitTestResult.IntersectionDetail != IntersectionDetail.Empty)
            {
                RichItemContainer container = VisualHelper.GetParentContainer(geometryHitTestResult.VisualHit);
                if (container != null && container.IsSelectable)
                {
                    // first element of this list is the last added item in the ItemsSource
                    _selectedContainers.Add(container);
                }
            }
            return HitTestResultBehavior.Continue;
        }

        private void UpdateSelectedItem()
        {
            _selectedContainers[0].IsSelected = true;
            _selectedContainer = _selectedContainers[0];
        }
    }
}
