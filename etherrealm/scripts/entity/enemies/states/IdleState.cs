using System;
using Godot;

namespace EtherRealm.Enemies.States;

public partial class IdleState : State
{
    [Export] private Enemy enemy;
    [Export] private Resource stats;
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
    private bool isWalking;
    public override void Enter()
    {
        waitTime = rdm.NextDouble() * 1000;
        timer = Time.GetTicksMsec();
        
        acceleration = enemy.acceleration;
        friction = enemy.friction;
        maxSpeed = enemy.maxSpeed;
        velocity = enemy.Velocity;
        
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
        if (timer + waitTime < Time.GetTicksMsec())
        {
            ChooseWalkDir();
            timer = Time.GetTicksMsec();
            waitTime = rdm.NextDouble() * 1000;
        }
        
        velocity = enemy.Velocity;
        float targetX = direction.X * maxSpeed;
        if (isWalking)
        {
            //Accelerate to target speed
            velocity.X = Mathf.MoveToward(velocity.X, targetX, acceleration * (float)delta);
        }
        else
        {
            //slow down when no direction is given
            velocity.X = Mathf.MoveToward(velocity.X, 0, friction * (float)delta);
        }
        
        enemy.Velocity = velocity;
        enemy.MoveAndSlide();
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
        isWalking = true;
    }
}