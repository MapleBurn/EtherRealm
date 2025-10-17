using Godot;
using System;
using EtherRealm.scripts.resource;

namespace EtherRealm.scripts.entity.itemEntities;
public partial class ActionEntity : Node2D
{
    protected ActionEntityData data;
    
    //other nodes and children
    protected Area2D hitbox;  
    protected CollisionPolygon2D attackCollider;  
    protected Sprite2D sprite;
    
    //variables
    protected bool isCooldown = false;  
    public bool isAttacking = false;
    protected float delay;  
    public string actionType; //swing, stab, shoot, etc.
    public Vector2 hitDir;

    public void SetChildNodes()
    {
        // Initialize components  
        hitbox = GetNode<Area2D>("hitbox");  
        attackCollider = GetNode<CollisionPolygon2D>("hitbox/collider");
        sprite = GetNode<Sprite2D>("Sprite2D");
        
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
    
    public virtual void PlayAnimation(int dir, AnimationPlayer animPlayer)
    {
        //will be always overriden for now
    }
}
