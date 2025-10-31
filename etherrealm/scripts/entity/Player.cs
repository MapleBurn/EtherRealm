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
	[Export] private AnimationPlayer _animPlayer;
	[Export] private Area2D _hurtbox;
	[Export] private Healthbar _healthbar;
	private RayCast2D raycast;
	private ShapeCast2D shapecast;
	private Tween stepTween;
	
	[Export] private Inventory inventory;
	public Hand hand;
	
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
		
		healthbar = _healthbar;
		healthbar.Initialize(maxHealth);
		raycast = GetNode<RayCast2D>("RayCast2D");
		shapecast = GetNode<ShapeCast2D>("ShapeCast2D");

		hand = GetNode<Hand>("hand");
	}
	
	public override void _Input(InputEvent @event)  
	{  
		if (!hand.isEntityInitialized)
			return;
		
		if (isDead || !hand.actionEntity.CanAttack() || inventory.isInventoryOpen)
			return;  
		  
		if (@event is InputEventMouseButton mouseEvent)  
		{  
			if (mouseEvent.IsActionPressed("MouseLeftButton"))  
			{  
				hand.actionEntity.UsePrimary();
				hand.PlayAnimation(dir, animPlayer);
			}  
			else if (mouseEvent.IsActionPressed("MouseRightButton"))  
			{  
				hand.actionEntity.UseSecondary(dir);
				hand.PlayAnimation(dir, animPlayer);
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
		if (Input.IsActionPressed("space") && (IsOnFloor() || remJumpTimer < 0.15f))
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
				if (CanStepUp())
				{
					Vector2 stepUpOffset = new Vector2(dir * 16, -16);
					Vector2 stepTarget = GlobalPosition + stepUpOffset;
					
					float duration = MathF.Abs(stepUpOffset.X / maxSpeed) * 1.5f;
					if (duration > 0.1f)
						duration = 0.1f;
					
					stepTween?.Kill();
					stepTween = CreateTween();
					stepTween.TweenProperty(this, "position", stepTarget, duration);

					shapecast.Enabled = false;
				}
			}
			
			//Accelerate to target speed
			velocity.X = Mathf.MoveToward(Velocity.X, targetX, acceleration * (float)delta);
			
			//direction and animation
			if (direction.X > 0)  
				dir = 1;  
			else if (direction.X < 0)  
				dir = -1;  
			
			if (hand.actionEntity == null || !hand.isAnimPlaying)  
				UpdateAnimation("walk");
		}  
		else  
		{  
			//slow down when no input  
			velocity.X = Mathf.MoveToward(Velocity.X, 0, friction * (float)delta);  
      
			if (hand.actionEntity == null || !hand.isAnimPlaying)  
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
		if (animName == "swingRight" || animName == "swingLeft" || animName == "placeRight" || animName == "placeLeft")
		{  
			hand.AnimationFinished();
		}  
	}
	
	#endregion
	

	private void UpdateAnimation(string animName)
	{
		string targetAnim = dir == 1 ? animName + "Right" : animName + "Left";  
		if (animPlayer.CurrentAnimation != targetAnim)  
			animPlayer.Play(targetAnim);  
	}

	private bool CanStepUp()
	{
		if (!IsOnFloor())
			return false;
		
		shapecast.Enabled = true;
		shapecast.ForceShapecastUpdate();
		return !shapecast.IsColliding();	//return true if the shape cast doesn't detect anything
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
