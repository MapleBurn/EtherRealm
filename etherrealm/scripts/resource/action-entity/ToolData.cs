using Godot;
using System;

namespace EtherRealm.scripts.resource;
public partial class ToolData : action_entity.ActionEntityData
{
    [Export] public int MiningLevel { get; set; }
    
    public ToolData()
    {
        Type = "tool";
    }
}
