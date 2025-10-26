using Godot;
using System;
using EtherRealm.scripts.entity;
using EtherRealm.scripts.resource;
using EtherRealm.scripts.util;
using ActionEntityData = EtherRealm.scripts.resource.action_entity.ActionEntityData;

namespace EtherRealm.scripts.entity.itemEntities;
public partial class Tool : ActionEntity
{
    //children and other nodes
    
    //variables
    private ToolData toolData;
    
    public override void _Ready()  
    {
        Initialize(toolData);
        //SetChildNodes();
    }

    public override void Initialize(ActionEntityData d)
    {
        toolData = (ToolData)d;
        data = toolData;
        
        delay = toolData.Delay;
    }
    
    public override void _Process(double delta)  
    {
        if (isAttacking)  
        {  
            Visible = true;  
            attackCollider.Disabled = false;  
        }  
        else  
        {  
            Visible = false;  
            attackCollider.Disabled = true;  
        }  
    }
    
    public override void UsePrimary()
    {
        Mine();
    }

    public override void UseSecondary(int dir)
    {
        
    }
    
    private void Mine()
    {
        if (isAttacking || isCooldown)  
            return;
        
        actionType = "swing";
        isAttacking = true;
        
        var mousePos = GetGlobalMousePosition();
        var tilePos = tilemap.LocalToMap(tilemap.ToLocal(mousePos));
        tilemap.TryBreakBlock(tilePos);
    }
}
