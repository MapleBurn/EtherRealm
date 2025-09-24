using EtherRealm.Enemies.States;
using Godot;

namespace EtherRealm.Enemies;

public partial class TestEnemy : Enemy
{
    [Export] private Area2D _hurtbox;
    [Export] private Healthbar _healthbar;
    [Export] private TestEnemyStats testEnemyStats;
    [Export] private StateMachine _stateMachine;

    public override void _Ready()
    {
        maxHealth = testEnemyStats.maxHealth;
        health = maxHealth;
        healthbar = _healthbar;
        healthbar.Initialize(maxHealth); 
        healthbar.Visible = false;
        hurtbox = _hurtbox;
        acceleration = testEnemyStats.acceleration;
        friction = testEnemyStats.friction;
        maxSpeed = testEnemyStats.maxSpeed;
        damage = testEnemyStats.damage;
        critChance = testEnemyStats.critChance;
        critDmgMult = testEnemyStats.critDamageMult;
        knockback = testEnemyStats.knockback;
        critKnockMult = testEnemyStats.critKnockbackMult;
        stateMachine = _stateMachine;
        fallDamageThreshold = testEnemyStats.fallDamageThreshold;

        base._Ready();
    }
}