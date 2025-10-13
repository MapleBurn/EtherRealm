using Godot;
using System;

public partial class Tool : ActionEntity
{
    public override void _Ready()  
    {  
        Initialize();
        SetChildNodes();
    }

    public void Initialize()
    {
        
    }
}
