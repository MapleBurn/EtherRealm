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
	
	//variables
	private double comboTime;
	private bool isCombo = true;
	  
	public override void _Ready()  
	{  
		//data = wepData;
		Initialize(wepData);
		//SetChildNodes();
		Visible = false;
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
		if (hand.isAnimPlaying)  
		{  
			Visible = true;  
			attackCollider.Disabled = false;  
		}  
		else  
		{  
			Visible = false;  
			attackCollider.Disabled = true;  
		}

		if (comboTime < 1)
			comboTime += delta;
		else
			isCombo = true;
	}

	public override void UsePrimary()
	{
		Attack();
	}

	public override void UseSecondary(int dir)
	{
		//swing based on player direction  
		Swing(dir);
	}

	private void Attack()
	{
		if (hand.isAnimPlaying || isCooldown)  
			return;  
		
		actionType = "attack";
		hand.isAnimPlaying = true;
		isCombo = true;
		comboTime = 0;
	}
	
	private void Swing(int dir)  
	{  
		if (hand.isAnimPlaying || isCooldown)  
			return;  
		
		actionType = "swing";
		hand.isAnimPlaying = true;  
	} 
	
	public override void AttackFinished()  
	{  
		isCooldown = true;  
		hand.isAnimPlaying = false;  
		
		if (isCombo)
		{
			 isCooldown = false;
			 return;
		}
		
		GetTree().CreateTimer(delay).Timeout += () =>  
		{  
			isCooldown = false;   
		};  
	}
}