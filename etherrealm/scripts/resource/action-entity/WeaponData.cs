using Godot;
using System;

namespace EtherRealm.scripts.resource;
public partial class WeaponData : action_entity.ActionEntityData
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
