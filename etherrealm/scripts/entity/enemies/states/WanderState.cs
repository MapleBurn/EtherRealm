using System;
using Godot;

namespace EtherRealm.Enemies.States;

public partial class WanderState : State
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
    private bool isWalking = true;
    public override void Enter()
    {
        waitTime = rdm.NextDouble() * 10000;
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
        if (timer + waitTime < Time.GetTicksMsec())
        {
            if (rdm.Next(0, 100) < 15)
            {
                Exit();
                EmitSignal(State.SignalName.StateChanged, this, "IdleState");
            }
            ChooseWalkDir();
            timer = Time.GetTicksMsec();
            waitTime = rdm.NextDouble() * 1000;
        }
        
        velocity = enemy.Velocity;
        float targetX = direction.X * maxSpeed;
        //Accelerate to target speed
        velocity.X = Mathf.MoveToward(velocity.X, targetX, acceleration * (float)delta);
        
        
        enemy.Velocity = velocity;
        Vector2 prevV = velocity;
        enemy.MoveAndSlide();
        
        enemy.ApplyImpactDamage(prevV); 
        if (enemy.isChasing)
        {
            Exit();
            EmitSignal(State.SignalName.StateChanged, this, "ChaseState");
        }
    }
    
    public override void Exit()
    {
        
    }
    
    private void ChooseWalkDir()
    {
        bool isRight = rdm.Next(0, 100) < 50;
        if (isRight)
            direction = Vector2.Right;
        else
            direction = Vector2.Left;
    }
}