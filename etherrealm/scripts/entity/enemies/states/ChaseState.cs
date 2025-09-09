using Godot;
using System;

public partial class ChaseState : State
{
    private Enemy enemy;
    public override void Enter()
    {
        //if (enemy.isOnWater)
        //{
        //    Exit();
        //    EmitSignal(State.SignalName.StateChanged, this, "swimState");
        //}
    }

    public override void Update(double delta)
    {
        
    }

    public override void PhysicsUpdate(double delta)
    {
        
    }
    
    public override void Exit()
    {
        
    }
}