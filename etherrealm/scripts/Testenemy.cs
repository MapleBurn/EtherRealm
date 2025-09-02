using Godot;
using System;

public partial class Testenemy : CharacterBody2D
{
    [Export] private Area2D hurtbox;
    
    public int maxHealth = 100;
    public int health;

    public override void _Ready()
    {
        health = maxHealth;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (health <= 0)
            Dispose();
        
        Vector2 velocity = Velocity;
        
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }
        
        Velocity = velocity;
        MoveAndSlide();
    }

    private void HurtboxAreaEntered(Area2D area)
    {
        Weapon weapon = area.GetParent<Weapon>();
        health -= weapon.damage;
    }
}
