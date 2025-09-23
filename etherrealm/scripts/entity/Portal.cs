using Godot;
using System;

public partial class Portal : Area2D
{
    private void OnPortalEntered(Area2D area)
    {
        if (area.GetParent() is Player player)
        {
            player.GlobalPosition = new  Vector2(1350, -350);
        }
    }
}
