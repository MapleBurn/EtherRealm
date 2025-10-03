using Godot;
using System;

public partial class Item : CharacterBody2D
{
    //other nodes and children
    private CollisionShape2D collider;
    public Sprite2D sprite;
    
    private int count = 1;

    public override void _Ready()
    {
        collider = GetNode<CollisionShape2D>("collider");
        sprite = GetNode<Sprite2D>("Sprite2D");
        CollisionMask = 2;  //make it collide with ground (2)
    }
    
    
}
