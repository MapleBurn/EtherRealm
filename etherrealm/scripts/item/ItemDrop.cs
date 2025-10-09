using Godot;
using System;

public partial class ItemDrop : CharacterBody2D
{
    //other nodes and children
    private CollisionShape2D collider;
    public Sprite2D sprite;
    [Export] private ItemStack item;
    
    //resource data
    public string DisplayName;

    public int count;

    public override void _Ready()
    {
        Initialize(item);
    }

    public void Initialize(ItemStack iData)
    { 
        collider = GetNode<CollisionShape2D>("collider");
        sprite = GetNode<Sprite2D>("Sprite2D");
        
        DisplayName = item.ItemData.DisplayName;
        sprite.Texture = item.ItemData.Texture;
        sprite.Visible = true;
        count = item.Count;
    }
    
    public void TryPickUp()
    {
        Inventory inventory = GetNode<Inventory>("/root/world/UI/inventory");
        
        var stackCopy = item.Clone();
        if (inventory.TryFit(stackCopy)) 
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
