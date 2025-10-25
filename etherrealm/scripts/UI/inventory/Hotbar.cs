using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using EtherRealm.scripts.entity;
using EtherRealm.scripts.resource.item;

namespace EtherRealm.scripts.UI.inventory;
public partial class Hotbar : Panel
{
    //children
    private HBoxContainer container;
    private Label activeLabel;
    [Export] private Player player;
    private Hand hand;
    [Export] private Inventory inventory;
    
    private List<Slot> slots = new List<Slot>();
    //private ItemStack currentItem => (index >= 0 && index < slots.Count) ? slots[index].Item : null;
    private int index = 0;

    public override void _Ready()
    {
        container = GetNode<HBoxContainer>("HBoxContainer");
        activeLabel = GetNode<Label>("activeLabel");
        hand = player.hand;
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
                var slot = slots[index];
                ChangeActiveItem(slot);
            }
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                if (index == slots.Count - 1)
                    return;
                index++;
                var slot = slots[index];
                ChangeActiveItem(slot);
            }
        }
        else if (@event is InputEventKey keyEvent && keyEvent.IsPressed())
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (Input.IsActionPressed($"slot{i + 1}"))
                {
                    index = i;
                    var slot = slots[index];
                    ChangeActiveItem(slot);
                    break;
                }
            }
        }
        
    }

    public override void _Draw()
    {
        Vector2 offset = new Vector2(5, 2); //why this value? idk but it works
        DrawRect(new Rect2(slots[index].Position + offset, slots[index].Size), Color.Color8(255,255,255), false, 4);
    }

    private void ChangeActiveItem(Slot slot)
    {
        var invSlot = inventory.inventorySlots.FirstOrDefault(a => a.index == slot.index);
        if (slot.Item == null)
        {
            activeLabel.Visible = false;
            hand.UpdateActionEntity(invSlot);
            QueueRedraw();
            return;
        }
        
        activeLabel.Visible = true;
        activeLabel.Text = slot.Item.ItemData.DisplayName;


        hand.UpdateActionEntity(invSlot);
        
        QueueRedraw(); //draws the rectangle around the selected slot
    }  
    
    
    private void SetSlots()
    {
        slots = container.GetChildren().OfType<Slot>().ToList();
    }

    
    public void OnSlotUpdated(InventorySlot slot)
    {
        if (slot.index > slots.Count)
            return;
        
        if (slot.index > 0 && slot.index <= slots.Count)
        {
            slots[slot.index-1].SetItem(slot.Item);
            //slots[slot.index-1].UpdateSlot();
        }

        if (slot.index-1 == index)
        {
            ChangeActiveItem(slots[index]);
        }
    }

}
