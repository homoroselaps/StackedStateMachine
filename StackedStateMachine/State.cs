using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace StackedStateMachine
{
    public class State
    {
        public virtual IEvent onActivate(AbortEvent e) { return e; }
        public virtual IEvent onActivate(DoneEvent e) { return null; }
        public virtual IEvent onActivate(IEvent e) { return null; }

        public virtual IEvent onRecieveEvent(IEvent e) { return null; }

        public virtual void onDeactivate(DoneEvent e) { /* do some cleanup */ }
        public virtual void onDeactivate(AbortEvent e) { /* do some cleanup */ }
        public virtual void onDeactivate(IEvent e) { }
    }
}
