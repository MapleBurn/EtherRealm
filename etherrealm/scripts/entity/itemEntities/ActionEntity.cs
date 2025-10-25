using Godot;
using System;
using EtherRealm.scripts.resource;
using EtherRealm.scripts.resource.item;
using EtherRealm.scripts.UI.inventory;
using EtherRealm.scripts.util;

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
    
    //variables
    protected bool isCooldown = false;  
    public bool isAttacking = false;
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
        
        sprite.Offset = data.SpriteOffset;
        sprite.Texture = data.Model;
        attackCollider.Polygon = data.ColliderPoints;
        
    }
    
    public virtual void Initialize(ActionEntityData data)
    { }

    public bool CanAttack()
    {
        return !isAttacking && !isCooldown;
    }
    
    public void AttackFinished()  
    {  
        isAttacking = false;  
        isCooldown = true;  
  
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
                pendingClear = true;
                return;
            }
            
            itemSlot.RemoveFromSlot(1);
        }
    }

    public void TryClear()
    {
        if (pendingClear)
        {
            itemSlot.RemoveFromSlot(1);
            pendingClear = false;
        }
    }
    
    /*public virtual void PlayAnimation(int dir, AnimationPlayer animPlayer)
    {
        //will be always overriden for now
    }*/
}
