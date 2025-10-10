using Godot;
using System;

public partial class ItemDrop : CharacterBody2D
{
    //other nodes and children
    private CollisionShape2D collider;
    public Sprite2D sprite;
    private ItemStack item;
    [Export] ItemData itemData;
    
    //resource data
    public string DisplayName;

    public int count;

    public override void _Ready()
    {
        item = new ItemStack(itemData);
        Initialize();
    }

    public void Initialize()
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
        
        var stackCopy = new ItemStack(item.ItemData, item.Count);
        if (inventory.TryFit(stackCopy) == 0) 
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
