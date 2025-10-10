using Godot;
using System;

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
        if (!CursorItem.HasItem() && Item != null)
        {
            // Pick up whole stack
            CursorItem.HeldItem = Item;
            ClearItem();
        }
        else if (CursorItem.HasItem())
        {
            if (Item == null)
            {
                // Place whole stack
                SetItem(CursorItem.HeldItem);
                CursorItem.Clear();
            }
            else if (Item.ItemData.ItemId == CursorItem.HeldItem.ItemData.ItemId && 
                     Item.ItemData.DisplayName == CursorItem.HeldItem.ItemData.DisplayName)
            {
                //Add whole stack or as much as fits, no split since place whole stack is requested
                int space = Item.ItemData.MaxStack - Item.Count;
                int toAdd = Math.Min(space, CursorItem.HeldItem.Count);
                Item.Count += toAdd;
                CursorItem.HeldItem.Count -= toAdd;
                if (CursorItem.HeldItem.Count <= 0)
                    CursorItem.Clear();
                UpdateSlot();
            }
            else
            {
                //Swap stacks
                var temp = Item;
                SetItem(CursorItem.HeldItem);
                CursorItem.HeldItem = temp;
                UpdateSlot();
            }
        }
    }

    private void RightClick()
    {
        if (!CursorItem.HasItem() && Item != null)
        {
            //Pick up half stack
            int half = (Item.Count + 1) / 2;
            CursorItem.HeldItem = new ItemStack(Item.ItemData, half);
            Item.Count -= half;
            if (Item.Count <= 0)
                ClearItem();
            else
                UpdateSlot();
        }
        else if (CursorItem.HasItem())
        {
            if (Item == null)
            {
                //Place one item from cursor into empty slot
                SetItem(new ItemStack(CursorItem.HeldItem.ItemData, 1));
                CursorItem.HeldItem.Count--;
                if (CursorItem.HeldItem.Count <= 0)
                    CursorItem.Clear();
            }
            else if (Item.ItemData.ItemId == CursorItem.HeldItem.ItemData.ItemId && 
                     Item.ItemData.DisplayName == CursorItem.HeldItem.ItemData.DisplayName)
            {
                //Place one item if it can fit
                if (Item.Count < Item.ItemData.MaxStack)
                {
                    Item.Count++;
                    CursorItem.HeldItem.Count--;
                    if (CursorItem.HeldItem.Count <= 0)
                        CursorItem.Clear();
                    UpdateSlot();
                }
            }
            //If different item, do nothing
        }
    }
}
