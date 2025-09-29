using Godot;
using System;

public partial class Swordtest : Weapon
{
    //[Export] private Player _player { get; set; }
    [Export] private Area2D _hitbox { get; set; }
    [Export] private AnimationPlayer _animPlayer { get; set; }
    
    //this weapon's properties
    [Export] private float _attackDistance = 10f;
    [Export] private float _attackDamage = 10f;
    [Export] private float _critChance = 15f; //percentage chance to crit
    [Export] private float _critDmgMult = 2f;
    [Export] private float _critKBMult = 1.5f;
    [Export] private float _knockback = 200f;
    [Export] private float _delay = 300f;

    public override void _Ready()
    {
        damage = _attackDamage;
        critChance = _critChance;
        critDmgMult = _critDmgMult;
        knockback = _knockback;
        critKBMult =  _critKBMult;
        hitbox = _hitbox;
        attackCollider = GetNode<CollisionPolygon2D>("hitbox/collider");
        stabDistance = _attackDistance;
        delay = _delay;
        //player = _player;
        animPlayer = _animPlayer;
    }
}
