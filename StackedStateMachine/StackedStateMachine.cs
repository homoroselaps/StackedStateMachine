using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackedStateMachine
{
    public delegate State StateContructor();
    public class StackedStateMachine
    {
        Stack<State> stateStack = new Stack<State>();
        Dictionary<Tuple<Type, Type>, StateContructor> transitions = new Dictionary<Tuple<Type, Type>, StateContructor>();

        public StackedStateMachine() { }
        public StackedStateMachine(State startState) {
                stateStack.Push(startState);
                startState.onActivate((IEvent)null);
        }
        public State State {
            get {
                if (stateStack.Count > 0)
                    return stateStack.Peek();
                return null;
            }
        }

        public void addTransition(Type state, Type e, StateContructor stateConstructor) {
            transitions.Add(new Tuple<Type, Type>(state, e), stateConstructor);
        }

        private IEvent handleEvent(IEvent e) {
            Debug.Assert(e != null);
            dynamic state = State;
            var stateType = state?.GetType();
            var eventType = e.GetType();
            if (eventType == typeof(AbortEvent) || eventType == typeof(DoneEvent)) { 
                state?.onDeactivate(e);
                stateStack.Pop();
                dynamic newState = State;
                return newState?.onActivate((dynamic)e);
            }
            else if (transitions.ContainsKey(new Tuple<Type, Type>(stateType, eventType))) {
                state?.onDeactivate((dynamic)e);
                var nextState = transitions[new Tuple<Type, Type>(stateType, eventType)]();
                Debug.Assert(nextState != null);
                stateStack.Push(nextState);
                dynamic newState = State;
                return newState?.onActivate((dynamic)e);
            }
            else {
                return state?.onRecieveEvent((dynamic)e);
            }
        }
        public void raiseEvent(IEvent e) {
            while (e != null) {
                e = handleEvent(e);
            }
        }
    }
}
