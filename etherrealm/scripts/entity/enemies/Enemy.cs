using Godot;
using System;
using EtherRealm.scripts.entity.enemies.states;
using EtherRealm.scripts.entity.itemEntities;

namespace EtherRealm.scripts.entity.enemies;
public partial class Enemy : Entity
{
    public Player player;
    protected StateMachine stateMachine;
    
    //Enemy attack stats
    public float damage;
    public float critChance; //chance in percent to do critical hit
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
        float wepDamage = weapon.damage;
        float wepCritDmgMult = weapon.critDmgMult;
        float wepCritChance = weapon.critChance;
        bool isCrit = false;
        Vector2 wepHitDir = weapon.hitDir;
        float wepKnockback = weapon.knockback;
        float wepCritKBMult = weapon.critKbMult;
        
        //attack type damage scaling
        if (weapon.actionType == "stab")
        {
            wepDamage *= 1.0f;
            wepKnockback *= 0.8f;
        }
        else if (weapon.actionType == "swing")
        {
            wepDamage *= 1.5f;
            wepKnockback *= 1.2f;
        }

        if (rdm.Next(0, 100) < wepCritChance) //chance for a critical hit
        {
            wepDamage *= wepCritDmgMult; 
            isCrit = true;
            wepKnockback *= wepCritKBMult;
        }
        ProcessDamage(wepDamage, isCrit);
        ProcessKnockback(wepKnockback, wepHitDir);
    }
    
    protected void DetectionAreaPlayerEntered(Area2D area)
     {
        if (!area.IsInGroup("player"))
            return;
        
        player = area.GetParent<Player>();
        isChasing = true;
    }
    
    #endregion

    protected override void ProcessDamage(float damage, bool isCrit)
    {
        health -= (int)damage;
        //spawn damage floating text
        SpawnDFT(isCrit, (int)damage, false);
        
        //update healthbar
        healthbar.Visible = true;
        healthbar.UpdateHealthbar(health);
    }
    
    protected override void Die()
    {
        QueueFree();
    }
}
