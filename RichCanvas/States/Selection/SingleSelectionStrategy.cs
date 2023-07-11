using RichCanvas.Helpers;
using System.Collections.Generic;
using System.Windows.Media;

namespace RichCanvas.States.SelectionStates
{
    public class SingleSelectionStrategy : SelectionStrategy
    {
        private List<object> _selectedContainers = new List<object>();
        private RichItemContainer? _selectedContainer;

        public SingleSelectionStrategy(RichItemsControl parent) : base(parent)
        {
        }

        public override void MouseMoveOnRealTimeSelection()
        {
            RectangleGeometry geom = Parent.SelectionRectangle.ToRectangleGeometry();

            VisualTreeHelper.HitTest(Parent.ItemsHost, null,
                new HitTestResultCallback(OnHitTestResultCallback),
                new GeometryHitTestParameters(geom));

            if (!_selectedContainers.Contains(Parent.SelectedItem) && Parent.SelectedItem != null)
            {
                Parent.SelectedItem = null;
                if (_selectedContainer != null)
                {
                    _selectedContainer.IsSelected = false;
                    _selectedContainer = null;
                }
            }
            _selectedContainers.Clear();
        }

        public override void MouseUpOnDeferredSelection()
        {
            MouseMoveOnRealTimeSelection();

            if (_selectedContainer != null)
            {
                Parent.SelectedItem = _selectedContainer.DataContext;
            }
            else
            {
                Parent.SelectedItem = null;
            }
        }

        private HitTestResultBehavior OnHitTestResultCallback(HitTestResult result)
        {
            var geometryHitTestResult = (GeometryHitTestResult)result;
            if (geometryHitTestResult.VisualHit.DependencyObjectType.SystemType != typeof(RichItemContainer) && geometryHitTestResult.IntersectionDetail != IntersectionDetail.Empty)
            {
                var container = VisualHelper.GetParentContainer(geometryHitTestResult.VisualHit);
                if (container != null && container.IsSelectable)
                {
                    if (Parent.SelectedItem == null)
                    {
                        container.IsSelected = true;
                        _selectedContainer = container;
                        _selectedContainers.Add(container.DataContext);
                    }
                    else
                    {
                        _selectedContainer ??= container;
                    }
                }
            }
            return HitTestResultBehavior.Continue;
        }
    }
}
