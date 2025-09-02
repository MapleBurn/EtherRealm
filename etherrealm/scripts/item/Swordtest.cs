using Godot;
using System;

public partial class Swordtest : Weapon
{
    [Export] private Sprite2D sprite { get; set; }
    
    private Tween stabTween;
    private float stabDistance = 10f;
    private int _stabDamage = 10;
    private int _crit = 15; //percentage chance to crit

    public override void _Ready()
    {
        damage = _stabDamage;
        critChance = _crit;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.IsActionPressed("MouseLeftButton"))
            {
                //rotate to mouse
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
        
        var origin = Position;
        var mouseDir = (GetGlobalMousePosition() - GlobalPosition).Normalized();

        stabTween = GetTree().CreateTween().BindNode(this);
        stabTween.TweenProperty(this, "position", origin + (mouseDir * stabDistance), 0.1f);
        stabTween.TweenProperty(this, "position", origin, 0.1f).SetDelay(0.1f);
        
        //make it null after finishing
        stabTween.Finished += () => stabTween = null;
    }
}
