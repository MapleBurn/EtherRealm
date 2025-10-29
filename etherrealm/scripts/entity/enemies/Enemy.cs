using Godot;
using System;
using EtherRealm.scripts.entity.enemies.states;
using EtherRealm.scripts.entity.itemEntities;
using EtherRealm.scripts.resource.entity;
using EtherRealm.scripts.UI;

namespace EtherRealm.scripts.entity.enemies;
public partial class Enemy : Entity
{
    //children and other nodes
    public Player player;
    [Export] private StateMachine stateMachine;
    [Export] private Area2D hurtbox;
    [Export] private Healthbar healthbar;
    private CollisionShape2D hurtboxCollider;
    private CollisionShape2D groundCollider;
    private Sprite2D sprite;
    private GpuParticles2D hurtParticles;
    
    [Export] private EnemyStats enemyStats;
    
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
        Initialize(enemyStats);
        SetChildNodes();
        //calls the Init method of the state machine to set up states only after enemy is loaded
        stateMachine.Init(this);
    }

    private void Initialize(EnemyStats eStats)
    {
        enemyStats = eStats;
        
        maxHealth = enemyStats.MaxHealth;
        acceleration = enemyStats.Acceleration;
        friction = enemyStats.Friction;
        maxSpeed = enemyStats.MaxSpeed;
        damage = enemyStats.Damage;
        critChance = enemyStats.CritChance;
        critDmgMult = enemyStats.CritDamageMult;
        knockback = enemyStats.Knockback;
        critKnockMult = enemyStats.CritKnockbackMult;
        fallDamageThreshold = enemyStats.FallDamageThreshold;
        
        health = maxHealth;
        healthbar.Initialize(maxHealth); 
        healthbar.Visible = false;
    }

    private void SetChildNodes()
    {
        groundCollider = GetNode<CollisionShape2D>("CollisionShape2D");
        hurtboxCollider = GetNode<CollisionShape2D>("hurtbox/collider");
        sprite = GetNode<Sprite2D>("Sprite2D");
        hurtParticles = GetNode<GpuParticles2D>("hurtParticles");

        groundCollider.Position = enemyStats.ColliderOffset;
        hurtboxCollider.Position = enemyStats.ColliderOffset;
        groundCollider.SetShape(enemyStats.ColliderShape);
        hurtboxCollider.SetShape(enemyStats.ColliderShape);
        sprite.Texture = enemyStats.Model;
        healthbar.Position = enemyStats.HealthBarOffset;
        healthbar.Size = enemyStats.HealthBarSize;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (health <= 0)
        {
            Die();
        }

        var velocity = Velocity;
        var direction = velocity.Normalized();

        
        if (!IsOnFloor())
        {
            Velocity += GetGravity() * (float)delta;
        }
        
        if (direction.X != 0)
        {
            sprite.FlipH = direction.X > 0;
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
        SpawnHurtParticles(wepHitDir);
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
    
    private void SpawnHurtParticles(Vector2 hitDir)
    {
        ParticleProcessMaterial material = (ParticleProcessMaterial)hurtParticles.ProcessMaterial;
        var dir = hitDir.Normalized();
        material.Direction = new Vector3(dir.X, dir.Y, 0);
        hurtParticles.Emitting = true;
    }
    
    protected override void Die()
    {
        QueueFree();
    }
}
