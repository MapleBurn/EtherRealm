using Godot;
using System;

public partial class Healthbar : ProgressBar
{
	[Export] private ProgressBar damagebar;
	
	private bool lowerDamageBar = false;
	private double startTime;
	private double endTime;
	

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (Time.GetTicksMsec() - startTime > 1000)
		{
			lowerDamageBar = true;
		}
		
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
		TimerStarted();
	}

	private void TimerStarted()
	{
		startTime = Time.GetTicksMsec();
	}
}
