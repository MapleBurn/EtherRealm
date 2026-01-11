using EtherRealm.scripts.entity.itemEntities;
using EtherRealm.scripts.UI.inventory;
using Godot;

namespace EtherRealm.scripts.entity;

public partial class HeldItemHandler : Node2D
{
    public ActionEntity actionEntity;
    public bool IsAnimPlaying = false;
    public bool IsEntityInitialized = false;
    
    private bool isPendingUpdate = false;
    private InventorySlot invSlot;

    public void QueueUpdate(InventorySlot slot)
    {
        isPendingUpdate = true;
        invSlot = slot;
    }

    public override void _Process(double delta)
    {
        if (isPendingUpdate)
        {
            if (!IsEntityInitialized || actionEntity.CanAttack())
            {
                UpdateActionEntity(invSlot);
                isPendingUpdate = false;
            }
        }
    }
    
    private void UpdateActionEntity(InventorySlot slot)
    {
        actionEntity = null;
        IsEntityInitialized = false;
        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }

        if (slot.Item == null || slot.Item.ItemData == null || slot.Item.ItemData.EntityData == null)
            return;

        var entityData = slot.Item.ItemData.EntityData;
        var entityScene = GD.Load<PackedScene>(entityData.EntityScenePath);
        var node = entityScene.Instantiate();
        if (node is ActionEntity)
        {
            actionEntity = node as ActionEntity;
            actionEntity.Initialize(slot.Item.ItemData.EntityData);
            actionEntity.ItemSlot = slot;
            CallDeferred("SpawnEntity", slot);
        }
    }

    private void SpawnEntity(InventorySlot slot)
    {
        AddChild(actionEntity);
        actionEntity.SetChildNodes();
    }
    
    public void PlayAnimation(int dir, AnimationPlayer animPlayer)
    {
        var action = actionEntity.ActionType;
        /*if (action == "swing")
        {
            if (dir == 1)
            {
                animPlayer.Play("swingRight");
                actionEntity.HitDir = Vector2.Right;
            }
            else
            {
                animPlayer.Play("swingLeft");
                actionEntity.HitDir = Vector2.Left;
            }
        }*/
        if (action == "attack")
        {
            animPlayer.Play("stab");
            actionEntity.HitDir = dir == 1 ? Vector2.Right : Vector2.Left;
        }
        else if (action == "mine")
        {
            animPlayer.Play("mine");
            //actionEntity.HitDir = dir == 1 ? Vector2.Right : Vector2.Left;
        }
        else if (action == "place")
        {
            animPlayer.Play("place");
        }
    }

    public void AnimationFinished()
    {
        IsAnimPlaying = false;  
        if (IsEntityInitialized)
            actionEntity.AttackFinished();
    }
    
}