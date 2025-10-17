using Godot;
using System;

namespace EtherRealm.scripts.resource;
public partial class ToolData : ActionEntityData
{
    [Export] public int MiningLevel { get; set; }
    
    public ToolData()
    {
        Type = "tool";
    }
}
