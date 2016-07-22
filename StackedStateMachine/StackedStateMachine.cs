using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StackedStateMachine
{
    public delegate State StateContructor();
    public class StackedStateMachine
    {
        Stack<State> stateStack = new Stack<State>();
        Dictionary<Tuple<Type, Type>, StateContructor> transitions = new Dictionary<Tuple<Type, Type>, StateContructor>();
        public StackedStateMachine(State stateStart) {
            stateStack.Push(stateStart);
            stateStart.activateState(null);
        }
        public IState State { get { return stateStack.Peek(); } }
        public void addTransition<TState, TEvent>(TState state1, TEvent e, StateContructor stateConstructor)
            where TState : Type
            where TEvent : Type {
            transitions.Add(new Tuple<Type, Type>(state1, e), stateConstructor);
        }

        private IEvent handleEvent(IEvent e) {
            Debug.Assert(e != null);
            var state = stateStack.Peek();
            if (state == null)
                //The state machine has no active state anymore
                return null;
            var stateType = state.GetType();
            var eventType = e.GetType();
            if (e.GetType() == typeof(AbortEvent)) {
                state?.deactivateState(new AbortEvent());
                stateStack.Pop();
                var newState = stateStack.Peek();
                return newState?.activateState(new AbortEvent());
            }
            else if (e.GetType() == typeof(DoneEvent)) {
                state?.deactivateState(new DoneEvent());
                stateStack.Pop();
                var newState = stateStack.Peek();
                return newState?.activateState(new DoneEvent());
            }
            else if (transitions.ContainsKey(new Tuple<Type, Type>(stateType, eventType))) {
                var constr = transitions[new Tuple<Type, Type>(stateType, eventType)];
                state?.deactivateState(e);
                var newState = constr();
                stateStack.Push(newState);
                return newState.activateState(e);
            }
            // an event occured with no valid transition
            Debug.Assert(false);
            return null;
        }
        public void raiseEvent(IEvent e) {
            while (e != null) {
                e = handleEvent(e);
            }
        }
    }
}
