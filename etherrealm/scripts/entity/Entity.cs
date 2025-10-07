using Godot;
using System;

public partial class Entity : CharacterBody2D
{
    protected AnimationPlayer animPlayer;
    protected Area2D hurtbox;
    protected static Random rdm = new Random();
    protected Healthbar healthbar;

    //combat properties
    public int maxHealth;
    public int health;
    public bool isDead;
    protected bool isAttacking;
    
    public float acceleration;
    public float friction;
    public float maxSpeed;
    public float jumpVelocity;
    public float fallDamageThreshold;

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
        PackedScene damageTextScene = GD.Load<PackedScene>("res://scenes/UI/floating_text.tscn");
        FloatingText damageText = damageTextScene.Instantiate<FloatingText>();
        GetParent().AddChild(damageText);
        
        if (isCrit)
            damageText.SetDamage(damage, FloatingText.DamageType.crit, isPlayer);
        else
            damageText.SetDamage(damage, FloatingText.DamageType.damage, isPlayer);
        damageText.GlobalPosition = GlobalPosition + new Vector2(GD.RandRange(-20, 20), GD.RandRange(-10, -30));
    }
    #endregion

    public void ApplyImpactDamage(Vector2 prevVelocity)
    {
        int slideCount = GetSlideCollisionCount();
        if (slideCount <= 0)
            return;

        //we’ll get the worst (max) impact this frame to avoid double-damaging on multiple contacts.
        float maxImpactCollision = 0f;

        for (int i = 0; i < slideCount; i++)
        {
            var col = GetSlideCollision(i);
            if (col == null) continue;

            if (!IsSolidCollider(col.GetCollider()))
                continue;

            //collision normal points out of the collider (into us), e.g. floor is (0, -1), wall is (±1, 0), ceiling is (0, 1).
            Vector2 n = col.GetNormal().Normalized();

            // Impact speed along the normal uses our pre-impact velocity.
            // Positive means we were moving INTO the surface.
            float impactSpeed = Mathf.Abs(prevVelocity.Dot(n));

            if (impactSpeed > maxImpactCollision)
                maxImpactCollision = impactSpeed;
        }

        if (maxImpactCollision > fallDamageThreshold)
        {
            int damage = ComputeDamage(maxImpactCollision - fallDamageThreshold, 0.05f);
            if (damage > 0)
                ProcessDamage(damage, false);
        }
    }

    private bool IsSolidCollider(object collider)
    {
        if (collider is Node node)
        {
            //treat tilemap and StaticBody2D as solid
            if (node is TileMapLayer) return true;
            if (node is StaticBody2D) return true;
        }
        return false;
    }

    private int ComputeDamage(float overSpeed, float scale)
    {
        var raw = overSpeed * scale;
        return Mathf.Clamp(Mathf.RoundToInt(raw), 1, health); //makes sure that it's not more than health
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
