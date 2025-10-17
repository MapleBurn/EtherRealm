using Godot;
using System;

namespace EtherRealm.scripts.UI;
public partial class FloatingText : Label
{
	private float floatSpeed = 50f;
	private float lifetime = 1.0f;
	private float timeElapsed;

	public enum DamageType { damage, heal, crit };
	
	public override void _Ready()
	{ }
	
	public override void _Process(double delta)
	{
		timeElapsed += (float)delta;
		Position += new  Vector2(0, -floatSpeed) * (float)delta;
		
		Modulate = new Color(Modulate, 1f - (timeElapsed / lifetime));

		if (timeElapsed >= lifetime)
			QueueFree();
	}
	
	public void SetDamage(int amount, DamageType type, bool isPlayer)
	{
		Text = amount.ToString();
		if (isPlayer)
		{
			if (type == DamageType.damage) //change text color based on whether it's damage, heal or crit
				Modulate = Colors.Red;
			else if (type == DamageType.heal)
				Modulate = Colors.LimeGreen;
			else if (type == DamageType.crit)
			{
				Modulate = Colors.Gold;
				floatSpeed *= 1.5f;
				lifetime *= 1.5f;
			}
		}
		else
		{
			if (type == DamageType.damage) //change text color based on whether it's damage, heal or crit
				Modulate = Colors.IndianRed;
			else if (type == DamageType.heal)
				Modulate = Colors.GreenYellow;
			else if (type == DamageType.crit)
			{
				Modulate = Colors.Yellow;
				floatSpeed *= 1.5f;
				lifetime *= 1.5f;
			}
		}
	}
}
