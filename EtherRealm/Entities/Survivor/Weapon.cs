using Godot;

namespace EtherRealm.Entities.Survivor;

public partial class Weapon : Area2D
{
    [Export] public float SideOffset = 16f;      // half of the player's width
    [Export] public float LungeDistance = 40f;
    [Export] public float LungeTime = 0.08f;
    [Export] public float ReturnTime = 0.15f;
    [Export] public float Cooldown = 0.05f;
    [Export] private Sprite2D _sprite;

    private Vector2 _originalPosition;
    private bool _isLunging = false;
    private Tween _tween;

    public override void _Ready()
    {
        
    }

    public void Flip(bool isFacingLeft)
    {
        _originalPosition = new Vector2(isFacingLeft ? -SideOffset : SideOffset, 0f);
        Position = _originalPosition;
        
        _sprite.FlipV = isFacingLeft;
    }

    public override void _Process(double delta)
    {
        if (!_isLunging)
        {
            LookAt(GetGlobalMousePosition());
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouse
            && mouse.Pressed
            && mouse.ButtonIndex == MouseButton.Left
            && !_isLunging)
        {
            Lunge();
        }
    }

    private void Lunge()
    {
        _isLunging = true;

        Vector2 direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();
        Vector2 target = _originalPosition + direction * LungeDistance;

        _tween?.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(this, "position", target, LungeTime)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
        _tween.TweenProperty(this, "position", _originalPosition, ReturnTime)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.In);
        _tween.TweenInterval(Cooldown);
        _tween.Finished += () => _isLunging = false;
    }
}