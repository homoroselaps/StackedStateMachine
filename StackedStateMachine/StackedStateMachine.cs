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
        public StackedStateMachine(State stateStart) {
            stateStack.Push(stateStart);
            stateStart.onActivate((IEvent)null);
        }
        public IState State { get { return stateStack.Peek(); } }
        public void addTransition<stateType1, eventType>(stateType1 state1, eventType e, StateContructor stateConstructor)
            where stateType1 : Type
            where eventType : Type {
            transitions.Add(new Tuple<Type, Type>(state1, e), stateConstructor);
        }

        private IEvent handleEvent(IEvent e) {
            Debug.Assert(e != null);
            dynamic state = stateStack.Peek();
            if (state == null)
                //The state machine has no active state anymore
                return null;
            var stateType = state.GetType();
            var eventType = e.GetType();
            if (e is AbortEvent) {
                state?.onDeactivate(new AbortEvent());
                stateStack.Pop();
                dynamic newState = stateStack.Peek();
                return newState?.onActivate(new AbortEvent());
            }
            else if (e is DoneEvent) {
                state?.onDeactivate(new DoneEvent());
                stateStack.Pop();
                dynamic newState = stateStack.Peek();
                return newState?.onActivate(new DoneEvent());
            }
            else if (transitions.ContainsKey(new Tuple<Type, Type>(stateType, eventType))) {
                var constr = transitions[new Tuple<Type, Type>(stateType, eventType)];
                dynamic ev = e;
                state?.onDeactivate(ev);
                dynamic newState = constr();
                stateStack.Push(newState);
                return newState.onActivate(ev);
            }
            // the state machine is not valid
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
