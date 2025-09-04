using Godot;
using System;

public partial class Swordtest : Weapon
{
    [Export] private Sprite2D sprite { get; set; }
    [Export] private Area2D hitbox { get; set; }
    
    private Tween stabTween;
    private CollisionShape2D stabCollider;
    private bool isAttacking = false;
    
    //this weapons properties
    private float stabDistance = 10f;
    private float _stabDamage = 10;
    private float _crit = 15; //percentage chance to crit
    private float _knockback = 10;

    public override void _Ready()
    {
        damage = _stabDamage;
        critChance = _crit;
        stabCollider = GetNode<CollisionShape2D>("hitbox/collider");
        knockback = _knockback;
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

    public override void _Process(double delta)
    {
        if (isAttacking)
        {
            Visible = true;
            stabCollider.Disabled = false;
        }
        else
        {
            Visible = false;
            stabCollider.Disabled = true;
        }
    }
    
    public override void Stab()
    {
        //let the stab finish so 1) no spamming 2) the sword doesn't fly off
        if (stabTween != null)
            return;
        
        var origin = Position;
        var mouseDir = (GetGlobalMousePosition() - GlobalPosition).Normalized();
        hitDir = mouseDir;

        stabTween = GetTree().CreateTween().BindNode(this);
        stabTween.TweenProperty(this, "position", origin + (mouseDir * stabDistance), 0.1f);
        stabTween.TweenProperty(this, "position", origin, 0.1f).SetDelay(0.1f);
        
        isAttacking = true;
        
        //make it null after finishing
        stabTween.Finished += () =>
        {
            isAttacking = false;
            stabTween = null;
        };
    }
}
