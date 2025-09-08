using Godot;
using System;

public partial class Swordtest : Weapon
{
    [Export] private Player _player { get; set; }
    [Export] private Sprite2D sprite { get; set; }
    [Export] private Area2D _hitbox { get; set; }
    
    //this weapons properties
    private float _attackDistance = 10f;
    private float _attackDamage = 10f;
    private float _critChance = 15f; //percentage chance to crit
    private float _critDmgMult = 2f;
    private float _critKBMult = 1.5f;
    private float _knockback = 100f;
    private float _delay = 300f;

    public override void _Ready()
    {
        damage = _attackDamage;
        critChance = _critChance;
        critDmgMult = _critDmgMult;
        knockback = _knockback;
        critKBMult =  _critKBMult;
        hitbox = _hitbox;
        attackCollider = GetNode<CollisionShape2D>("hitbox/collider");
        stabDistance = _attackDistance;
        delay = _delay;
        player = _player;
    }
}
