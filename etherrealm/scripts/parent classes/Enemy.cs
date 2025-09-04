using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    public Area2D hurtbox;
    public Healthbar healthbar;
    
    public int maxHealth;
    public int health;
    public Random rdm = new Random();
    
    public float acceleration;
    public float friction;
    public float maxSpeed;
    public bool isWalking = false;
    
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
        float critDmgMult = weapon.critDmgMult;
        float critChance = weapon.critChance;
        bool isCrit = false;
        Vector2 hitDir = weapon.hitDir;
        float knockback = weapon.knockback;
        float critKBMult = weapon.critKBMult;
        
        if (rdm.Next(0, 100) < critChance) //chance for a critical hit
        {
            damage *= critDmgMult; 
            isCrit = true;
            knockback *= critKBMult;
        }
        ProcessDamage(damage, isCrit);
        ProcessKnockback(knockback, hitDir);
    }

    public virtual void ProcessDamage(float damage, bool isCrit)
    {
        health -= (int)damage;
        //spawn damage floating text
        SpawnDFT(isCrit, (int)damage);
        
        //update healthbar
        healthbar.Visible = true;
        healthbar.UpdateHealthbar(health);
    }

    public virtual void ProcessKnockback(float knockback, Vector2 hitDir)
    {
        Velocity += hitDir * knockback;
    }
    
    public void SpawnDFT(bool isCrit, int damage)
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
    public virtual void Die()
    {
        QueueFree();
    }
}
