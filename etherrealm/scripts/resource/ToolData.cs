using Godot;
using System;

public partial class ToolData : ActionEntityData
{
    [Export] public int MiningLevel { get; set; }
    
    public ToolData()
    {
        Type = "tool";
    }
}
