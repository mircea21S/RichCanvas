using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace RichCanvas.States
{
    public static class StateManager
    {
        private static Dictionary<Type, Func<InputEventArgs, bool>> _canvasStates = new Dictionary<Type, Func<InputEventArgs, bool>>();
        private static Dictionary<Type, Func<InputEventArgs, bool>> _containerStates = new Dictionary<Type, Func<InputEventArgs, bool>>();

        public static void RegisterCanvasState<T>(Func<InputEventArgs, bool> canExecute) where T : CanvasState => _canvasStates[typeof(T)] = canExecute;

        public static void RegisterContainerState<T>(Func<InputEventArgs, bool> canExecute) where T : ContainerState => _containerStates[typeof(T)] = canExecute;

        /// <summary>
        /// Finds the matching <see cref="CanvasState"/> based on canExecute condition passed on registration.
        /// <br />
        /// Note: <i>If two or more states are matching, it picks up the first one in the order they were registered.</i>
        /// </summary>
        /// <param name="e"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static CanvasState? GetMatchingCanvasState(InputEventArgs e, RichItemsControl parent)
        {
            var matchingStateType = _canvasStates.FirstOrDefault(c => c.Value(e)).Key;
            if (matchingStateType == null)
            {
                return null;
            }
            var state = (CanvasState?)Activator.CreateInstance(matchingStateType, parent);
            return state;
        }

        public static ContainerState? GetMatchingContainerState(InputEventArgs e, RichItemContainer parent)
        {
            var matchingStateType = _containerStates.FirstOrDefault(c => c.Value(e)).Key;
            if (matchingStateType == null)
            {
                return null;
            }
            var state = (ContainerState?)Activator.CreateInstance(matchingStateType, parent);
            return state;
        }
    }
}
