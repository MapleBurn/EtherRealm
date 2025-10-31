using Godot;
using System;
using EtherRealm.scripts.resource;
using EtherRealm.scripts.resource.item;
using EtherRealm.scripts.UI.inventory;
using EtherRealm.scripts.util;
using ActionEntityData = EtherRealm.scripts.resource.action_entity.ActionEntityData;

namespace EtherRealm.scripts.entity.itemEntities;
public partial class ActionEntity : Node2D
{
    protected ActionEntityData data;
    public InventorySlot itemSlot;
    
    //other nodes and children
    protected Area2D hitbox;  
    protected CollisionPolygon2D attackCollider;  
    protected Sprite2D sprite;
    protected Map tilemap;
    protected Hand hand;
    
    //variables
    protected bool isCooldown = false;  
    protected float delay;  
    public string actionType; //swing, stab, shoot, etc.
    public Vector2 hitDir;
    public bool pendingClear;

    public bool consumeOnUse = false;
    public void SetChildNodes()
    {
        // Initialize components  
        hitbox = GetNode<Area2D>("hitbox");  
        attackCollider = GetNode<CollisionPolygon2D>("hitbox/collider");
        sprite = GetNode<Sprite2D>("Sprite2D");
        tilemap = GetNode<Map>("/root/world/map");
        hand = GetParent<Hand>();
        
        sprite.Offset = data.SpriteOffset;
        sprite.Texture = data.Model;
        attackCollider.Polygon = data.ColliderPoints;

        hand.isEntityInitialized = true;
    }
    
    public virtual void Initialize(ActionEntityData data)
    { }

    public bool CanAttack()
    {
        return !hand.isAnimPlaying && !isCooldown;
    }
    
    public void AttackFinished()  
    {  
        isCooldown = true;
        hand.isAnimPlaying = false;
  
        GetTree().CreateTimer(delay).Timeout += () =>  
        {  
            isCooldown = false;   
        };  
    }

    public virtual void UsePrimary()
    {
        
    }

    public virtual void UseSecondary(int dir)
    {
        
    }
    
    protected void ConsumeItem()
    {
        if (consumeOnUse)
        {
            if (itemSlot.Item.Count == 1)
            {
                hand.QueueUpdate(itemSlot);
                return;
            }
            
            itemSlot.RemoveFromSlot(1);
        }
    }
}
