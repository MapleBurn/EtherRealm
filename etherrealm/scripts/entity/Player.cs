using Godot;
using System;

public partial class Player : Entity
{
	[Export] private AnimatedSprite2D _sprite;
	[Export] private Area2D _hurtbox;
	[Export] private TileMapLayer tilemap;
	[Export] private RayCast2D raycast;
	
	//tilemap stuff
	private int tilesize;
	
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
		
		tilesize = tilemap.TileSet.TileSize.X;
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
			/*if (raycast.IsColliding())
			{
				Vector2 tilePos = raycast.GetCollisionPoint();
				tilePos = tilemap.ToLocal(tilePos);
				Vector2I tileCoords = tilemap.LocalToMap(tilePos);
				if (CanStepUp(tileCoords))
				{
					GlobalPosition += new Vector2(8 * dir, -8);
				}
			}*/
			
			raycast.ForceRaycastUpdate();
			if (raycast.IsColliding())
			{
				// získej bod kolize v globálních souřadnicích
				Vector2 collisionPoint = raycast.GetCollisionPoint();

				// převedeme do lokálních souřadnic tilemapy před voláním LocalToMap
				Vector2 localPoint = tilemap.ToLocal(collisionPoint);
				Vector2I tileCoords = tilemap.LocalToMap(localPoint);

				if (CanStepUp(tileCoords))
				{
					// posuň hráče o malý krok nahoru+ahead (globálně)
					GlobalPosition += new Vector2(8 * dir, -8);
				}
			}
			
			//Accelerate to target speed
			velocity.X = Mathf.MoveToward(Velocity.X, targetX, acceleration * (float)delta);
			sprite.Play("walk");
			if (direction > Vector2.Zero)
			{
				sprite.FlipH = false;
				dir = 1;
				DirChanged();
			}
			else
			{
				sprite.FlipH = true;
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
			raycast.Position += new Vector2(15, 0);
			raycast.Rotation = -90;
		}
		else
		{
			raycast.Position -= new Vector2(15, 0);
			raycast.Rotation = 90;
		}
	}
	
	public override void Die()
	{
		isDead = true;
	}
}
