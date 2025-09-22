using Godot;
using System;

public partial class Entity : CharacterBody2D
{
    protected AnimatedSprite2D sprite;
    protected Area2D hurtbox;
    protected static Random rdm = new Random();
    protected Healthbar healthbar;

    public int maxHealth;
    public int health;
    public bool isDead;
    
    public float acceleration;
    public float friction;
    public float maxSpeed;
    public float jumpVelocity;


    protected virtual void HurtboxAreaEntered(Area2D area) { }
    
    #region Combat
    protected virtual void ProcessDamage(float damage, bool isCrit)
    {
        health -= (int)damage;
        SpawnDFT(isCrit, (int)damage, true);
    }
    
    protected void ProcessKnockback(float knockback, Vector2 hitDir)
    {
        Velocity = hitDir * knockback;
    }
    
    protected void SpawnDFT(bool isCrit, int damage, bool isPlayer)
    {
        PackedScene damageTextScene = GD.Load<PackedScene>("res://scenes/floating_text.tscn");
        FloatingText damageText = damageTextScene.Instantiate<FloatingText>();
        GetParent().AddChild(damageText);
        
        if (isCrit)
            damageText.SetDamage(damage, FloatingText.DamageType.crit, isPlayer);
        else
            damageText.SetDamage(damage, FloatingText.DamageType.damage, isPlayer);
        damageText.GlobalPosition = GlobalPosition + new Vector2(GD.RandRange(-20, 20), GD.RandRange(-10, -30));
    }
    #endregion

    protected virtual void FallDamage()
    {
        //fall damage
    }
    
    protected virtual void ApplyHealing(int healAmount)
    {
        if (health + healAmount >= maxHealth)
        { 
            health = maxHealth;
            return;
        }
        health += healAmount;
        healthbar.UpdateHealthbar(health);
    }
    
    protected virtual void Die()
    {
        isDead = true;
    }
}
