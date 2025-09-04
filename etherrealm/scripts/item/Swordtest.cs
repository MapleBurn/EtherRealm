using Godot;
using System;

public partial class Swordtest : Weapon
{
    [Export] private Sprite2D sprite { get; set; }
    [Export] private Area2D _hitbox { get; set; }
    
    //this weapons properties
    private float _attackDistance = 10f;
    private float _attackDamage = 10;
    private float _crit = 15; //percentage chance to crit
    private float _knockback = 10;

    public override void _Ready()
    {
        damage = _attackDamage;
        critChance = _crit;
        knockback = _knockback;
        hitbox = _hitbox;
        attackCollider = GetNode<CollisionShape2D>("hitbox/collider");
        stabDistance = _attackDistance;
    }
    
    public override void Attack()
    {
        Stab();
    }
}
