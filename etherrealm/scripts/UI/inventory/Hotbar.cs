using Godot;
using System;
using System.Collections.Generic;

public partial class Hotbar : Panel
{
    [Signal] public delegate void SlotSelectedEventHandler(ItemStack item);
    
    private HBoxContainer container;
    
    private List<Slot> slots = new List<Slot>();
    private ItemStack currentlyEquipped;
    private int index = 0;

    public override void _Ready()
    {
        container = GetNode<HBoxContainer>("HBoxContainer");
        SetSlots();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsPressed())
        {
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
            {
                if (index == 0)
                    return;
                index--;
                currentlyEquipped = slots[index].Item;
                EmitSignal(SignalName.SlotSelected, currentlyEquipped);
                QueueRedraw(); //draws the rectangle around the selected slot
            }
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                if (index == slots.Count - 1)
                    return;
                index++;
                currentlyEquipped = slots[index].Item;
                EmitSignal(SignalName.SlotSelected, currentlyEquipped);
                QueueRedraw();
            }
        }
        /*else if (@event is InputEventKey keyEvent && keyEvent.IsPressed()) //consider adding switching slots using keys
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (Input.IsActionPressed($"slot{i + 1}")) //AI generated - check if this works
                {
                    index = i;
                    currentlyEquipped = slots[index].item;
                    EmitSignal(SignalName.SlotSelected, currentlyEquipped);
                    break;
                }
            }
        }*/
    }

    public override void _Draw()
    {
        Vector2 offset = new Vector2(5, 2); //why this value? idk but it works
        DrawRect(new Rect2(slots[index].Position + offset, slots[index].Size), Color.Color8(255,255,255), false, 4);
    }
    
    private void SetSlots()
    {
        var s = container.GetChildren();
        int count = container.GetChildren().Count;
        for (int i = 0; i < count; i++)
        {
            if (s[i] is Slot slot)
            {
                slots.Add(slot);
            }
        }
    }


}
