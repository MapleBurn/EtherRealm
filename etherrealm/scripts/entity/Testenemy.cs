using Godot;
using System;

public partial class Testenemy : Enemy
{
    [Export] private Area2D _hurtbox;
    [Export] private Healthbar _healthbar;
    
    public const int _maxHealth = 1500;
    private float _acceleration = 600.0f;
    private float _friction = 600.0f;
    private const float _maxSpeed = 150.0f;
    
    //Enemy attack stats
    private float _damage = 20f;
    private float _critChance = 10f; // chance in percent to do critical hit
    private float _critDmgMult = 2f;
    private float _knockback = 200f;
    private float _critKBMult = 5f;

    public override void _Ready()
    {
        maxHealth = _maxHealth;
        health = maxHealth;
        healthbar = _healthbar;
        healthbar.Initialize(maxHealth); 
        healthbar.Visible = false;
        hurtbox = _hurtbox;
        acceleration = _acceleration;
        friction = _friction;
        maxSpeed = _maxSpeed;
        
        //Enemy attack stats
        damage = _damage;
        critChance = _critChance;
        critDmgMult = _critDmgMult;
        knockback = _knockback;
        critKBMult = _critKBMult;
    }
}
