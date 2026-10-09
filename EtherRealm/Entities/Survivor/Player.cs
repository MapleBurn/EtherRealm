using EtherRealm.Entities.Visual;
using Godot;

namespace EtherRealm.Entities.Survivor;

public partial class Player : LivingEntity
{
	[Export] private Weapon _weapon;
	[Export] private Sprite2D _sprite;

	private int _health;
	
	public override void _Ready()
	{
		_health = MaxHealth;
	}
	
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
			velocity.X = direction.X * MoveSpeed;
			if (direction < Vector2.Zero)
			{
				_weapon.Flip(true);
				_sprite.FlipH = true;
			}
			else
			{
				_weapon.Flip(false);
				_sprite.FlipH = false;
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, MoveSpeed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	
	public void TakeDamage(int amount)
	{
		_health -= amount;
		var scene = GD.Load<PackedScene>(FloatingTextPath);
		var text = scene.Instantiate<FloatingText>();
		AddChild(text);
		text.SetText(amount, FloatingText.DamageType.Damage, false);

		if (_health <= 0)
			return; // implement death
	}
}
