using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using RichCanvas.Gestures;
using RichCanvas.Helpers;

namespace RichCanvas.States
{
    public class MultipleSelectionState : CanvasState
    {
        private Point _selectionRectangleInitialPosition;

        public MultipleSelectionState(RichItemsControl parent) : base(parent)
        {
        }

        public override void Enter()
        {
            Parent.SelectionRectangle = new Rect();
            Parent.IsSelecting = true;
            _selectionRectangleInitialPosition = Mouse.GetPosition(Parent.ItemsHost);
            Parent.UnselectAll();
        }

        public override void ReEnter()
        {
            Parent.SelectionRectangle = SelectionHelper.DrawSelectionRectangle(Mouse.GetPosition(Parent.ItemsHost), _selectionRectangleInitialPosition);
            SelectItems();
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
                SelectItems();
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
                SelectItems();
            }
        }

        public override void HandleAutoPanning(MouseEventArgs e) => HandleMouseMove(e);

        protected void SelectItems()
        {
            Parent.UnselectAll();
            var geom = new RectangleGeometry(Parent.SelectionRectangle);

            Parent.BeginSelectionTransaction();

            VisualTreeHelper.HitTest(Parent.ItemsHost, null,
                new HitTestResultCallback(OnHitTestResultCallback),
                new GeometryHitTestParameters(geom));

            Parent.EndSelectionTransaction();
        }

        private HitTestResultBehavior OnHitTestResultCallback(HitTestResult result)
        {
            var geometryHitTestResult = (GeometryHitTestResult)result;
            if (geometryHitTestResult.VisualHit.DependencyObjectType.SystemType != typeof(RichItemContainer) && geometryHitTestResult.IntersectionDetail != IntersectionDetail.Empty)
            {
                RichItemContainer container = VisualHelper.GetParentContainer(geometryHitTestResult.VisualHit);
                if (container != null && container.IsSelectable)
                {
                    Parent.BaseSelectedItems.Add(container.DataContext);
                }
            }
            return HitTestResultBehavior.Continue;
        }
    }
}
