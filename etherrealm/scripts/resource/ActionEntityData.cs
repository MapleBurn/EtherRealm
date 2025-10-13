using Godot;
using System;

public partial class ActionEntityData : Resource
{
    [Export] public string Type { get; set; }
    [Export] public float Delay { get; set; }
    [Export] public Texture2D Model { get; set; }
    [Export] public Vector2 SpriteOffset { get; set; }
    [Export] public Vector2[] ColliderPoints { get; set; }
    [Export] public string EntityScenePath { get; set; }
}
