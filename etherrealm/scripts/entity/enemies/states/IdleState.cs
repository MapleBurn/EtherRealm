using System;
using Godot;

namespace EtherRealm.scripts.entity.enemies.states;

public partial class IdleState : State
{
    [Export] private Enemy enemy;
    private Random rdm = new Random();
    private ulong timer;
    private double waitTime; //time to wait before choosing new destination
    
    //movement
    private float walkDir;
    private Vector2 velocity;
    private Vector2 direction;
    private float acceleration;
    private float friction;
    private float maxSpeed;
    public override void Enter()
    {
        waitTime = rdm.NextDouble() * 2000;
        timer = Time.GetTicksMsec();
        
        acceleration = enemy.acceleration;
        friction = enemy.friction;
        maxSpeed = enemy.maxSpeed;
        velocity = enemy.Velocity;
    }

    public override void Update(double delta)
    {
        
    }

    public override void PhysicsUpdate(double delta)
    {
        velocity = enemy.Velocity;
        float targetX = direction.X * maxSpeed; 
        //slow down when no direction is given
        velocity.X = Mathf.MoveToward(velocity.X, 0, friction * (float)delta);
        
        enemy.Velocity = velocity;
        enemy.MoveAndSlide();
        
        //temporary idle to wander transition
        if (timer + waitTime < Time.GetTicksMsec())
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "WanderState");
        }
        else if (enemy.isChasing)
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "ChaseState");
        }
    }
    
    public override void Exit()
    {
        
    }
}