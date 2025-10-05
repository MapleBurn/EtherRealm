using Godot;
using System;

public partial class Inventory : Panel
{
    public bool InventoryOpen;
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey)
        {
            if (Input.IsActionPressed($"inventory"))
            {
                InventoryOpen = !InventoryOpen;
                Visible = InventoryOpen;
            }
        }
    }
}
