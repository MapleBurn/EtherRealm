using Godot;

namespace EtherRealm.Entities.Visual;

public partial class FloatingText : Label
{
    private float _floatSpeed = 50f;
    private float _lifetime = 1.0f;
    private float _timeElapsed;

    public enum DamageType { Damage, Heal, Crit };
	
    public override void _Process(double delta)
    {
        _timeElapsed += (float)delta;
        Position += new  Vector2(0, -_floatSpeed) * (float)delta;
		
        Modulate = new Color(Modulate, 1f - (_timeElapsed / _lifetime));

        if (_timeElapsed >= _lifetime)
            QueueFree();
    }
	
    public void SetText(int amount, DamageType type, bool isPlayer)
    {
        Text = amount.ToString();
        if (isPlayer)
        {
            if (type == DamageType.Damage) //change text color based on whether it's damage, heal or crit
                Modulate = Colors.Red;
            else if (type == DamageType.Heal)
                Modulate = Colors.LimeGreen;
            else if (type == DamageType.Crit)
            {
                Modulate = Colors.Gold;
                _floatSpeed *= 1.5f;
                _lifetime *= 1.5f;
            }
        }
        else
        {
            if (type == DamageType.Damage) //change text color based on whether it's damage, heal or crit
                Modulate = Colors.IndianRed;
            else if (type == DamageType.Heal)
                Modulate = Colors.GreenYellow;
            else if (type == DamageType.Crit)
            {
                Modulate = Colors.Yellow;
                _floatSpeed *= 1.5f;
                _lifetime *= 1.5f;
            }
        }
    }
}