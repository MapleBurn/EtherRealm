using Godot;
using System;

public partial class Testenemy : CharacterBody2D
{
    [Export] private Area2D hurtbox;
    [Export] private Healthbar healthbar;
    
    public int maxHealth = 100;
    public int health;
    private Random rdm = new Random();

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
        
        Velocity = velocity;
        MoveAndSlide();
    }

    public void HurtboxAreaEntered(Area2D area)
    {
        if (!area.IsInGroup("weapons"))
            return;
        
        Weapon weapon = area.GetParent<Weapon>();
        int damage = weapon.damage;
        int critChance = weapon.critChance;
        bool isCrit = false;
        
        if (rdm.Next(0, 100) < critChance) //chance for a critical hit
        {
            damage *= 2; 
            isCrit = true;
        }
        health -= damage;
        
        //spawn damage floating text
        SpawnDFT(isCrit, damage);
        
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
