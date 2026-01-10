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
        Visible = false;
    }

    public override void Initialize(ActionEntityData d)
    {
        toolData = (ToolData)d;
        Data = toolData;
        
        Delay = toolData.Delay;
    }
    
    public override void _Process(double delta)  
    {
        if (ItemHandler.IsAnimPlaying)  
        {  
            Visible = true;  
            AttackCollider.Disabled = false;  
        }  
        else  
        {  
            Visible = false;  
            AttackCollider.Disabled = true;  
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
        if (ItemHandler.IsAnimPlaying || IsCooldown)  
            return;
        
        ActionType = "swing";
        ItemHandler.IsAnimPlaying = true;
        
        var mousePos = GetGlobalMousePosition();
        var tilePos = Tilemap.LocalToMap(Tilemap.ToLocal(mousePos));
        Tilemap.TryBreakBlock(tilePos);
    }
}
