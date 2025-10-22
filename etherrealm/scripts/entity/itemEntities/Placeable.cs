using Godot;
using System;
using EtherRealm.scripts.entity.itemEntities;
using EtherRealm.scripts.resource;

public partial class Placeable : ActionEntity
{
    //children and other nodes
    
    //variables
    private PlaceableData placeableData;
    private int terrain;
    
    public override void _Ready()  
    {
        Initialize(placeableData);
        SetChildNodes();
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
        if (isAttacking)  
        {  
            Visible = true;  
            //attackCollider.Disabled = false;  
        }  
        else  
        {  
            Visible = false;  
            //attackCollider.Disabled = true;  
        }  
    }
    
    public override void UsePrimary()
    {
        PlaceBlock();
    }

    public override void UseSecondary(int dir)
    {
        
    }
    
    private void PlaceBlock()
    {
        if (isAttacking || isCooldown)  
            return;
        
        actionType = "swing";
        isAttacking = true;
        
        var mousePos = GetGlobalMousePosition();
        var tilePos = tilemap.LocalToMap(tilemap.ToLocal(mousePos));

        tilemap.PlaceBlock(tilePos, terrain);
        itemSlot.RemoveFromSlot(1);
    }
    
    public override void PlayAnimation(int dir, AnimationPlayer animPlayer)
    {
        if (actionType == "swing")
        {
            if (dir == 1)
            {
                animPlayer.Play("swingRight");
                hitDir = Vector2.Right;
            }
            else
            {
                animPlayer.Play("swingLeft");
                hitDir = Vector2.Left;
            }
        }
    }
    
}
