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
	public float delay;
	public Area2D hitbox;
	public Player player;
	public float stabDistance;
	public Tween attackTween;
	public CollisionPolygon2D attackCollider;
	public AnimationPlayer animPlayer;
	public bool isAttacking = false;
	
	private bool isCooldown = false;
	private float startCooldown;
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.IsActionPressed("MouseLeftButton"))
			{
				//rotate to mouse
				LookAt(GetGlobalMousePosition());
				Rotation += 45;
				Stab();
			}
			else if (mouseEvent.IsActionPressed("MouseRightButton"))
			{
				Swing();
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

		if (isCooldown && startCooldown + delay < Time.GetTicksMsec())
			isCooldown = false;
	}
	
	public void Stab()
	{
		//let the stab finish so 1) no spamming 2) the sword doesn't fly off
		if (attackTween != null || isCooldown)
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
		if (animPlayer.IsPlaying() || isCooldown)
			return;

		isAttacking = true;
		animPlayer.Play("swingRight");
	}
	
	protected void AnimationFinished(StringName animName)
	{
		if (animName == "swing")
		{
			isAttacking = false;
			isCooldown = true;
			startCooldown = Time.GetTicksMsec();
		}
	}
}