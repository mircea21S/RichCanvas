using System.Windows;
using System.Windows.Input;

namespace RichCanvas.States.ContainerStates
{
    public class DraggingContainerState : ContainerState
    {
        private Point _initialPosition;
        private DraggingStrategy _draggingStrategy;
        private DraggingStrategy DraggingStrategy => _draggingStrategy ??= Container.Host.CanSelectMultipleItems ? new MultipleDraggingStrategy(Container) : new SingleDraggingStrategy(Container);

        public DraggingContainerState(RichItemContainer container) : base(container)
        {
        }

        public override void Enter()
        {
            _initialPosition = Mouse.GetPosition(Container?.Host?.ItemsHost);
            if (Container.IsSelectable)
            {
                if (Container.Host.CanSelectMultipleItems)
                {
                    Container.IsSelected = true;
                }
                else
                {
                    Container?.Host?.UpdateSingleSelectedItem(Container);
                }
            }
            Container.Host.IsDragging = true;
            DraggingStrategy.OnItemsDragStarted();
            Container?.RaiseDragStartedEvent(_initialPosition);
        }

        public override void HandleMouseMove(MouseEventArgs e)
        {
            var currentPosition = e.GetPosition(Container?.Host?.ItemsHost);
            var offset = currentPosition - _initialPosition;
            if (offset.X != 0 || offset.Y != 0)
            {
                var offsetPoint = new Point(offset.X, offset.Y);
                DraggingStrategy.OnItemsDragDelta(offsetPoint);
                Container?.RaiseDragDeltaEvent(offsetPoint);

                _initialPosition = currentPosition;
            }
        }

        public override void HandleMouseUp(MouseButtonEventArgs e)
        {
            DraggingStrategy.OnItemsDragCompleted();
            Container.Host.IsDragging = false;
            Container.RaiseDragCompletedEvent(e.GetPosition(Container.Host.ItemsHost));
        }
    }
}
