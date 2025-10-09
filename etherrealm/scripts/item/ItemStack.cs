using Godot;
using System;

public partial class ItemStack : Resource
{
    [Export] public ItemData ItemData { get; set; }
    public int Count { get; set; } = 1;
    
	public ItemStack Clone()
	{
		return new ItemStack  
		{  
			ItemData = this.ItemData,  
			Count = this.Count  
		};
	}
}
