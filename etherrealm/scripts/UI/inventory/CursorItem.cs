using Godot;
using System;

public partial class CursorItem : Node2D
{
    public ItemStack HeldItem = null;

    public bool HasItem()
    {
        return HeldItem != null && HeldItem.Count > 0;
    }
    
    private TextureRect icon;
    private Label countLabel;
    
    public void Refresh()
    {
        if (HeldItem != null)
        {
            icon.Texture = HeldItem.ItemData.Texture;
            countLabel.Text = HeldItem.Count.ToString();
            return;
        }
        icon.Texture = null;
        countLabel.Text = "";
    }
    public override void _Process(double delta)
    {
        if(Visible) GlobalPosition = GetViewport().GetMousePosition();
    }
    public override void _Ready()
    {
        icon = GetNode<TextureRect>("icon");
        countLabel = GetNode<Label>("count");
    }
}
