using Godot;
using System;
using EtherRealm.scripts.resource;

public partial class PlaceableData : ActionEntityData
{
    [Export] public int Terrain { get; set; } = 1;
    
    public PlaceableData()
    {
        Type = "placeable";
    }
}
