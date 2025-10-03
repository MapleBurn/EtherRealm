using Godot;
using System;

public partial class HotSlot : Panel
{
    private TextureRect icon;
    private Item item;

    public override void _Ready()
    {
        PackedScene itemScene = GD.Load<PackedScene>("res://scenes/items/test_item.tscn");
        item = itemScene.Instantiate<Item>();
        
        icon = GetNode<TextureRect>("icon");
        GetTree();
        
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey pressedButton)
        {
            if (Input.IsActionPressed("slot1"))
                icon.Texture = item.sprite.Texture;
            else if (Input.IsActionPressed("slot2"))
                icon.Texture = item.sprite.Texture;
            else if (Input.IsActionPressed("slot3"))
                icon.Texture = item.sprite.Texture;
            else if (Input.IsActionPressed("slot4"))
                icon.Texture = item.sprite.Texture;
            else if (Input.IsActionPressed("slot5"))
                icon.Texture = item.sprite.Texture;
            else if (Input.IsActionPressed("slot6"))
                icon.Texture = item.sprite.Texture;
        }
    }
}
