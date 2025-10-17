using Godot;
using System;
using EtherRealm.scripts.resource.item;

namespace EtherRealm.scripts.UI.inventory;
public partial class InventorySlot : Slot
{
	[Signal]
	public delegate void SlotUpdatedEventHandler(int slotIndex);

	private static Inventory inventory;
	public override void _Ready()
	{
		Inicialize();
		if (inventory == null) inventory = (Inventory)GetParent().GetParent();
	}
	public override void UpdateSlot()
	{
		base.UpdateSlot();
		if (index <= inventory.grid.Columns) EmitSignal(SignalName.SlotUpdated, index);
	}
	
    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsPressed())
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
                LeftClick();
            else if (mouseEvent.ButtonIndex == MouseButton.Right)
                RightClick();
        }
    }

    private void LeftClick()
    {
        if (!inventory.cursorItem.HasItem() && Item != null)
        {
            // Pick up whole stack
            inventory.cursorItem.HeldItem = Item;
            ClearItem();
        }
        else if (inventory.cursorItem.HasItem())
        {
            if (Item == null)
            {
                // Place whole stack
                SetItem(inventory.cursorItem.HeldItem);
                inventory.cursorItem.HeldItem = null;
            }
            else if (Item.ItemData.ItemId == inventory.cursorItem.HeldItem.ItemData.ItemId && 
                     Item.ItemData.DisplayName == inventory.cursorItem.HeldItem.ItemData.DisplayName)
            {
                //Add whole stack or as much as fits, no split since place whole stack is requested
                int space = Item.ItemData.MaxStack - Item.Count;
                int toAdd = Math.Min(space, inventory.cursorItem.HeldItem.Count);
                Item.Count += toAdd;
                inventory.cursorItem.HeldItem.Count -= toAdd;
                if (inventory.cursorItem.HeldItem.Count <= 0)
                    inventory.cursorItem.HeldItem = null;
                UpdateSlot();
            }
            else
            {
                //Swap stacks
                var temp = Item;
                SetItem(inventory.cursorItem.HeldItem);
                inventory.cursorItem.HeldItem = temp;
                UpdateSlot();
            }
        }
        inventory.cursorItem.Refresh();
    }

    private void RightClick()
    {
        if (!inventory.cursorItem.HasItem() && Item != null)
        {
            //Pick up half stack
            int half = (Item.Count + 1) / 2;
            inventory.cursorItem.HeldItem = new ItemStack(Item.ItemData, half);
            Item.Count -= half;
            if (Item.Count <= 0)
                ClearItem();
            else
                UpdateSlot();
        }
        else if (inventory.cursorItem.HasItem())
        {
            if (Item == null)
            {
                //Place one item from cursor into empty slot
                SetItem(new ItemStack(inventory.cursorItem.HeldItem.ItemData, 1));
                inventory.cursorItem.HeldItem.Count--;
                if (inventory.cursorItem.HeldItem.Count <= 0)
                    inventory.cursorItem.HeldItem = null;
                UpdateSlot();
            }
            else if (Item.ItemData.ItemId == inventory.cursorItem.HeldItem.ItemData.ItemId && 
                     Item.ItemData.DisplayName == inventory.cursorItem.HeldItem.ItemData.DisplayName)
            {
                //Place one item if it can fit
                if (Item.Count < Item.ItemData.MaxStack)
                {
                    Item.Count++;
                    inventory.cursorItem.HeldItem.Count--;
                    if (inventory.cursorItem.HeldItem.Count <= 0)
                        inventory.cursorItem.HeldItem = null;
                    UpdateSlot();
                }
            }
            //If different item, do nothing
        }
        inventory.cursorItem.Refresh();
    }
}
