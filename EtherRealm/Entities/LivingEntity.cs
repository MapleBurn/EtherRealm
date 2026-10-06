using Godot;

namespace EtherRealm.Entities;

public partial class LivingEntity : CharacterBody2D
{
    protected const float Speed = 150.0f;
    protected const float JumpVelocity = -400.0f;
}