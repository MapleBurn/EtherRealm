using Godot;
using System;

public partial class HotSlot : Panel
{
    private TextureRect icon;
    private Item item;
    [Export] private int index;

    public override void _Ready()
    {
        PackedScene itemScene = GD.Load<PackedScene>("res://scenes/items/test_item.tscn");
        item = itemScene.Instantiate<Item>();
        item.Inicialize();
        icon = GetNode<TextureRect>("icon");
        GetTree();
        
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey pressedButton)
        {
            if (Input.IsActionPressed($"slot{index}"))
                icon.Texture = item.sprite.Texture;
        }
    }
}
