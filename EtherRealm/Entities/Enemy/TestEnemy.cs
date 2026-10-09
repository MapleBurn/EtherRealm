using System;
using EtherRealm.Entities.Survivor;
using EtherRealm.Entities.Visual;
using Godot;

namespace EtherRealm.Entities.Enemy;

public partial class TestEnemy : LivingEntity
{
    public enum State
    {
        Idle,
        Wander,
        Chase
    }
    
    private int _health;

    [Export] public float MinIdleTime = 1.0f; // seconds
    [Export] public float MaxIdleTime = 3.0f;
    [Export] public float MinWanderTime = 1.0f;
    [Export] public float MaxWanderTime = 2.5f;

    private State _state = State.Idle;
    private float _stateTimer = 0f;
    private float _stateDuration = 0f;
    private int _wanderDir = 1; // 1 = right, -1 = left
    private Player _player = null;
    [Export] private RayCast2D _raycast;
    [Export] private Sprite2D _sprite;

    public override void _Ready()
    {
        _health = MaxHealth;
        EnterState(State.Idle);
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        Vector2 velocity = Velocity;
        Vector2 direction = Vector2.Zero;

        if (!IsOnFloor())
            velocity.Y += GetGravity().Y * dt;

        if (_raycast.IsColliding() && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        switch (_state)
        {
            case State.Idle:
                direction = Idle(dt);
                break;
            case State.Wander:
                direction = Wander(dt);
                break;
            case State.Chase:
                direction = Chase();
                break;
        }

        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * MoveSpeed;
            if (direction < Vector2.Zero) // left
            {
                _sprite.FlipH = true;
                _raycast.Position = new Vector2(-16, 18);
                _raycast.Rotation = Mathf.DegToRad(180);
            }
            else // right
            {
                _sprite.FlipH = false;
                _raycast.Position = new Vector2(16, 18);
                _raycast.Rotation = Mathf.DegToRad(0);
            }
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, MoveSpeed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;
        var scene = GD.Load<PackedScene>(FloatingTextPath);
        var text = scene.Instantiate<FloatingText>();
        AddChild(text);
        text.SetText(amount, FloatingText.DamageType.Damage, false);
        
        if (_health <= 0)
        {
            QueueFree();
        }
    }

    #region StateMachine
    private Vector2 Idle(float dt)
    {
        _stateTimer += dt;

        if (_stateTimer >= _stateDuration)
            EnterState(State.Wander);

        return Vector2.Zero;
    }

    private Vector2 Wander(float dt)
    {
        _stateTimer += dt;

        if (_stateTimer >= _stateDuration)
        {
            // 50 % chance to rest in Idle, 50 % to pick a new direction
            EnterState(Random.Shared.NextDouble() < 0.5 ? State.Idle : State.Wander);
        }


        return new Vector2(1, 0) * _wanderDir;
    }

    private Vector2 Chase()
    {
        if (_player != null)
        {
            float dir = Mathf.Sign(_player.GlobalPosition.X - GlobalPosition.X);
            return new Vector2(1, 0) * dir;
        }

        EnterState(State.Wander);
        return Vector2.Zero;
    }

    private void EnterState(State newState)
    {
        _state = newState;
        _stateTimer = 0f;

        switch (newState)
        {
            case State.Idle:
                _stateDuration = RandomRange(MinIdleTime, MaxIdleTime);
                break;

            case State.Wander:
                _stateDuration = RandomRange(MinWanderTime, MaxWanderTime);
                _wanderDir = Random.Shared.NextDouble() < 0.5 ? 1 : -1;
                break;

            case State.Chase:
                _stateDuration = 0f;
                break;
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            _player = player;
            EnterState(State.Chase);
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is Player)
        {
            _player = null;
            EnterState(State.Wander);
        }
    }

    private float RandomRange(float min, float max)
        => min + (float)Random.Shared.NextDouble() * (max - min);

    #endregion
}