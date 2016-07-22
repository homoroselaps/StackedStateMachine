using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace StackedStateMachine
{
    public interface IState
    {
        IEvent onGameTick();
    }
    public class State: IState
    {
        public State() {
            addOnActivateHandler(typeof(AbortEvent), (IEvent e) => { return onActivate((AbortEvent)e); });
            addOnActivateHandler(typeof(DoneEvent), (IEvent e) => { return onActivate((DoneEvent)e); });
        }
        protected delegate IEvent OnActivateHandler(IEvent e);
        protected delegate void OnDeactivateHandler(IEvent e);
        Dictionary<Type, OnActivateHandler> onActivateHandlers = new Dictionary<Type, OnActivateHandler>();
        Dictionary<Type, OnDeactivateHandler> onDeactivateHandlers = new Dictionary<Type, OnDeactivateHandler>();
        protected void addOnActivateHandler(Type eventType, OnActivateHandler eventHandler) {
            onActivateHandlers[eventType] = eventHandler;
        }
        protected void addOnDeactivateHandler(Type eventType, OnDeactivateHandler eventHandler) {
            onDeactivateHandlers[eventType] = eventHandler;
        }
        public virtual IEvent activateState(IEvent e) {
            if (e != null && onActivateHandlers.ContainsKey(e.GetType()))
                return onActivateHandlers[e.GetType()](e);
            else
                return onActivate();
        }
        public virtual void deactivateState(IEvent e) {
            if (e != null && onDeactivateHandlers.ContainsKey(e.GetType()))
                onDeactivateHandlers[e.GetType()](e);
            else
                onDeactivate();
        }
        public virtual IEvent onGameTick() { return null; }

        protected virtual IEvent onActivate() { return null; }
        protected virtual IEvent onActivate(AbortEvent e) { return e; }
        protected virtual IEvent onActivate(DoneEvent e) { return null; }
        protected virtual IEvent onDeactivate() { return null; }
        protected virtual void onDeactivate(AbortEvent e) { }
        protected virtual void onDeactivate(DoneEvent e) { }
    }
}
