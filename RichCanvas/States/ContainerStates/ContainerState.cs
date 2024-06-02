using System.Windows.Input;

namespace RichCanvas.States.ContainerStates
{
    /// <summary>The base class for container states.</summary>
    public abstract class ContainerState
    {
        /// <summary>The owner of the state.</summary>
        protected RichItemContainer Container { get; }

        /// <summary>
        /// Constructs a new <see cref="ContainerState "/>.
        /// </summary>
        /// <param name="container">The owner of the state.</param>
        public ContainerState(RichItemContainer container)
        {
            Container = container;
        }

        /// <summary>
        /// Called whenever <see cref="RichItemContainer.PushState(ContainerState)"/> is called (becomes the <see cref="RichItemContainer.CurrentState"/>).
        /// <br />
        /// Note: <i>Used to initialize the State before any input is processed by it.</i>
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// Called whenever <see cref="RichItemContainer.PopState()"/> is called.
        /// <br />
        /// Note: <i>Used whenever a state switch happens in order to update the state which was suspended.</i>
        /// </summary>
        public virtual void ReEnter() { }

        /// <summary>
        /// Called whenever <see cref="RichItemContainer.PopState()"/> is called.
        /// </summary>
        public virtual void Exit() { }

        /// <inheritdoc cref="RichItemContainer.OnMouseDown(MouseButtonEventArgs)"/>
        public virtual void HandleMouseDown(MouseButtonEventArgs e) { }

        /// <inheritdoc cref="RichItemContainer.OnMouseMove(MouseEventArgs)"/>
        public virtual void HandleMouseMove(MouseEventArgs e) { }

        /// <inheritdoc cref="RichItemContainer.OnMouseUp(MouseButtonEventArgs)"/>
        public virtual void HandleMouseUp(MouseButtonEventArgs e) { }

        /// <summary>Pushes a new state into the stack.</summary>
        /// <param name="state">The new state.</param>
        public virtual void PushState(ContainerState state) => Container.PushState(state);

        /// <summary>Pops the current state from the stack.</summary>
        public virtual void PopState() => Container.PopState();
    }
}
