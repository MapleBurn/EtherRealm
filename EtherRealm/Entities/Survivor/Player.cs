using Godot;

namespace EtherRealm.Entities.Survivor;

public partial class Player : LivingEntity
{
	[Export] private Weapon _weapon;
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}
		
		Vector2 direction = Input.GetVector("left", "right", "deadkey", "deadkey");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			if (direction < Vector2.Zero)
			{
				_weapon.Flip(true);
			}
			else
			{
				_weapon.Flip(false);
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	
	
}