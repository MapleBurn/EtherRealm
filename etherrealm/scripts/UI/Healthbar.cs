using Godot;
using System;

public partial class Healthbar : ProgressBar
{
	[Export] private ProgressBar damagebar;
	[Export] private Timer timer;
	
	private bool lowerDamageBar = false;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (lowerDamageBar)
		{
			if (damagebar.Value > Value)
			{
				damagebar.Value -= delta * 200; //slowly lower damage bar
			}
			else
			{
				damagebar.Value = Value;
				lowerDamageBar = false;
			}
		}
	}

	public void Initialize(int maxHp)
	{
		MaxValue = maxHp;
		Value = maxHp;
		damagebar.MaxValue = maxHp;
		damagebar.Value = maxHp;
	}
	
	public void UpdateHealthbar(int hp)
	{
		Value = hp;
		timer.Start();
	}

	public void TimerTimeout()
	{
		lowerDamageBar = true;
	}
}
