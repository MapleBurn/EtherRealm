using Godot;
using System;

public partial class InventorySlot : Button
{
    public Item Item;
    // Set these in the editor or with [Export] if you want.
    protected TextureRect IconRect;
    protected Label CountLabel;

    public override void _Ready()
    {
        // Get nodes if not assigned in editor.
        if (IconRect == null)
            IconRect = GetNode<TextureRect>("icon");
        if (CountLabel == null)
            CountLabel = GetNode<Label>("countLabel");
        
        PackedScene itemScene = GD.Load<PackedScene>("res://scenes/items/item.tscn");
        Item item = itemScene.Instantiate<Item>();
        item.Initialize();
        SetSlot(item);
        
        UpdateSlot();
    }

    public void SetSlot(Item item)
    {
        Item = item;
        UpdateSlot();
    }

    public void ClearItem()
    {
        Item = null;
        UpdateSlot();
    }

    protected void UpdateSlot()
    {
        if (Item != null)
        {
            IconRect.Texture = Item.sprite.Texture;
            IconRect.Visible = true;
            CountLabel.Text = Item.count > 1 ? Item.count.ToString() : "";
            CountLabel.Visible = true;
            EquipmentManager.LoadEntity(Item.EntityData);
        }
        else
        {
            IconRect.Visible = false;
            CountLabel.Visible = false;
        }
    }

    // Drag & Drop
    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (Item == null)
            return new Variant();

        // Show dragged item texture
        var preview = new TextureRect();
        preview.Texture = Item.sprite.Texture;
        preview.CustomMinimumSize = IconRect.Size;
        SetDragPreview(preview);
        
        return this;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return data.Obj is InventorySlot;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (data.Obj is InventorySlot draggedSlot)
        {
            if (Item != null && draggedSlot.Item != null && Item.Name == draggedSlot.Item.Name)
            {
                AddToSlot(draggedSlot.Item);
                draggedSlot.ClearItem();
            }
            else
            {
                // Different item, swap between slots
                var tempItem = Item;
                SetSlot(draggedSlot.Item);
                draggedSlot.SetSlot(tempItem);
            }
        }
    }

    public void AddToSlot(Item item)
    {
        Item.count += item.count;
        UpdateSlot();
    }
}
