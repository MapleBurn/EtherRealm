using Godot;
using System;
using EtherRealm.scripts.entity.enemies;
using EtherRealm.scripts.entity.itemEntities;
using EtherRealm.scripts.resource;
using EtherRealm.scripts.resource.item;
using EtherRealm.scripts.UI;
using EtherRealm.scripts.UI.inventory;

namespace EtherRealm.scripts.entity;
public partial class Player : Entity
{
	//other nodes
	[Export] private Healthbar _healthbar;
	[Export] private Label _debugLabel;
	private RayCast2D _raycast;
	private ShapeCast2D _shapecast;
	private Tween _stepTween;
	
	[Export] private Inventory _inventory;
	public HeldItemHandler ItemHandler;
	
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
		hurtbox = GetNode<Area2D>("hurtbox");
		animPlayer = GetNode<AnimationPlayer>("animationPlayer");
		
		acceleration = _acceleration;
		friction =  _friction;
		maxSpeed = _maxSpeed;
		jumpVelocity = _jumpVelocity;
		fallDamageThreshold = _fallDamageThreshold;
		maxHealth = _maxHealth;
		health = maxHealth;
		
		healthbar = _healthbar;
		healthbar.Initialize(maxHealth);
		_raycast = GetNode<RayCast2D>("RayCast2D");
		_shapecast = GetNode<ShapeCast2D>("ShapeCast2D");

		ItemHandler = GetNode<HeldItemHandler>("heldItem");
	}
	
	public override void _Input(InputEvent @event)  
	{  
		if (!ItemHandler.IsEntityInitialized)
			return;
		
		if (isDead || !ItemHandler.actionEntity.CanAttack() || _inventory.isInventoryOpen)
			return;  
		  
		if (@event is InputEventMouseButton mouseEvent)  
		{  
			if (mouseEvent.IsActionPressed("MouseLeftButton"))  
			{  
				ItemHandler.actionEntity.UsePrimary();
				ItemHandler.PlayAnimation(dir, animPlayer);
			}  
			else if (mouseEvent.IsActionPressed("MouseRightButton"))  
			{  
				//hand.actionEntity.UseSecondary(dir);
			}  
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (isDead)
			return;
		
		if (health <= 0)
			Die();
		
		TimerProcess((float)delta);	//does all the time stuff
			
		Vector2 velocity = Velocity;

		// Add the gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		
		// Handle Jump
		if (Input.IsActionPressed("space") && (IsOnFloor() || remJumpTimer < 0.15f))
		{
			velocity.Y = jumpVelocity;
		}
		if (Input.IsActionJustReleased("space") && velocity.Y < 0)
			velocity.Y = jumpVelocity * cutJumpHeight;
		
		var maxspeed = ItemHandler.IsAnimPlaying ? maxSpeed * 0.5f : maxSpeed;
		Vector2 direction = Input.GetVector("left", "right", "deadkey", "deadkey");
		float targetX = direction.X * maxspeed;
		
		if (direction != Vector2.Zero)
		{
			//step-up logic
			HandleStepUp();
			
			//Accelerate to target speed
			velocity.X = Mathf.MoveToward(Velocity.X, targetX, acceleration * (float)delta);
			
			//direction for animation
			if (direction.X > 0)
			{
				Scale = new Vector2(1, 1);
				RotationDegrees = 0f;
				dir = 1;
			} 
			else
			{
				Scale = new Vector2(1, -1);
				RotationDegrees = 180f;
				dir = -1;
			}
		}  
		else  
		{  
			//slow down when no input  
			velocity.X = Mathf.MoveToward(Velocity.X, 0, friction * (float)delta);  
		}

		HandleAnimation(direction);
		
		Velocity = velocity;
		Vector2 prevV = velocity;
		_debugLabel.Text = "Rotation: " + RotationDegrees + "\nScale: " + Scale;
		MoveAndSlide();

		//fall and collision damage
		ApplyImpactDamage(prevV); 
	}

	#region Animation
	private void HandleAnimation(Vector2 direction)
	{
		if ((ItemHandler.actionEntity != null && ItemHandler.IsAnimPlaying))
			return;
		
		if (!IsOnFloor())
		{
			UpdateAnimation("fall");
		}
		else
		{
			if (direction != Vector2.Zero)
				UpdateAnimation("walk");
			else
				UpdateAnimation("idle");
		}
	}
	
	private void UpdateAnimation(string animName)
	{
		const float blendDuration = 0.5f;
		const float walkBlendDuration = 0.08f;
		
		if (animPlayer.CurrentAnimation != animName)
		{
			if (animName == "walk")
				animPlayer.Play(animName, walkBlendDuration);
			animPlayer.Play(animName, blendDuration);
		}
	}
	
	#endregion
	
	#region Signals
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

	private void PickupAreaBodyEntered(Node2D body)
	{
		if (body.IsInGroup("items"))
		{
			ItemDrop item = (ItemDrop)body;
			item.TryPickUp();
		}
	}
	
	private void AnimationFinished(StringName animName)  
	{  
		if (animName == "stab" || animName == "place")
		{  
			ItemHandler.AnimationFinished();
		}
		/*else
		{
			if (animName == "startFallRight" || animName == "startFallLeft")
			{
				playFall = true;
			}
		}*/
	}
	
	#endregion

	#region  Step-Up Logic
	private void HandleStepUp()
	{
		_raycast.ForceRaycastUpdate();
		if (_raycast.IsColliding())
		{
			if (CanStepUp())
			{
				Vector2 stepUpOffset = new Vector2(dir * 16, -16);
				Vector2 stepTarget = GlobalPosition + stepUpOffset;
					
				float duration = MathF.Abs(stepUpOffset.X / maxSpeed) * 1.5f;
				if (duration > 0.1f)
					duration = 0.1f;
					
				_stepTween?.Kill();
				_stepTween = CreateTween();
				_stepTween.TweenProperty(this, "position", stepTarget, duration);

				_shapecast.Enabled = false;
			}
		}
	}

	private bool CanStepUp()
	{
		if (!IsOnFloor())
			return false;
		
		_shapecast.Enabled = true;
		_shapecast.ForceShapecastUpdate();
		return !_shapecast.IsColliding();	//return true if the shape cast doesn't detect anything
	}
	#endregion
	
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
	
	protected override void ProcessDamage(float damage, bool isCrit)
	{
		health -= (int)damage;
		//spawn damage floating text
		SpawnDFT(isCrit, (int)damage, true);
        
		hurtbox.Monitoring = false; //temporal
		//update healthbar
		healthbar.UpdateHealthbar(health);
	}
	
	protected override void Die()
	{
		isDead = true;
	}
}
