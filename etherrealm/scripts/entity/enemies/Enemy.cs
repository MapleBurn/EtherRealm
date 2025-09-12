using Godot;
using System;

public partial class Enemy : Entity
{
    public Player player;
    protected StateMachine stateMachine;
    protected Healthbar healthbar;
    
    //Enemy attack stats
    public float damage;
    public float critChance; // chance in percent to do critical hit
    public float critDmgMult;
    public float knockback;
    public float critKnockMult;
    public Vector2 hitDir;
    public bool isChasing;

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
    
    protected override void HurtboxAreaEntered(Area2D area)
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
    
    protected void DetectionAreaPlayerEntered(Area2D area)
     {
        if (!area.IsInGroup("player"))
            return;
        
        player = area.GetParent<Player>();
        isChasing = true;
    }
    
    #endregion

    public override void ProcessDamage(float damage, bool isCrit)
    {
        health -= (int)damage;
        //spawn damage floating text
        SpawnDFT(isCrit, (int)damage, false);
        
        //update healthbar
        healthbar.Visible = true;
        healthbar.UpdateHealthbar(health);
    }
    
    public override void Die()
    {
        QueueFree();
    }
}
