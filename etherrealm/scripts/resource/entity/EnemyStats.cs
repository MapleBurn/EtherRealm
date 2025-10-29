using Godot;

namespace EtherRealm.scripts.resource.entity;
public partial class EnemyStats : Resource
{
    //basic Enemy stats
    [Export] public int MaxHealth;
    [Export] public float MaxSpeed;
    [Export] public float Acceleration;
    [Export] public float Friction;
    [Export] public float FallDamageThreshold;

    //Enemy attack stats
    [Export] public float Damage;
    [Export] public float CritChance; //chance in percent to do critical hit
    [Export] public float CritDamageMult; //critical damage multiplier
    [Export] public float Knockback;
    [Export] public float CritKnockbackMult; //critical knockback multiplier
    
    //Enemy model properties
    [Export] public Texture2D Model;
    [Export] public Vector2 ColliderOffset { get; set; }
    [Export] public RectangleShape2D ColliderShape { get; set; }
    [Export] public Vector2 HealthBarOffset { get; set; }
    [Export] public Vector2 HealthBarSize { get; set; }
}