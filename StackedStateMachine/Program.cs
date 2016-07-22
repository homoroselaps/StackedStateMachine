﻿using System;
using System.Collections.Generic;

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
        public override IEvent onGameTick() {
            printDebug("onGameTick", null);
            return base.onGameTick();
        }
        public override IEvent activateState(IEvent e) {
            printDebug("onActivate", e);
            return base.activateState(e);
        }
        public override void deactivateState(IEvent e) {
            printDebug("onDeactivate", e);
            base.deactivateState(e);
        }
    }
    class DummyState: DebugState
    {
        private int counter { get; set; }
        public DummyState(int counter) {
            this.counter = counter;
        }
        override public IEvent onGameTick() {
            base.onGameTick();
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
        public CarryState() {
            addOnActivateHandler(typeof(CarryEvent), (IEvent e) => { return onActivate((CarryEvent)e); });
        }

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
            from = e.from;
            to = e.to;
            return controlAction();            
        }
        protected override IEvent onActivate(DoneEvent e) {
            return controlAction();
        }
    }
    class IdleState: DebugState
    {
        public IdleState() { }
        protected override IEvent onActivate(AbortEvent e) {
            return null;
        }
    }

    class Program
    {
        static State createCS() {
            return new CarryState();
        }
        static void Main(string[] args) {
            Console.WriteLine("Welome to the StackedStateMachine Testing Environment");
            Console.WriteLine("Write 'exit' to end the program");
            Console.WriteLine("Write 'carry' to send a CarryEvent");
            Console.WriteLine("Write 'abort' to send an AbortEvent");
            Console.WriteLine("Press 'enter' to step forward");
            
            // setup state machine
            StackedStateMachine ssm = new StackedStateMachine(new IdleState());
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
                    var e = ssm.State.onGameTick();
                    if (e != null) ssm.raiseEvent(e);
                }
            }
        }
    }
}
