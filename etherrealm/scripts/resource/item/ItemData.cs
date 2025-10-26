using Godot;
using System;

namespace EtherRealm.scripts.resource.item;
public partial class ItemData : Resource
{
    [Export] public string ItemId { get; set; } = "";
    [Export] public string DisplayName { get; set; } = "";
    [Export] public Texture2D Texture { get; set; }
    [Export] public int MaxStack { get; set; } = 999;
    [Export] public action_entity.ActionEntityData EntityData { get; set; }
    
}
