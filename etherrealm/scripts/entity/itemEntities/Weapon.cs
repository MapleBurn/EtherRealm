using Godot;  
using System;
using EtherRealm.scripts.entity;
using EtherRealm.scripts.resource;
using ActionEntityData = EtherRealm.scripts.resource.action_entity.ActionEntityData;

namespace EtherRealm.scripts.entity.itemEntities;
public partial class Weapon : ActionEntity
{
	//resource and res variables
	private WeaponData wepData;
	public float damage;  
	public float critChance;
	public float critDmgMult;  
	public float knockback;  
	public float critKbMult;  
	private float stabDistance; 
	
	private Tween attackTween;
	  
	public override void _Ready()  
	{  
		//data = wepData;
		Initialize(wepData);
		SetChildNodes();
	}

	public override void Initialize(ActionEntityData d)
	{
		wepData = (WeaponData)d;
		data = wepData;
		
		stabDistance = wepData.StabDistance;
		damage = wepData.AttackDamage;
		critChance = wepData.CritChance;
		critDmgMult = wepData.CritDmgMult;
		knockback = wepData.Knockback;
		critKbMult  = wepData.CritKbMult;
		delay = wepData.Delay;
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

	public override void UsePrimary()
	{
		//rotate weapon to mouse and stab  
		SetRotationToTarget(GetGlobalMousePosition());  
		Stab(GetGlobalMousePosition());
	}

	public override void UseSecondary(int dir)
	{
		//swing based on player direction  
		Swing(dir);
	}

	private void Stab(Vector2 targetPosition)  
	{  
		//let the stab finish so 1) no spamming 2) the sword doesn't fly off  
		if (attackTween != null || isCooldown)  
			return;
		
		actionType = "stab";
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
			Rotation = 0;
		};  
	}  
  
	private void Swing(int dir)  
	{  
		if (isAttacking || isCooldown)  
			return;  
		
		actionType = "swing";
		isAttacking = true;  
	} 
	  
	private void SetRotationToTarget(Vector2 targetPosition)  
	{  
		LookAt(targetPosition);  
		Rotation += Mathf.DegToRad(45);  
	}  
}