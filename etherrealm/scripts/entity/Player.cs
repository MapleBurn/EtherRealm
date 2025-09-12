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
			if (raycast.IsColliding())
			{ 
				Vector2 tilePos = raycast.GetCollisionPoint();
				tilePos = tilemap.ToLocal(tilePos);
				Vector2I tileCoords = tilemap.LocalToMap(tilePos);
				TryStepUp(tileCoords);
			}
			
			//Accelerate to target speed
			velocity.X = Mathf.MoveToward(Velocity.X, targetX, acceleration * (float)delta);
			sprite.Play("walk");
			if (direction > Vector2.Zero)
			{
				sprite.FlipH = false;
				dir = 1;
			}
			else
			{
				sprite.FlipH = true;
				dir = -1;
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

	private bool TryStepUp(Vector2I tileCoords)
	{
		var upTC1 =  tileCoords + new Vector2I(0, -1);
		var upTC2 =  tileCoords + new Vector2I(0, -2);
		var upTC3 =  tileCoords + new Vector2I(0, -3);
		var upSideTC =  tileCoords + new Vector2I(-dir, -3);

		return false;
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
	
	public override void Die()
	{
		isDead = true;
	}
}
