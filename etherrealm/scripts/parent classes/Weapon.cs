using Godot;
using System;

public partial class Weapon : Node2D
{
	public float damage;
	public float critChance; // chance in percent to do critical hit
	public float knockback;
	public Vector2 hitDir;
	
	public virtual void Input(InputEvent @event)
	{
		// zjistí, že bylo stisknuto tlačítko myši
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.IsActionPressed("MouseLeftButton"))
			{
				// otočí Node2D tak, aby směřoval na pozici myši (globální souřadnice)
				LookAt(GetGlobalMousePosition());
			}
		}
	}

	public virtual void Stab()
	{

	}
}