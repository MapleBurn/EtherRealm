using EtherRealm.scripts.entity.itemEntities;
using EtherRealm.scripts.UI.inventory;
using Godot;

namespace EtherRealm.scripts.entity;

public partial class Hand : Node2D
{
    public ActionEntity actionEntity;
    public bool isAnimPlaying = false;
    public bool isEntityInitialized = false;
    
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
            if (!isEntityInitialized || actionEntity.CanAttack())
            {
                UpdateActionEntity(invSlot);
                isPendingUpdate = false;
            }
        }
    }
    
    private void UpdateActionEntity(InventorySlot slot)
    {
        actionEntity = null;
        isEntityInitialized = false;
        foreach (var child in GetChildren())
        {
            child.QueueFree(); //flush the children into the toilet
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
            actionEntity.itemSlot = slot;
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
        var action = actionEntity.actionType;
        if (action == "swing")
        {
            if (dir == 1)
            {
                animPlayer.Play("swingRight");
                actionEntity.hitDir = Vector2.Right;
            }
            else
            {
                animPlayer.Play("swingLeft");
                actionEntity.hitDir = Vector2.Left;
            }
        }
        else if (action == "place")
        {
            animPlayer.Play(dir == 1 ? "placeRight" : "placeLeft");
        }
    }

    public void AnimationFinished()
    {
        isAnimPlaying = false;  
        if (isEntityInitialized)
            actionEntity.AttackFinished();
    }
    
}