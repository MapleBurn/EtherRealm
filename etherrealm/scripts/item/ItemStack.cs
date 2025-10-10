using Godot;
using System;

public partial class ItemStack
{
    [Export] public ItemData ItemData { get; set; }
    public int Count { get; set; } = 5;

    public ItemStack(ItemData iData)
    {
        ItemData = iData;
    }
    public ItemStack(ItemData iData, int? count)
    {
	    ItemData = iData;
        Count = count ?? 1;
    }
}
