using Godot;
using System;
using EtherRealm.scripts.entity.animals;

namespace EtherRealm.scripts.entity.animals;

public partial class Capybara : Animal
{
	//stats
	[Export] private int _maxHealth;
	[Export] private float _acceleration;
	[Export] private float _friction;
	[Export] private float _maxSpeed;
	[Export] private float _jumpVelocity;
	[Export] private float _fallDamageThreshold;

	public override void _Ready()
	{
		//set stats
		maxHealth = _maxHealth;
		health = maxHealth;
		acceleration = _acceleration;
		friction = _friction;
		maxSpeed = _maxSpeed;
		jumpVelocity = _jumpVelocity;
		fallDamageThreshold = _fallDamageThreshold;
		
		//set other nodes
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		hurtParticles = GetNode<GpuParticles2D>("hurtParticles");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Walk(delta);
	}
}
