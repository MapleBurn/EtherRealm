using Godot;
using System;

public partial class ChaseState : State
{
    [Export] private Enemy enemy;
    
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
        acceleration = enemy.acceleration;
        friction = enemy.friction;
        maxSpeed = enemy.maxSpeed;
        velocity = enemy.Velocity;
    }

    public override void Update(double delta)
    {
        enemy.hitDir = (enemy.player.GlobalPosition - enemy.GlobalPosition).Normalized();
    }

    public override void PhysicsUpdate(double delta)
    {
        direction = (enemy.player.Position - enemy.Position).Normalized();
        velocity = enemy.Velocity;
        float targetX = direction.X * maxSpeed;
        //Accelerate to target speed
        velocity.X = Mathf.MoveToward(velocity.X, targetX, acceleration * (float)delta);
        
        
        enemy.Velocity = velocity;
        Vector2 prevV = velocity;
        enemy.MoveAndSlide();
        
        enemy.ApplyImpactDamage(prevV); 
    }
    
    public override void Exit()
    {
        
    }
}