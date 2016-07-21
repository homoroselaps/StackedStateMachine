# StackedStateMachine
A lightweight implementation for a stacked state machine in C#. It's designed to control any actions of an NPC in a game.
Part of the project is sample console application. It simulates the actions of a carrier with verbose output.

## Usage
Create a new instance:
```c#
var ssm = new StackedStateMachine(<inital State>);
```
Configure transitions:
```c#
ssm.addTransition(<typeof active State>, <typeof Event>, <delegate that returns the new state>);
```
Execute active state's action and control the state machine with external events:
```c#
var e = ssm.State.onGameTick();
if (e != null) ssm.raiseEvent(e);
```

## Default Events

### AbortEvent:
The active state raises it, if it runs into problems.
Raise it externally to abort the active action and to initiate rollback of the state stack.

### DoneEvent:
The active state raises it, if the action is done. Hands over control to the previous state on the state stack.
