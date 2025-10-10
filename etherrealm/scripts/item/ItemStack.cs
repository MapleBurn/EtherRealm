using Godot;
using System;

public partial class ItemStack
{
    [Export] public ItemData ItemData { get; set; }
    public int Count { get; set; } = 1;

    public ItemStack(ItemData iData)
    {
	    ItemData = iData;
    }
    
	public ItemStack Clone()
	{
		return new ItemStack(ItemData)  
		{  
			ItemData = this.ItemData,  
			Count = this.Count  
		};
	}
}
