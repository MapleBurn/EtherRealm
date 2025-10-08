using Godot;
using System;

public partial class ItemDrop : CharacterBody2D
{
    //other nodes and children
    private CollisionShape2D collider;
    public Sprite2D sprite;
    private ItemStack item;
    [Export] private ItemData itemData; //export for testing
    
    //resource data
    public string DisplayName;

    public int count;

    public override void _Ready()
    {
        Initialize(itemData);
        CollisionMask = 2;  //make it collide with ground (2)
    }

    public void Initialize(ItemData iData)
    {
        item.ItemData = itemData;   //temporarily use ItemData for testing, later switch to ItemStack
        item.Count = 1;
        collider = GetNode<CollisionShape2D>("collider");
        sprite = GetNode<Sprite2D>("Sprite2D");
        
        DisplayName = item.ItemData.DisplayName;
        sprite.Texture = item.ItemData.Texture;
        count = item.Count;
    }
    
    public void TryPickUp()
    {
        Inventory inventory = GetNode<Inventory>("/root/world/UI/inventory");
        if (inventory.TryFit(item)) 
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
