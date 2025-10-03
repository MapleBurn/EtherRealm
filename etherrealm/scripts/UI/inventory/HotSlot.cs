using Godot;
using System;

public partial class HotSlot : Panel
{
    private TextureRect icon;
    private string item;

    public override void _Ready()
    {
        icon = GetNode<TextureRect>("icon");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey pressedButton)
        {
            //if (Input.IsActionPressed("slot1"))
                //NOT FISHING
        }
    }
}
