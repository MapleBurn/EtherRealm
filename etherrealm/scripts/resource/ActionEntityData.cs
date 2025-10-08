using Godot;
using System;

public partial class ActionEntityData : Resource
{
    [Export] public Texture2D Model { get; set; }
    [Export] public float Delay { get; set; }
}
