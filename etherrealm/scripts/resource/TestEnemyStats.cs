using Godot;
using System;

namespace EtherRealm.Enemies;

public partial class TestEnemyStats : Resource
{
    //basic Enemy stats
    [Export] public int maxHealth;
    [Export] public float maxSpeed;
    [Export] public float acceleration;
    [Export] public float friction;
    [Export] public float fallDamageThreshold;

    //Enemy attack stats
    [Export] public float damage;
    [Export] public float critChance; //chance in percent to do critical hit
    [Export] public float critDamageMult; //critical damage multiplier
    [Export] public float knockback;
    [Export] public float critKnockbackMult; //critical knockback multiplier
}
