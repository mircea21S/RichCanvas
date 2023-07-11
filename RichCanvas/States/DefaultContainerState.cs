using RichCanvas.Gestures;
using RichCanvas.States.Dragging;
using System;
using System.Windows.Input;

namespace RichCanvas.States
{
    public class DefaultContainerState : ContainerState
    {
        public DefaultContainerState(RichItemContainer container) : base(container)
        {
        }

        public override void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (RichCanvasGestures.Drag.Matches(e.Source, e))
            {
                var draggingState = new DraggingContainerState(Container);
                Container.CurrentState = draggingState;
                draggingState.HandleMouseDown(e);
            }
        }
    }
}
