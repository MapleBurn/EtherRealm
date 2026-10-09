using EtherRealm.Entities.Visual;
using Godot;

namespace EtherRealm.Entities;

public partial class LivingEntity : CharacterBody2D
{
    protected const string FloatingTextPath = "res://Entities/Visual/FloatingText.cs";
    
    protected const float MoveSpeed = 150.0f;
    protected const float JumpVelocity = -400.0f;
    protected int MaxHealth { get; set; }
}