using Godot;
using System;
using EtherRealm.scripts.entity;
using EtherRealm.scripts.entity.itemEntities;

namespace  EtherRealm.scripts.entity.animals;

public partial class Animal : Entity
{
    protected int dir = 1; //one equals right
    protected int decide = 0;
    protected GpuParticles2D hurtParticles;
    
    protected virtual void Walk(double delta)
    {
        var velocity = Velocity;
        
        //handle gravity
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }
        
        //put pathfinding logic here
        
        Vector2 direction = Vector2.Zero;
        if (decide < 80)
        {
            if (decide < 40)
            {
                direction = Vector2.Left;
                dir = -1;
            }
            else
            {
                direction = Vector2.Right;
                dir = 1;
            }
            
            if (direction.X > 0)
            {
                Scale = new Vector2(1, -1);
                RotationDegrees = 180f;
                dir = -1;
            } 
            else
            {
                Scale = new Vector2(1, 1);
                RotationDegrees = 0f;
                dir = 1;
            }

            var targetX = direction.X * maxSpeed;
            
            velocity.X = Mathf.MoveToward(Velocity.X, targetX, acceleration * (float)delta);
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, friction * (float)delta);  
        }
        
        HandleAnimation(direction);
        
        Velocity = velocity;
        MoveAndSlide();
    }

    protected virtual void HandleAnimation(Vector2 direction)
    {
        /*if (!IsOnFloor())
        {
            UpdateAnimation("fall");
        }
        else
        {*/
            if (direction != Vector2.Zero)
                UpdateAnimation("walk");
            else
                UpdateAnimation("idle");
        //}
    }
    
    protected void UpdateAnimation(string animName)
    {
        const float blendDuration = 0.5f;
        const float walkBlendDuration = 0.08f;
		
        if (animPlayer.CurrentAnimation != animName)
        {
            if (animName == "walk")
                animPlayer.Play(animName, walkBlendDuration);
            animPlayer.Play(animName, blendDuration);
        }
    }

    public void DecideTimerTick()
    {
        decide = rdm.Next(0, 100);
    }

    protected override void HurtboxAreaEntered(Area2D area)
    {
        if (!area.IsInGroup("weapons"))
            return;
        
        Weapon weapon = area.GetParent<Weapon>();
        float wepDamage = weapon.damage;
        float wepCritDmgMult = weapon.critDmgMult;
        float wepCritChance = weapon.critChance;
        bool isCrit = false;
        Vector2 wepHitDir = weapon.HitDir;
        float wepKnockback = weapon.knockback;
        float wepCritKBMult = weapon.critKbMult;
        
        //attack type damage scaling
        if (weapon.ActionType == "stab")
        {
            wepDamage *= 1.0f;
            wepKnockback *= 0.8f;
        }
        else if (weapon.ActionType == "swing")
        {
            wepDamage *= 1.5f;
            wepKnockback *= 1.2f;
        }

        if (rdm.Next(0, 100) < wepCritChance) //chance for a critical hit
        {
            wepDamage *= wepCritDmgMult; 
            isCrit = true;
            wepKnockback *= wepCritKBMult;
        }
        ProcessDamage(wepDamage, isCrit);
        ProcessKnockback(wepKnockback, wepHitDir);
        SpawnHurtParticles(wepHitDir);
    }
    
    private void SpawnHurtParticles(Vector2 hitDir)
    {
        ParticleProcessMaterial material = (ParticleProcessMaterial)hurtParticles.ProcessMaterial;
        var dir = hitDir.Normalized();
        material.Direction = new Vector3(dir.X, dir.Y, 0);
        hurtParticles.Emitting = true;
    }
}
