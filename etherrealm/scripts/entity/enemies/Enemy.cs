using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    public Area2D hurtbox;
    public Healthbar healthbar;
    public Player player;
    public StateMachine stateMachine;
    
    public int maxHealth;
    public int health;
    public Random rdm = new Random();
    public bool isDead;
    
    //Enemy attack stats
    public float damage;
    public float critChance; // chance in percent to do critical hit
    public float critDmgMult;
    public float knockback;
    public float critKnockMult;
    public Vector2 hitDir;
    public bool isChasing;
    
    //movement stats
    public float acceleration;
    public float friction;
    public float maxSpeed;

    public override void _Ready()
    {
        //calls the Init method of the state machine to set up states only after enemy is loaded
        stateMachine.Init(this);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (health <= 0)
        {
            Die();
        }
        
        if (!IsOnFloor())
        {
            Velocity += GetGravity() * (float)delta;
        }
    }

    #region Signals
    
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
    
    public void DetectionAreaPlayerEntered(Area2D area)
     {
        if (!area.IsInGroup("player"))
            return;
        
        player = area.GetParent<Player>();
        isChasing = true;
    }
    
    #endregion

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
            damageText.SetDamage(damage, FloatingText.DamageType.crit, false);
        else
            damageText.SetDamage(damage, FloatingText.DamageType.damage, false);
        damageText.GlobalPosition = GlobalPosition + new Vector2(GD.RandRange(-20, 20), GD.RandRange(-10, -30));
    }
    
    public virtual void Die()
    {
        QueueFree();
    }
}
