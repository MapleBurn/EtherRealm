using Godot;
using System;
using EtherRealm.scripts.resource.item;
using EtherRealm.scripts.UI.inventory;

namespace EtherRealm.scripts.entity.itemEntities;
public partial class ItemDrop : CharacterBody2D
{
    //other nodes and children
    private CollisionShape2D collider;
    private Sprite2D sprite;
    private ItemStack item;
    [Export] ItemData itemData;
    
    //resource data
    private string DisplayName;

    private int count;

    public override void _Ready()
    {
        item = new ItemStack(itemData);
        Initialize();
    }

    private void Initialize()
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
        Inventory inventory = GetNode<Inventory>("/root/world/player/UI/inventory");
        
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
