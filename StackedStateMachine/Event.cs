using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackedStateMachine
{
    public interface IEvent { }
    public sealed class AbortEvent : IEvent { }
    public sealed class DoneEvent : IEvent { }
    public class TimerEvent : IEvent { }
}
