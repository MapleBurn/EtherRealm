using Godot;
using System;

public partial class Slot : Panel
{
	public ItemStack Item;
	private TextureRect icon;
	private Label countLabel;

	public override void _Ready()
	{
		icon = GetNode<TextureRect>("icon");
		countLabel = GetNode<Label>("count");
	}
	
	public void SetItem(ItemStack value)
	{
		Item = value;
		if (Item == null)
		{
			UpdateSlot();
			return;
		}

		icon.Texture = value.ItemData.Texture;
		UpdateSlot();
	}
	
	private void UpdateSlot()
	{
		if (Item != null)
		{
			icon.Texture = Item.ItemData.Texture;
			icon.Visible = true;
			countLabel.Text = Item.Count.ToString();
			countLabel.Visible = (Item.Count > 1);
		}
		else
		{
			icon.Visible = false;
			countLabel.Visible = false;
		}
	}
	
	public void AddToSlot(ItemStack item)
	{
		Item.Count = Item.Count + item.Count > Item.ItemData.MaxStack ? Item.ItemData.MaxStack : item.Count + Item.Count;
		UpdateSlot();
	}
	
	public void ClearItem()
	{
		Item = null;
		UpdateSlot();
	}
	
	// Drag & Drop
	public override Variant _GetDragData(Vector2 atPosition)
	{
		if (Item == null)
			return new Variant();

		// Show dragged item texture
		var preview = new TextureRect();
		preview.Texture = Item.ItemData.Texture;
		preview.CustomMinimumSize = icon.Size;
		SetDragPreview(preview);
        
		return this;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		return data.Obj is Slot;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		if (data.Obj is Slot draggedSlot)
		{
			if (draggedSlot == this)  
				return;
			
			if (Item != null && draggedSlot.Item != null && Item.ItemData.ItemId == draggedSlot.Item.ItemData.ItemId)
			{
				AddToSlot(draggedSlot.Item);
				draggedSlot.ClearItem();
			}
			else
			{
				//different item, swap between slots
				var tempItem = Item;
				SetItem(draggedSlot.Item);
				draggedSlot.SetItem(tempItem);
			}
		}
	}
}
