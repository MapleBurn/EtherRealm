using Godot;  
using System;  
  
public partial class Weapon : Node2D  
{  
	public float damage;  
	public float critChance; // chance in percent to do critical hit  
	public float critDmgMult;  
	public float knockback;  
	public float critKBMult;  
	protected float delay;  
	public Vector2 hitDir;  
	protected Area2D hitbox;  
	protected float stabDistance;  
	protected Tween attackTween;  
	protected CollisionPolygon2D attackCollider;  
	protected AnimationPlayer animPlayer;  
	
	public bool isAttacking = false;
	public string attackType;
	private bool isCooldown = false;  
	  
	public override void _Ready()  
	{  
		// Initialize components  
		hitbox = GetNode<Area2D>("Hitbox");  
		attackCollider = GetNode<CollisionPolygon2D>("Hitbox/CollisionPolygon2D"); ;  
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
	  
	public bool CanAttack()  
	{  
		return !isAttacking && !isCooldown;  
	}  
	  
	public void Stab(Vector2 targetPosition)  
	{  
		//let the stab finish so 1) no spamming 2) the sword doesn't fly off  
		if (attackTween != null || isCooldown)  
			return;

		attackType = "stab";
		var origin = Position;  
		var mouseDir = (targetPosition - GlobalPosition).Normalized();  
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
  
	public void Swing(int dir)  
	{  
		if (isAttacking || isCooldown)  
			return;  
		
		attackType = "swing";
		isAttacking = true;  
		  
		if (dir == 1)
		{
			animPlayer.Play("swingRight");
			hitDir = Vector2.Right;
		}
		else
		{
			animPlayer.Play("swingLeft");
			hitDir = Vector2.Left;
		}
	}  
	
	public void AttackFinished()  
	{  
		isAttacking = false;  
		isCooldown = true;  
  
		GetTree().CreateTimer(0.8).Timeout += () =>  
		{  
			isCooldown = false;   
		};  
	}
	  
	public void SetRotationToTarget(Vector2 targetPosition)  
	{  
		LookAt(targetPosition);  
		Rotation += Mathf.DegToRad(45);  
	}  
}