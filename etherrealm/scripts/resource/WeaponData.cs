using Godot;
using System;

public partial class WeaponData : Resource
{
    [Export] public float StabDistance { get; set; }
    [Export] public float AttackDamage { get; set; }
    [Export] public float CritChance { get; set; }
    [Export] public float CritDmgMult { get; set; }
    [Export] public float Knockback { get; set; }
    [Export] public float CritKbMult { get; set; }
    [Export] public float Delay { get; set; }
    [Export] public Texture2D Model { get; set; }
    //public CollisionPolygon2D collider;
}
