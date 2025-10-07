using Godot;
using System;

public partial class SlotBase : Button
{
    protected TextureRect IconRect;
    protected Label CountLabel;
    protected int index;
    public override void _Ready()
    {
        if (IconRect == null)
            IconRect = GetNode<TextureRect>("icon");
        if (CountLabel == null)
            CountLabel = GetNode<Label>("countLabel");
        FocusMode = Control.FocusModeEnum.None;
        int.TryParse(Name.ToString().Split("Slot")[1].Trim(), out int index); //get index?
    }
}
