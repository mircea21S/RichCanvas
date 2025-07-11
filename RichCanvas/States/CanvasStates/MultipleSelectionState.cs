using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using RichCanvas.Gestures;
using RichCanvas.Helpers;

namespace RichCanvas.States
{
    /// <summary>
    /// Defines a new state used when selecting multiple items action happens on <see cref="RichCanvas"/>.
    /// </summary>
    public class MultipleSelectionState : CanvasState
    {
        private Point _selectionRectangleInitialPosition;

        /// <summary>
        /// Initializes a new <see cref="MultipleSelectionState"/>.
        /// </summary>
        /// <param name="parent">Owner of the state.</param>
        public MultipleSelectionState(RichCanvas parent) : base(parent)
        {
        }

        /// <inheritdoc/>
        public override void Enter()
        {
            Parent.SelectionRectangle = new Rect();
            Parent.IsSelecting = true;
            _selectionRectangleInitialPosition = Mouse.GetPosition(Parent.ItemsHost);
            Parent.UnselectAll();
        }

        /// <inheritdoc/>
        public override void ReEnter()
        {
            Parent.SelectionRectangle = SelectionHelper.DrawSelectionRectangle(Mouse.GetPosition(Parent.ItemsHost), _selectionRectangleInitialPosition);
            SelectItems();
        }

        /// <inheritdoc/>
        public override void HandleKeyDown(KeyEventArgs e)
        {
            if (RichCanvasGestures.Pan.Matches(e.Source, e))
            {
                PushState(new PanningState(Parent));
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (RichCanvasGestures.Pan.Matches(e.Source, e))
            {
                PushState(new PanningState(Parent));
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void HandleAutoPanning(MouseEventArgs e) => HandleMouseMove(e);

        private void SelectItems()
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
            if (geometryHitTestResult.VisualHit.DependencyObjectType.SystemType != typeof(RichCanvasContainer) && geometryHitTestResult.IntersectionDetail != IntersectionDetail.Empty)
            {
                RichCanvasContainer container = VisualHelper.GetParentContainer(geometryHitTestResult.VisualHit);
                if (container != null && container.IsSelectable)
                {
                    Parent.BaseSelectedItems.Add(container.DataContext);
                }
            }
            return HitTestResultBehavior.Continue;
        }
    }
}
