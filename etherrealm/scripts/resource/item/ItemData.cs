using Godot;
using System;

public partial class ItemData : Resource
{
    [Export] public string ItemId { get; set; } = "";
    [Export] public string DisplayName { get; set; } = "";
    [Export] public Texture2D Texture { get; set; }
    [Export] public int MaxStack { get; set; } = 999;
    [Export] public ActionEntityData EntityData { get; set; }
    
}
