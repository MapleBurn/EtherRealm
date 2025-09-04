using Godot;
using System;

public partial class Testenemy : CharacterBody2D
{
    [Export] private Area2D hurtbox;
    [Export] private Healthbar healthbar;
    
    public const int maxHealth = 500;
    public int health;
    private Random rdm = new Random();
    
    private float acceleration = 600.0f;
    private float friction = 600.0f;
    private const float maxSpeed = 150.0f;
    private bool isWalking = false;

    public override void _Ready()
    {
        health = maxHealth;
        healthbar.Visible = false;
        healthbar.Initialize(maxHealth);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (health <= 0)
        {
            Die();
        }
        
        
        Vector2 velocity = Velocity;
        
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }
        
        Vector2 direction = velocity.Normalized();
        float targetX = direction.X * maxSpeed;
        if (isWalking)
        {
            //Accelerate to target speed
            velocity.X = Mathf.MoveToward(Velocity.X, targetX, acceleration * (float)delta);
        }
        else
        {
            //slow down when no input
            velocity.X = Mathf.MoveToward(Velocity.X, 0, friction * (float)delta);
        }
        
        Velocity = velocity;
        MoveAndSlide();
    }

    public void HurtboxAreaEntered(Area2D area)
    {
        if (!area.IsInGroup("weapons"))
            return;
        
        Weapon weapon = area.GetParent<Weapon>();
        float damage = weapon.damage;
        float critChance = weapon.critChance;
        float knockback = weapon.knockback;
        bool isCrit = false;
        
        if (rdm.Next(0, 100) < critChance) //chance for a critical hit
        {
            damage *= 2; 
            isCrit = true;
            knockback *= 1.5f;
        }
        health -= (int)damage;
        Velocity += weapon.hitDir * knockback;
        
        //spawn damage floating text
        SpawnDFT(isCrit, (int)damage);
        
        //update healthbar
        healthbar.Visible = true;
        healthbar.UpdateHealthbar(health);
    }

    private void SpawnDFT(bool isCrit, int damage)
    {
        PackedScene damageTextScene = GD.Load<PackedScene>("res://scenes/floating_text.tscn");
        FloatingText damageText = damageTextScene.Instantiate<FloatingText>();
        GetParent().AddChild(damageText);
        
        if (isCrit)
            damageText.SetDamage(damage, FloatingText.DamageType.crit);
        else
            damageText.SetDamage(damage, FloatingText.DamageType.damage);
        damageText.GlobalPosition = GlobalPosition + new Vector2(GD.RandRange(-20, 20), GD.RandRange(-10, -30));
    }
    
    public void Die()
    {
        QueueFree();
    }
}
