using Godot;
using System;

public partial class Weapon : Node2D
{
	public float damage;
	public float critChance; // chance in percent to do critical hit
	public float critDmgMult;
	public float knockback;
	public float critKBMult;
	public Vector2 hitDir;
	public Area2D hitbox { get; set; }
	public float stabDistance;
	public Tween attackTween;
	public CollisionShape2D attackCollider;
	public bool isAttacking = false;
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.IsActionPressed("MouseLeftButton"))
			{
				//rotate to mouse
				LookAt(GetGlobalMousePosition());
				Attack();
			}
		}
	}
	
	public override void _Process(double delta)
	{
		if (isAttacking)
		{
			Visible = true;
			attackCollider.Disabled = false;
		}
		else
		{
			Visible = false;
			attackCollider.Disabled = true;
		}
	}

	public virtual void Attack()
	{
		Stab();
	}
	
	public void Stab()
	{
		//let the stab finish so 1) no spamming 2) the sword doesn't fly off
		if (attackTween != null)
			return;
        
		var origin = Position;
		var mouseDir = (GetGlobalMousePosition() - GlobalPosition).Normalized();
		hitDir = mouseDir;

		attackTween = GetTree().CreateTween().BindNode(this);
		attackTween.TweenProperty(this, "position", origin + (mouseDir * stabDistance), 0.1f);
		attackTween.TweenProperty(this, "position", origin, 0.1f).SetDelay(0.1f);
        
		isAttacking = true;
        
		//make it null after finishing
		attackTween.Finished += () =>
		{
			isAttacking = false;
			attackTween = null;
		};
	}

	public void Swing()
	{
		if (attackTween != null)
			return;

		Vector2 mouseDir = (GetGlobalMousePosition() - GlobalPosition).Normalized();
		float targetAngle = mouseDir.Angle();

		float swingAngle = Mathf.Pi / 2f; // 90 degrees

		// Determine if swing is to the left or right
		bool rightSide = mouseDir.X > 0;
		float startAngle, endAngle;

		if (rightSide)
		{
			// Clockwise swing
			startAngle = targetAngle - swingAngle / 2f;
			endAngle = targetAngle + swingAngle / 2f;
		}
		else
		{
			// Counterclockwise swing
			startAngle = targetAngle + swingAngle / 2f;
			endAngle = targetAngle - swingAngle / 2f;
		}

		Rotation = startAngle;
		attackTween = GetTree().CreateTween().BindNode(this);
		attackTween.TweenProperty(this, "rotation", endAngle, 0.2f);

		isAttacking = true;
		attackTween.Finished += () =>
		{
			isAttacking = false;
			attackTween = null;
		};
	}
}