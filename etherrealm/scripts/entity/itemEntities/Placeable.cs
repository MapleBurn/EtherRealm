using Godot;
using System;
using EtherRealm.scripts.entity.itemEntities;
using EtherRealm.scripts.resource;
using ActionEntityData = EtherRealm.scripts.resource.action_entity.ActionEntityData;

public partial class Placeable : ActionEntity
{
    //children and other nodes
    
    //variables
    private PlaceableData placeableData;
    private int terrain;
    
    public override void _Ready()  
    {
        Initialize(placeableData);
        consumeOnUse = true;
        Visible = false;
    }

    public override void Initialize(ActionEntityData d)
    {
        placeableData = (PlaceableData)d;
        data = placeableData;

        terrain = placeableData.Terrain;
        delay = placeableData.Delay;
    }

    public override void _Process(double delta)
    {
    }
    
    public override void UsePrimary()
    {
        var mousePos = GetGlobalMousePosition();
        var tilePos = tilemap.LocalToMap(tilemap.ToLocal(mousePos));
        
        if (tilemap.CanPlaceBlock(tilePos))
            PlaceBlock();
    }

    public override void UseSecondary(int dir)
    {
        
    }
    
    private void PlaceBlock()
    {
        if (hand.isAnimPlaying || isCooldown)  
            return;
        
        actionType = "place";
        hand.isAnimPlaying = true;
        
        ConsumeItem();
    }
}
