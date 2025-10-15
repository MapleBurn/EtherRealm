using Godot;
using System;

public partial class Tool : ActionEntity
{
    private ToolData toolData;
    public override void _Ready()  
    {
        Initialize(toolData);
        SetChildNodes();
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
        GD.Print("Mining with tool");
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
