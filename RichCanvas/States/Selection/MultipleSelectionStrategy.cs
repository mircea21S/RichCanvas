using RichCanvas.Helpers;
using System.Collections;
using System.Windows.Media;

namespace RichCanvas.States.SelectionStates
{
    public class MultipleSelectionStrategy : SelectionStrategy
    {
        public MultipleSelectionStrategy(RichItemsControl parent) : base(parent)
        {
        }

        public override void MouseMoveOnRealTimeSelection()
        {
            RectangleGeometry geom = Parent.SelectionRectangle.ToRectangleGeometry();
            if (Parent.SelectedItems?.Count > 0)
            {
                Parent.SelectedItems?.Clear();
            }

            Parent.BeginSelectionTransaction();

            VisualTreeHelper.HitTest(Parent.ItemsHost, null,
                new HitTestResultCallback(OnHitTestResultCallback),
                new GeometryHitTestParameters(geom));

            Parent.EndSelectionTransaction();
        }

        public override void MouseUpOnDeferredSelection()
        {
            MouseMoveOnRealTimeSelection();

            IList selected = Parent.SelectedItems;

            if (selected != null)
            {
                IList added = Parent.BaseSelectedItems;
                for (var i = 0; i < added.Count; i++)
                {
                    // Ensure no duplicates are added
                    if (!selected.Contains(added[i]))
                    {
                        selected.Add(added[i]);
                    }
                }
            }
            if (selected?.Count == 1 || Parent.BaseSelectedItems?.Count == 1)
            {
                Parent.SelectedItem = Parent.BaseSelectedItems[0];
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
                    Parent.BaseSelectedItems.Add(container.DataContext);
                }
            }
            return HitTestResultBehavior.Continue;
        }
    }
}
