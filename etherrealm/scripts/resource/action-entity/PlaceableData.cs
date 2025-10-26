using Godot;
using System;
using EtherRealm.scripts.resource;
using ActionEntityData = EtherRealm.scripts.resource.action_entity.ActionEntityData;

public partial class PlaceableData : ActionEntityData
{
    [Export] public int Terrain { get; set; } = 1;
    
    public PlaceableData()
    {
        Type = "placeable";
    }
}
