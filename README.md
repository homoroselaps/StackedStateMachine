# StackedStateMachine
A lightweight implementation for a stacked state machine in C#. It's designed to control any actions of an NPC in a game.
Part of the project is sample console application. It simulates the actions of a carrier with verbose output.

## Usage
create a new instance:
```c#
var ssm = new StackedStateMachine(<inital State>);
```
configure transitions:
```c#
ssm.addTransition(<typeof active State>, <typeof Event>, <delegate that returns the new state>);
```
execute active state's action and control the state machine with external events:
```c#
var e = ssm.State.onGameTick();
if (e != null) ssm.raiseEvent(e);
```

## Default Events

### AbortEvent:
The active state raises it, if it run into problems.
Raise it externally to abort the active action and to initiate rollback of the state stack.

### DoneEvent:
The active state raises it, if the job is done. Hands over to previous state on the stack.
