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
	//private double comboTime;
	//private bool isCombo = true;
	  
	public override void _Ready()  
	{  
		Initialize(wepData);
		Visible = false;
	}

	public override void Initialize(ActionEntityData data)
	{
		wepData = (WeaponData)data;
		Data = wepData;
		
		stabDistance = wepData.StabDistance;
		damage = wepData.AttackDamage;
		critChance = wepData.CritChance;
		critDmgMult = wepData.CritDmgMult;
		knockback = wepData.Knockback;
		critKbMult  = wepData.CritKbMult;
		Delay = wepData.Delay;
	}
	
	public override void _Process(double delta)  
	{
		if (ItemHandler.IsAnimPlaying)  
		{  
			Visible = true;  
			AttackCollider.Disabled = false;  
		}  
		else  
		{  
			Visible = false;  
			AttackCollider.Disabled = true;  
		}

		/*if (comboTime < 1)
			comboTime += delta;
		else
			isCombo = true;*/
	}

	public override void UsePrimary()
	{
		if (ItemHandler.IsAnimPlaying || IsCooldown)  
			return;  
		
		ActionType = "attack";
		ItemHandler.IsAnimPlaying = true;
	}

	public override void UseSecondary(int dir)
	{
		//swing based on player direction  
		Swing(dir);
	}
	
	private void Swing(int dir)  
	{  
		if (ItemHandler.IsAnimPlaying || IsCooldown)  
			return;  
		
		ActionType = "swing";
		ItemHandler.IsAnimPlaying = true;  
	} 
	
	public override void AttackFinished()  
	{  
		IsCooldown = true;  
		ItemHandler.IsAnimPlaying = false;  
		
		/*if (isCombo)
		{
			 IsCooldown = false;
			 return;
		}*/
		
		GetTree().CreateTimer(Delay).Timeout += () =>  
		{  
			IsCooldown = false;   
		};  
	}
}