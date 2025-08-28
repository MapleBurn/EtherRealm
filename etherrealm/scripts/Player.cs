using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public float acceleration = 600.0f;
	public float friction = 1000.0f;
	public const float maxSpeed = 200.0f;
	public const float JumpVelocity = -400.0f;
	

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}
		
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		float targetX = direction.X * maxSpeed;
		
		if (direction != Vector2.Zero)
		{
			//Accelerate to target speed
			velocity.X = Mathf.MoveToward(Velocity.X, targetX, acceleration * (float)delta);
		}
		else
		{
			//slow down when no input
			velocity.X = Mathf.MoveToward(Velocity.X, 0, friction * (float)delta);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
