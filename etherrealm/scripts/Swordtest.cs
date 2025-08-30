using Godot;
using System;

public partial class Swordtest : Weapon
{
    [Export] private Sprite2D sprite { get; set; }
    
    private Tween stabTween;
    private float stabDistance = 10f;
    
    public override void _Input(InputEvent @event)
    {
        //Input(@event);
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.IsActionPressed("MouseLeftButton"))
            {
                //otočí Node2D tak, aby směřoval na pozici myši (globální souřadnice)
                LookAt(GetGlobalMousePosition());
                Stab();
            }
        }
    }

    public override void Stab()
    {
        //let the stab finish so 1) no spamming 2) the sword doesn't fly off
        if (stabTween != null)
            return;
        
        var origin = GlobalPosition;
        var mouseDir = (GetGlobalMousePosition() - origin).Normalized();

        stabTween = GetTree().CreateTween().BindNode(this);
        stabTween.TweenProperty(this, "global_position", origin + (mouseDir * stabDistance), 0.1f);
        stabTween.TweenProperty(this, "global_position", origin, 0.1f).SetDelay(0.1f);
        
        //make it null after finishing
        stabTween.Finished += () => stabTween = null;
    }
}
