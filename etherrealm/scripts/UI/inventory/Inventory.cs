using Godot;
using System;

public partial class Inventory : Panel
{
    [Export] public Panel hotbar;
    [Export] private GridContainer grid;
    private bool isInventoryOpen;
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey)
        {
            if (Input.IsActionPressed($"inventory"))
            {
                isInventoryOpen = !isInventoryOpen;
                Visible = isInventoryOpen;
            }
        }
    }

    public bool TryFit(Item item)
    {
        foreach (InventorySlot slot in grid.GetChildren())
        {
            if (item == null) return false;
            if (slot.Item !=null && slot.Item.ItemId == item.ItemId)
            {
                slot.AddToSlot(item);
                return true;
            }
        }
        foreach (InventorySlot slot in grid.GetChildren())
        {
            if (slot.Item == null)
            {
                slot.SetSlot(item);
                return true;
            }
        }
        return false;
    }
}
