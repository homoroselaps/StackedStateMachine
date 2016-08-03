using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackedStateMachine
{
    struct Point
    {
        float x, y;
        public Point(float x, float y) {
            this.x = x;
            this.y = y;
        }
    }
    class DebugState: State
    {
        private void printDebug(string funcName, IEvent e) {
            Console.WriteLine($"{GetType().ToString(),10}.{funcName}({e?.GetType().ToString()})");
        }
        public override IEvent onActivate(IEvent e) {
            printDebug("onActivate", e);
            return base.onActivate(e);
        }
        public override IEvent onActivate(AbortEvent e) {
            printDebug("onActivate", e);
            return base.onActivate(e);
        }
        public override IEvent onActivate(DoneEvent e) {
            printDebug("onActivate", e);
            return base.onActivate(e);
        }

        public override IEvent onRecieveEvent(IEvent e) {
            printDebug("onRecieveEvent", e);
            return base.onRecieveEvent(e);
        }

        public override void onDeactivate(IEvent e) {
            printDebug("onDeactivate", e);
            base.onDeactivate(e);
        }
        public override void onDeactivate(AbortEvent e) {
            printDebug("onDeactivate", e);
            base.onDeactivate(e);
        }
        public override void onDeactivate(DoneEvent e) {
            printDebug("onDeactivate", e);
            base.onDeactivate(e);
        }
    }
    class DummyState: DebugState
    {
        private int counter { get; set; }
        public DummyState(int counter) {
            this.counter = counter;
        }
        public IEvent onRecieveEvent(TimerEvent e) {
            base.onRecieveEvent(e);
            counter--;
            if (counter <= 0) return new DoneEvent();
            return null;
        }
    }

    class PathingEvent: IEvent
    {
        public Point target;
    }
    class PathingState : DummyState
    {
        public PathingState() : base(3) { }
    }

    class DropEvent: IEvent { }
    class DropState: DummyState
    {
        public DropState(): base(1) { }
    }

    class PickEvent : IEvent { }
    class PickState: DummyState
    {
        public PickState() : base(1) { }
    }

    class CarryEvent: IEvent
    {
        public Point from;
        public Point to;
    }
    class CarryState : DebugState
    {
        public CarryState() { }

        private int stepCounter;
        private Point from, to;
        private IEvent controlAction() {
            switch (stepCounter) {
                case 0:
                    stepCounter++;
                    return new PathingEvent() { target = from };
                case 1:
                    stepCounter++;
                    return new PickEvent();
                case 2:
                    stepCounter++;
                    return new PathingEvent() { target = to };
                case 3:
                    stepCounter++;
                    return new DropEvent();
                default:
                    return new DoneEvent();
            }
        }
        public IEvent onActivate(CarryEvent e) {
            base.onActivate(e);
            from = e.from;
            to = e.to;
            return controlAction();            
        }
        public override IEvent onActivate(DoneEvent e) {
            base.onActivate(e);
            return controlAction();
        }
    }
    class IdleState: DebugState
    {
        public IdleState() { }
        public override IEvent onActivate(AbortEvent e) {
            base.onActivate(e);
            return null;
        }
    }

    class Program
    {
        static void Main(string[] args) {
            Console.WriteLine("Welome to the StackedStateMachine Testing Environment");
            Console.WriteLine("Write 'exit' to end the program");
            Console.WriteLine("Write 'carry' to send a CarryEvent");
            Console.WriteLine("Write 'abort' to send an AbortEvent");
            Console.WriteLine("Press 'enter' to step forward");

            // setup state machine
            StackedStateMachine ssm = new StackedStateMachine();
            ssm.addTransition(null, typeof(TimerEvent), () => { return new IdleState(); });
            ssm.addTransition(null, typeof(CarryEvent), () => { return new CarryState(); });
            ssm.addTransition(typeof(IdleState), typeof(CarryEvent), () => { return new CarryState(); });
            ssm.addTransition(typeof(CarryState), typeof(DropEvent), () => { return new DropState(); });
            ssm.addTransition(typeof(CarryState), typeof(PathingEvent), () => { return new PathingState(); });
            ssm.addTransition(typeof(CarryState), typeof(PickEvent), () => { return new PickState(); });
            
            // run the state machine
            while (true) {
                string input = Console.ReadLine().ToLowerInvariant();
                if (input == "exit")
                    break;
                else if (input == "carry")
                    ssm.raiseEvent(new CarryEvent() { from = new Point(5, 5), to = new Point(8, 8) });
                else if (input == "abort")
                    ssm.raiseEvent(new AbortEvent());
                else if (input == "") {
                    ssm.raiseEvent(new TimerEvent());
                }
            }
        }
    }
}
