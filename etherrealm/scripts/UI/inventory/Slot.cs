using Godot;
using System;
using EtherRealm.scripts.resource.item;

namespace EtherRealm.scripts.UI.inventory;
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
		if (!int.TryParse(Name.ToString().Split("slot")[1], out index))
			index = 0;
	}
	
	public void SetItem(ItemStack value)
	{
		Item = value;
		UpdateSlot();
	}
	
	public void RemoveFromSlot(int count)
	{
		Item.Count = Item.Count - count;
		if (Item.Count <= 0)
			ClearItem();
		UpdateSlot();
	}
	
	public void ClearItem()
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
