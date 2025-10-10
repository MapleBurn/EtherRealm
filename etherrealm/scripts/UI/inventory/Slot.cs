using Godot;
using System;

public partial class Slot : Panel
{
	public ItemStack Item;
	protected TextureRect icon;
	protected Label countLabel;
	public int index;

	public override void _Ready()
	{
		Inicialize();
	}

	public void Inicialize()
	{
		icon = GetNode<TextureRect>("icon");
		countLabel = GetNode<Label>("count");
		int.TryParse(Name.ToString().Split("slot")[1], out index);
	}
	
	public void SetItem(ItemStack value)
	{
		Item = value;
		UpdateSlot();
	}
	
	public void AddToSlot(ItemStack item)
	{
		Item.Count = Math.Min(Item.Count + item.Count, Item.ItemData.MaxStack);
		UpdateSlot();
	}
	
	protected void ClearItem()
	{
		Item = null;
		UpdateSlot();
	}
	
	public virtual void UpdateSlot()
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
}
