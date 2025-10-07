using Godot;
using System;

public partial class Item : CharacterBody2D
{
    //other nodes and children
    private CollisionShape2D collider;
    public Sprite2D sprite;
    [Export] private ItemData itemData;
    
    //resource data
    public string ItemId;
    public string DisplayName;
    public int MaxStack;

    public int count = 1;

    public override void _Ready()
    {
        Initialize();
        CollisionMask = 2;  //make it collide with ground (2)
    }

    public void Initialize()
    {
        collider = GetNode<CollisionShape2D>("collider");
        sprite = GetNode<Sprite2D>("Sprite2D");
        
        ItemId = itemData.ItemId;
        DisplayName = itemData.DisplayName;
        MaxStack = itemData.MaxStack;
        sprite.Texture = itemData.Icon;
    }

    public void PickUp()
    {
        Inventory inventory = GetNode<Inventory>("../UI/inventory");
        if (inventory.TryFit(this)) 
            QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }
            
        Velocity = velocity;
        MoveAndSlide();
    }
}
