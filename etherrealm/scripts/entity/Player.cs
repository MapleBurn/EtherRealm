using Godot;
using System;

public partial class Player : Entity
{
	[Export] private AnimatedSprite2D _sprite;
	[Export] private Area2D _hurtbox;
	[Export] private TileMapLayer tilemap;
	[Export] private RayCast2D raycast;
	private Tween stepTween;
	
	//stepup stuff
	
	
	[Export] private float _acceleration = 600.0f;
	[Export] private float _friction = 800.0f;
	[Export] private float _maxSpeed = 150.0f;
	[Export] private float _JumpVelocity = -400.0f;
	[Export] private int _maxHealth = 100;
	private int dir = 1; //one means right

	public override void _Ready()
	{
		hurtbox = _hurtbox;
		sprite = _sprite;
		
		acceleration = _acceleration;
		friction =  _friction;
		maxSpeed = _maxSpeed;
		jumpVelocity = _JumpVelocity;
		maxHealth = _maxHealth;
		health = maxHealth;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (isDead)
			return;
		
		if (health <= 0)
		{
			Die();
		}
		
		Vector2 velocity = Velocity;

		// Add the gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = jumpVelocity;
		}
		
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		float targetX = direction.X * maxSpeed;
		
		if (direction != Vector2.Zero)
		{
			raycast.ForceRaycastUpdate();
			if (raycast.IsColliding())
			{
				Vector2 collisionPoint = raycast.GetCollisionPoint();
				
				Vector2 localPoint = tilemap.ToLocal(collisionPoint);
				Vector2I tileCoords = tilemap.LocalToMap(localPoint);

				if (CanStepUp(tileCoords))
				{
					//GlobalPosition += new Vector2(0, -10);
					//velocity -= (GetGravity() / 10) + new Vector2(dir * 50, 50);
					
					stepTween?.Kill();
					var targetPos = collisionPoint - new  Vector2(0, 1000);
					
					stepTween = CreateTween();
					stepTween.TweenProperty(this, "velocity", targetPos, 0.2f);
					//.SetTrans(Tween.TransitionType.Sine)  
					//.SetEase(Tween.EaseType.Out);
				}
			}
			
			//Accelerate to target speed
			velocity.X = Mathf.MoveToward(Velocity.X, targetX, acceleration * (float)delta);
			sprite.Play("walk");
			if (direction > Vector2.Zero)
			{
				dir = 1;
				DirChanged();
			}
			else
			{
				dir = -1;
				DirChanged();
			}
		}
		else
		{
			//slow down when no input
			velocity.X = Mathf.MoveToward(Velocity.X, 0, friction * (float)delta);
			sprite.Play("idle");
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	
	protected override void HurtboxAreaEntered(Area2D area)
	{
		if (!area.IsInGroup("enemies"))
			return;
        
		Enemy enemy = area.GetParent<Enemy>();
		float damage = enemy.damage;
		float critDmgMult = enemy.critDmgMult;
		float critChance = enemy.critChance;
		bool isCrit = false;
		Vector2 hitDir = enemy.hitDir;
		float knockback = enemy.knockback;
		float critKnockMult = enemy.critKnockMult;
		
		if (rdm.Next(0, 100) < critChance) //chance for a critical hit
		{
			damage *= critDmgMult; 
			isCrit = true;
			knockback *= critKnockMult;
		}
		ProcessDamage(damage, isCrit);
		ProcessKnockback(knockback, hitDir);
	}

	private bool CanStepUp(Vector2I tileCoords)
	{
		if (!IsOnFloor())
			return false;
		
		Godot.Collections.Array<Vector2I> tilecoords = new Godot.Collections.Array<Vector2I>();
		tilecoords.Add(tileCoords + new Vector2I(0, -1));
		tilecoords.Add(tileCoords + new Vector2I(0, -2));
		tilecoords.Add(tileCoords + new Vector2I(0, -3));
		tilecoords.Add(tileCoords + new Vector2I(-dir, -3));

		foreach (Vector2I coord in tilecoords)
		{
			Vector2I atlascoords = tilemap.GetCellAtlasCoords(coord);
			if (atlascoords != new Vector2I(-1, -1))
				return false;
		}
		return true;
	}
	
	public override void ProcessDamage(float damage, bool isCrit)
	{
		health -= (int)damage;
		//spawn damage floating text
		SpawnDFT(isCrit, (int)damage, true);
        
		//update healthbar
		//healthbar.Visible = true;
		//healthbar.UpdateHealthbar(health);
	}

	private void DirChanged()
	{
		if (dir == 1)
		{
			sprite.FlipH = false;
			raycast.Rotation = Mathf.DegToRad(-90);
		}
		else
		{
			sprite.FlipH = true;
			raycast.Rotation = Mathf.DegToRad(90);
		}
	}
	
	public override void Die()
	{
		isDead = true;
	}
}
