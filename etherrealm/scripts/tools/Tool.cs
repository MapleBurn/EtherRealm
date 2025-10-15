using Godot;
using System;

public partial class Tool : ActionEntity
{
    private ActionEntityData data;
    public override void _Ready()  
    {
        Initialize(data);
        SetChildNodes();
    }

    public override void Initialize(ActionEntityData d)
    {
        
    }
}
