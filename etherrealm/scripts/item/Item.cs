using Godot;
using System;

public partial class Item : CharacterBody2D
{
    //other nodes and children
    private CollisionShape2D collider;
    public Sprite2D sprite;
    [Export] public string name;
    [Export] public int itemId = 1;
    public int count = 1;
    //private bool isPickedUp;

    public override void _Ready()
    {
        Initialize();
        CollisionMask = 2;  //make it collide with ground (2)
    }

    public void Initialize()
    {
        collider = GetNode<CollisionShape2D>("collider");
        sprite = GetNode<Sprite2D>("Sprite2D");
    }

    public void PickUp()
    {
        Inventory inventory = GetNode<Inventory>("../UI/inventory");
        if (inventory.TryFit(this))
            QueueFree();
    }
}
