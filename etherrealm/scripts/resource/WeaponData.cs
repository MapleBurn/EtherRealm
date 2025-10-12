using Godot;
using System;

public partial class WeaponData : ActionEntityData
{
    [Export] public float StabDistance { get; set; }
    [Export] public float AttackDamage { get; set; }
    [Export] public float CritChance { get; set; }
    [Export] public float CritDmgMult { get; set; }
    [Export] public float Knockback { get; set; }
    [Export] public float CritKbMult { get; set; }

    public WeaponData()
    {
        Type = "weapon";
    }
}
