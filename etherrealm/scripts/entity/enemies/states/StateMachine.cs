using Godot;
using System;
using Godot.Collections;

namespace EtherRealm.scripts.entity.enemies.states;

public partial class StateMachine : Node
{
    [Export] private State inicialState;
    public State currentState;
    private Enemy enemy;
    private Dictionary<string, State> states = new Dictionary<string, State>();

    public void Init(Enemy e)
    {
        enemy = e;
        
        foreach (var child in GetChildren())
        {
            if (child is State state)
            {
                states[state.Name] = state;
                state.Connect(State.SignalName.StateChanged, Callable.From<State, string>(StateChanged));
            }
        }

        if (inicialState != null)
        {
            inicialState.Enter();
            currentState = inicialState;
        }
    }

    public override void _Process(double delta)
    {
        if (currentState != null && !enemy.isDead)
            currentState.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (currentState != null && !enemy.isDead)
            currentState.PhysicsUpdate(delta);
    }

    public void StateChanged(State state, string newStateName)
    {
        if (state != currentState)
        {
            return;
        }
        State newState = states[newStateName];
        if (newState == null)
        {
            return;
        }

        if (currentState != null)
        {
            currentState.Exit();
        }
        
        newState.Enter();
        currentState = newState;
    }
}