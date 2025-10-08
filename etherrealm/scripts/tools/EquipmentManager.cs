using Godot;
using System;

public static class EquipmentManager
{
    private static Player player;

    public static void Initialize(Player p)
    {
        player = p;
    }
    public static void LoadEntity(ActionEntityData eData)
    {
        if  (eData == null)
            return;
        
    }
}
