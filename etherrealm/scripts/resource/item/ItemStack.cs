using Godot;
using System;

namespace EtherRealm.scripts.resource.item;
public class ItemStack
{
    [Export] public ItemData ItemData { get; set; }
    public int Count { get; set; }

    public ItemStack(ItemData iData, int count)
    {
        ItemData = iData;
        Count = count;
    }
    public ItemStack(ItemData iData, int? count)
    {
	    ItemData = iData;
        Count = count ?? 1;
    }
}
