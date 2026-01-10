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
    protected ActionEntityData Data;
    public InventorySlot ItemSlot;
    
    //other nodes and children
    private Area2D hitbox;  
    protected CollisionPolygon2D AttackCollider;  
    private Sprite2D sprite;
    protected Map Tilemap;
    protected HeldItemHandler ItemHandler;
    
    //variables
    protected bool IsCooldown = false;  
    protected float Delay;  
    public string ActionType; //swing, stab, shoot, etc.
    public Vector2 HitDir;

    protected bool ConsumeOnUse = false;
    public void SetChildNodes()
    {
        // Initialize components  
        hitbox = GetNode<Area2D>("hitbox");  
        AttackCollider = GetNode<CollisionPolygon2D>("hitbox/collider");
        sprite = GetNode<Sprite2D>("Sprite2D");
        Tilemap = GetNode<Map>("/root/world/map");
        ItemHandler = GetParent<HeldItemHandler>();
        
        sprite.Offset = Data.SpriteOffset;
        sprite.Texture = Data.Model;
        AttackCollider.Polygon = Data.ColliderPoints;

        ItemHandler.IsEntityInitialized = true;
    }
    
    public virtual void Initialize(ActionEntityData data)
    { }

    public bool CanAttack()
    {
        return !ItemHandler.IsAnimPlaying && !IsCooldown;
    }
    
    public virtual void AttackFinished()  
    {  
        IsCooldown = true;
        ItemHandler.IsAnimPlaying = false;
  
        GetTree().CreateTimer(Delay).Timeout += () =>  
        {  
            IsCooldown = false;   
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
        if (ConsumeOnUse)
        {
            if (ItemSlot.Item.Count == 1)
            {
                ItemHandler.QueueUpdate(ItemSlot);
                return;
            }
            
            ItemSlot.RemoveFromSlot(1);
        }
    }
}
