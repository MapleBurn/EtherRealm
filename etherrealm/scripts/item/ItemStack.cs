using Godot;
using System;

public partial class ItemStack : Node
{
    [Export] public ItemData ItemData { get; set; }
    public int Count { get; set; } = 1;
    
	public override void _Ready()
	{
		
	}
}
