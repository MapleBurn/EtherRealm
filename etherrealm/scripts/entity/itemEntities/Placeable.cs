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
        ConsumeOnUse = true;
        Visible = false;
    }

    public override void Initialize(ActionEntityData d)
    {
        placeableData = (PlaceableData)d;
        Data = placeableData;

        terrain = placeableData.Terrain;
        Delay = placeableData.Delay;
    }

    public override void _Process(double delta)
    {
    }
    
    public override void UsePrimary()
    {
        var mousePos = GetGlobalMousePosition();
        var tilePos = Tilemap.LocalToMap(Tilemap.ToLocal(mousePos));
        
        if (Tilemap.CanPlaceBlock(tilePos))
            PlaceBlock();
    }

    public override void UseSecondary(int dir)
    {
        
    }
    
    private void PlaceBlock()
    {
        if (ItemHandler.IsAnimPlaying || IsCooldown)  
            return;
        
        ActionType = "place";
        ItemHandler.IsAnimPlaying = true;
        
        ConsumeItem();
    }
}
