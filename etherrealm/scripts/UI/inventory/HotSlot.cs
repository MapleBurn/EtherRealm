using Godot;
using System;

public partial class HotSlot : SlotBase
{
    public override void _Ready()
    {
        base._Ready();
        FocusMode = Control.FocusModeEnum.Click;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey)
        {
            if (Input.IsActionPressed($"slot{index}"))
            {
                GrabFocus();
            }
        }
    }
    
    protected void UpdateHotSlot()
    {
        //Logic to update the contents of the hotbar slot
    }
}
