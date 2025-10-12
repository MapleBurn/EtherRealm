using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Hotbar : Panel
{
    [Signal] public delegate void SlotSelectedEventHandler(ItemData itemData, int count);
    
    //children
    private HBoxContainer container;
    private Label activeLabel;
    [Export] private Inventory inventory;
    
    private List<Slot> slots = new List<Slot>();
    //private ItemStack currentItem => (index >= 0 && index < slots.Count) ? slots[index].Item : null;
    private int index = 0;

    public override void _Ready()
    {
        container = GetNode<HBoxContainer>("HBoxContainer");
        activeLabel = GetNode<Label>("activeLabel");
        SetSlots();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsPressed())
        {
            var item = slots[index].Item;
            
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
            {
                if (index == 0)
                    return;
                index--;
                ChangeActiveItem(item);
                QueueRedraw(); //draws the rectangle around the selected slot
            }
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                if (index == slots.Count - 1)
                    return;
                index++;
            }
            ChangeActiveItem(item);
        }
        else if (@event is InputEventKey keyEvent && keyEvent.IsPressed())
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (Input.IsActionPressed($"slot{i + 1}"))
                {
                    index = i;
                    break;
                }
            }
            var item = slots[index].Item;
            ChangeActiveItem(item);
        }
        
    }

    public override void _Draw()
    {
        Vector2 offset = new Vector2(5, 2); //why this value? idk but it works
        DrawRect(new Rect2(slots[index].Position + offset, slots[index].Size), Color.Color8(255,255,255), false, 4);
    }

    private void ChangeActiveItem(ItemStack item)
    {
        if (item == null)
        {
            activeLabel.Visible = false;
            QueueRedraw();
            return;
        }
        
        activeLabel.Visible = true;
        activeLabel.Text = item.ItemData.DisplayName;
        
        EmitSignal(SignalName.SlotSelected, item.ItemData, item.Count);
        QueueRedraw(); //draws the rectangle around the selected slot
    }  
    
    
    private void SetSlots()
    {
        slots = container.GetChildren().OfType<Slot>().ToList();
    }

    
    public void OnSlotUpdated(int slotIndex, ItemStack item)
    {
        if (slotIndex > 0 && slotIndex <= slots.Count)
        {
            slots[slotIndex-1].SetItem(item);
            slots[slotIndex-1].UpdateSlot();
        }
    }

}
