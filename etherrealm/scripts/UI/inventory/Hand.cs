using System.Linq;
using EtherRealm.scripts.entity.itemEntities;
using EtherRealm.scripts.resource;
using EtherRealm.scripts.resource.item;
using Godot;

namespace EtherRealm.scripts.UI.inventory;

public partial class Hand : Node2D
{
    public ActionEntity actionEntity;
    public void UpdateActionEntity(InventorySlot slot)
    {
        actionEntity = null;
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
        if (actionEntity.actionType == "swing")
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
        else if (actionEntity.actionType == "place")
        {
            animPlayer.Play(dir == 1 ? "placeRight" : "placeLeft");
        }
    }
    
}