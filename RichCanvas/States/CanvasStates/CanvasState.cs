using System.Windows.Input;

namespace RichCanvas.States
{
    /// <summary>The base class for <see cref="RichItemsControl"/> states.</summary>
    public abstract class CanvasState
    {
        /// <summary>The owner of the state.</summary>
        protected RichItemsControl Parent { get; }

        /// <summary>
        /// Constructs a new <see cref="CanvasState "/>.
        /// </summary>
        /// <param name="parent">The owner of the state.</param>
        public CanvasState(RichItemsControl parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Called whenever <see cref="RichItemsControl.PushState(CanvasState)"/> is called (becomes the <see cref="RichItemsControl.CurrentState"/>).
        /// <br />
        /// Note: <i>Used to initialize the State before any input is processed by it.</i>
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// Called whenever <see cref="RichItemsControl.PopState()"/> is called.
        /// <br />
        /// Note: <i>Used whenever a state switch happens in order to update the state which was suspended.</i>
        /// </summary>
        public virtual void ReEnter() { }

        /// <summary>
        /// Called whenever <see cref="RichItemsControl.PopState()"/> is called.
        /// </summary>
        public virtual void Exit() { }

        /// <inheritdoc cref="RichItemsControl.OnMouseDown(MouseButtonEventArgs)"/>
        public virtual void HandleMouseDown(MouseButtonEventArgs e) { }

        /// <inheritdoc cref="RichItemsControl.OnMouseMove(MouseEventArgs)"/>
        public virtual void HandleMouseMove(MouseEventArgs e) { }

        /// <inheritdoc cref="RichItemsControl.OnMouseUp(MouseButtonEventArgs)"/>
        public virtual void HandleMouseUp(MouseButtonEventArgs e) { }

        /// <inheritdoc cref="RichItemsControl.OnKeyDown(KeyEventArgs)"/>
        public virtual void HandleKeyDown(KeyEventArgs e) { }

        /// <inheritdoc cref="RichItemsControl.OnKeyUp(KeyEventArgs)"/>
        public virtual void HandleKeyUp(KeyEventArgs e) { }

        /// <summary>Handles auto panning when mouse is outside the canvas.</summary>
        public virtual void HandleAutoPanning(MouseEventArgs e) { }

        /// <summary>Pushes a new state into the stack.</summary>
        /// <param name="state">The new state.</param>
        public virtual void PushState(CanvasState state) => Parent.PushState(state);

        /// <summary>Pops the current state from the stack.</summary>
        public virtual void PopState() => Parent.PopState();

        /// <summary>
        /// Called by <see cref="RichItemsControl.OnPreviewMouseDown(MouseButtonEventArgs)"/> to check if any state has priority over other controls handling MouseDown event.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="matchingState"></param>
        /// <returns></returns>
        public virtual bool MatchesPreviewMouseDownState(MouseButtonEventArgs e, out CanvasState? matchingState)
        {
            matchingState = null;
            return false;
        }
    }
}
