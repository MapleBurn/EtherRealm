using Godot;
using System;

public partial class Player : Entity
{
	//other nodes
	[Export] private AnimationPlayer _animPlayer;
	[Export] private Area2D _hurtbox;
	[Export] private TileMapLayer tilemap;
	[Export] private RayCast2D raycast;
	[Export] private Weapon weapon;
	private Tween stepTween;
	
	//players properties
	[Export] private float _acceleration = 600.0f;
	[Export] private float _friction = 800.0f;
	[Export] private float _maxSpeed = 150.0f;
	[Export] private float _jumpVelocity = -400.0f;
	[Export] private float _fallDamageThreshold = 550f;
	[Export] private int _maxHealth = 100;
	
	//timers
	private float invTimer = 0;
	private float heTimer = 0;
	private float remJumpTimer = 0; //jump after getting in the air for responsiveness

	private int dir = 1; //one means right
	private float cutJumpHeight = 0.4f;

	public override void _Ready()
	{
		hurtbox = _hurtbox;
		animPlayer = _animPlayer;
		
		acceleration = _acceleration;
		friction =  _friction;
		maxSpeed = _maxSpeed;
		jumpVelocity = _jumpVelocity;
		fallDamageThreshold = _fallDamageThreshold;
		maxHealth = _maxHealth;
		health = maxHealth;
		
		healthbar = GetNode<Healthbar>("healthbar");
		healthbar.Initialize(maxHealth);
	}
	
	public override void _Input(InputEvent @event)  
	{  
		if (isDead || !weapon.CanAttack())  
			return;  
		  
		if (@event is InputEventMouseButton mouseEvent)  
		{  
			if (mouseEvent.IsActionPressed("MouseLeftButton"))  
			{  
				// Rotate weapon to mouse and stab  
				weapon.SetRotationToTarget(GetGlobalMousePosition());  
				weapon.Stab(GetGlobalMousePosition());  
			}  
			else if (mouseEvent.IsActionPressed("MouseRightButton"))  
			{  
				// Swing based on player direction  
				weapon.Swing(dir);  
			}  
		}  
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (isDead)
			return;
		
		if (health <= 0)
		{
			Die();
		}
		
		TimerProcess((float)delta);	//does all the time stuff
			
		Vector2 velocity = Velocity;

		// Add the gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		
		// Handle Jump
		if (Input.IsActionPressed("space") && (IsOnFloor() || remJumpTimer < 0.1f))
		{
			velocity.Y = jumpVelocity;
		}
		if (Input.IsActionJustReleased("space") && velocity.Y < 0)
			velocity.Y = jumpVelocity * cutJumpHeight;
		
		Vector2 direction = Input.GetVector("left", "right", "deadkey", "deadkey");
		float targetX = direction.X * maxSpeed;
		
		if (direction != Vector2.Zero)
		{
			//step-up logic
			raycast.ForceRaycastUpdate();
			if (raycast.IsColliding())
			{
				Vector2 collisionPoint = raycast.GetCollisionPoint();
				Vector2 localPoint = tilemap.ToLocal(collisionPoint);
				Vector2I tileCoords = tilemap.LocalToMap(localPoint);

				if (CanStepUp(tileCoords))
				{
					Vector2 stepUpOffset = new Vector2(dir * 16, -16);
					Vector2 stepTarget = GlobalPosition + stepUpOffset;

					float duration;
					duration = (stepUpOffset.X / maxSpeed) * 1.5f;
					if (duration > 0.1f)
						duration = 0.1f;
					
					stepTween?.Kill();
					stepTween = CreateTween();
					stepTween.TweenProperty(this, "position", stepTarget, duration);
				}

			}
			
			//Accelerate to target speed
			velocity.X = Mathf.MoveToward(Velocity.X, targetX, acceleration * (float)delta);
			
			//direction and animation
			if (direction.X > 0)  
			{  
				dir = 1;  
				DirChanged();  
			}  
			else if (direction.X < 0)  
			{  
				dir = -1;  
				DirChanged();  
			}

			if (!weapon.isAttacking)
				UpdateAnimation("walk");
		}
		else
		{
			//slow down when no input
			velocity.X = Mathf.MoveToward(Velocity.X, 0, friction * (float)delta);

			if (!weapon.isAttacking)
				UpdateAnimation("idle");
		}

		Velocity = velocity;
		Vector2 prevV = velocity;
		MoveAndSlide();

		//fall and collision damage
		ApplyImpactDamage(prevV); 
	}
	
	private void TimerProcess(float delta)
	{
		if (!hurtbox.Monitoring)
		{
			if (invTimer < 0.5f)
				invTimer += delta;
			else
			{
				invTimer = 0;
				hurtbox.Monitoring = true;
			}
		}

		if (heTimer < 5.0f)
			heTimer += delta;
		else
		{
			ApplyHealing(2);	//passive healing
			heTimer = 0;
		}
		
		//jump memory - for smoothness
		if (IsOnFloor())
			remJumpTimer = 0;
		else
			remJumpTimer += delta;
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
		CallDeferred("ProcessDamage", damage, isCrit);
		ProcessKnockback(knockback, hitDir);
	}
	
	private void AnimationFinished(StringName animName)  
	{  
		if (animName == "swingRight" || animName == "swingLeft")  
		{  
			weapon.AttackFinished();
		}  
	}

	private void UpdateAnimation(string animName)
	{
		if (dir == 1)
			animPlayer.Play(animName + "Right");
		else
			animPlayer.Play(animName + "Left");
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
	
	protected override void ProcessDamage(float damage, bool isCrit)
	{
		health -= (int)damage;
		//spawn damage floating text
		SpawnDFT(isCrit, (int)damage, true);
        
		hurtbox.Monitoring = false; //temporal
		//update healthbar
		healthbar.UpdateHealthbar(health);
	}

	private void DirChanged() //method name is wrong - rn it's called every frame not only when dir changes
	{
		if (dir == -1)	//left
		{
			raycast.Position = new Vector2(-21, 16);
		}
		else   //right
		{
			raycast.Rotation = Mathf.DegToRad(90 * dir);
			raycast.Position = new Vector2(0, 16);
		}
	}
	
	protected override void Die()
	{
		isDead = true;
	}
}
