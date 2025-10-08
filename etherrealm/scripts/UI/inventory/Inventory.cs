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

    public bool TryFit(ItemStack item)
    {
        if (item == null)
            return false;
        
        foreach (Slot slot in grid.GetChildren())
        {
            if (slot.Item !=null && slot.Item.ItemData.ItemId == item.ItemData.ItemId)
            {
                slot.AddToSlot(item);
                return true;
            }
        }
        foreach (Slot slot in grid.GetChildren())
        {
            if (slot.Item == null)
            {
                slot.SetItem(item);
                return true;
            }
        }
        return false;
    }
}
