using Godot;
using System;

public partial class HotSlot : InventorySlot
{
    [Export] private int index;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey)
        {
            if (Input.IsActionPressed($"slot{index}"))
                if (Item != null)
                {
                    Item.count++;
                    UpdateSlot();
                }
        }
    }
}
