
namespace StackedStateMachine
{
    public interface IEvent { }
    public sealed class AbortEvent : IEvent { }
    public sealed class DoneEvent : IEvent { }
}
