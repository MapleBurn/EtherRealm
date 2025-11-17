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
	}
	
	private void Swing(int dir)  
	{  
		if (hand.isAnimPlaying || isCooldown)  
			return;  
		
		actionType = "swing";
		hand.isAnimPlaying = true;  
	} 
}