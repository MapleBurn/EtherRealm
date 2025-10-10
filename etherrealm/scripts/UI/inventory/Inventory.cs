using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Inventory : Panel
{
    [Export] public Hotbar hotbar;
    [Export] public GridContainer grid;
    [Export] public CursorItem cursorItem;
    private bool isInventoryOpen;
    public List<InventorySlot> inventorySlots;
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey)
        {
            if (Input.IsActionJustPressed($"inventory"))
            {
                isInventoryOpen = !isInventoryOpen;
                Visible = isInventoryOpen;
            }
        }
    }

    public override void _Ready()
    {
        Visible = false;
        inventorySlots = grid.GetChildren().OfType<InventorySlot>().ToList();
        
        foreach (var slot in inventorySlots)
        {
            slot.Connect(InventorySlot.SignalName.SlotUpdated, new Callable(this, nameof(OnSlotUpdated)));
        }
    }
    
    private void OnSlotUpdated(int slotIndex)
    {
        // Only update hotbar if the updated slot is in the first row
        if (slotIndex <= grid.Columns)
        {
            // Pass updated item to hotbar slot
            hotbar.OnSlotUpdated(slotIndex, inventorySlots[slotIndex-1].Item);
        }
    }
    
    public int TryFit(ItemStack item)
    {
        int amountLeft = item.Count;
        if (amountLeft <= 0)
            return 0;

        foreach (var slot in inventorySlots)
        {
            //First add to existing stacks
            if (slot.Item != null && slot.Item.ItemData.ItemId == item.ItemData.ItemId && slot.Item.ItemData.DisplayName == item.ItemData.DisplayName)
            {
                int space = slot.Item.ItemData.MaxStack - slot.Item.Count;
                int toAdd = Math.Min(space, amountLeft);
                slot.Item.Count += toAdd;
                slot.UpdateSlot();
                amountLeft -= toAdd;
                if (amountLeft == 0)
                    return 0;
            }
        }

        foreach (var slot in inventorySlots)
        {
            //Fill empty slots
            if (slot.Item == null)
            {
                int toAdd = Math.Min(item.ItemData.MaxStack, amountLeft);
                slot.SetItem(new ItemStack(item.ItemData, toAdd));
                amountLeft -= toAdd;
                if (amountLeft == 0)
                    return 0;
            }
        }

        return amountLeft; //Return unfitted item count
    }
}
