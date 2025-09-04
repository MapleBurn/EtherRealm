using Godot;
using System;

public partial class Testenemy : Enemy
{
    [Export] private Area2D _hurtbox;
    [Export] private Healthbar _healthbar;
    
    public const int _maxHealth = 500;
    private float _acceleration = 600.0f;
    private float _friction = 600.0f;
    private const float _maxSpeed = 150.0f;

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
    }
}
