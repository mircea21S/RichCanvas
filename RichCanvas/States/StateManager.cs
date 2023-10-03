using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace RichCanvas.States
{
    public static class StateManager
    {
        private static readonly Dictionary<Type, InputGesture> _stateTypeByGesture = new Dictionary<Type, InputGesture>();
        private static readonly Dictionary<Type, Func<bool>> _executableStates = new Dictionary<Type, Func<bool>>();

        /// <summary>
        /// Finds the matching <see cref="CanvasState"/> based on <paramref name="e"/>.
        /// <br />
        /// Note: <i>If two or more states are matching, it picks up the first one in the order they were registered.</i>
        /// </summary>
        /// <param name="e"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static CanvasState? GetMatchingCanvasState(InputEventArgs e, RichItemsControl parent)
        {
            var matchingStateTypes = _stateTypeByGesture.Where(c => c.Value.Matches(e.Source, e)).Select(c => c.Key);
            if (matchingStateTypes == null)
            {
                return null;
            }

            foreach (var matchingType in matchingStateTypes)
            {
                if (!matchingType.IsSubclassOf(typeof(CanvasState)))
                {
                    continue;
                }
                var state = (CanvasState?)Activator.CreateInstance(matchingType, parent);
                if (_executableStates.TryGetValue(matchingType, out var canExecute))
                {
                    if (canExecute())
                    {
                        return state;
                    }
                    else
                    {
                        continue;
                    }
                }
                return state;
            }
            return null;
        }

        public static ContainerState? GetMatchingContainerState(InputEventArgs e, RichItemContainer parent)
        {
            var matchingStateTypes = _stateTypeByGesture.Where(c => c.Value.Matches(e.Source, e)).Select(c => c.Key);
            if (matchingStateTypes == null)
            {
                return null;
            }

            foreach (var matchingType in matchingStateTypes)
            {
                if (!matchingType.IsSubclassOf(typeof(ContainerState)))
                {
                    continue;
                }
                var state = (ContainerState?)Activator.CreateInstance(matchingType, parent);
                if (_executableStates.TryGetValue(matchingType, out var canExecute))
                {
                    if (canExecute())
                    {
                        return state;
                    }
                    else
                    {
                        continue;
                    }
                }
                return state;
            }
            return null;
        }

        public static void RegisterCanvasState<T>(InputGesture inputGesture, Func<bool>? canExecute = null) where T : CanvasState
        {
            _stateTypeByGesture[typeof(T)] = inputGesture;
            if (canExecute != null)
            {
                _executableStates[typeof(T)] = canExecute;
            }
        }

        public static void RegisterContainerState<T>(InputGesture inputGesture, Func<bool>? canExecute = null) where T : ContainerState
        {
            _stateTypeByGesture[typeof(T)] = inputGesture;
            if (canExecute != null)
            {
                _executableStates[typeof(T)] = canExecute;
            }
        }
    }
}
