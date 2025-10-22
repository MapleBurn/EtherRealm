using System.Linq;
using EtherRealm.scripts.entity.itemEntities;
using EtherRealm.scripts.resource;
using EtherRealm.scripts.resource.item;
using Godot;

namespace EtherRealm.scripts.UI.inventory;

public partial class Hand : Node2D
{
    public ActionEntity actionEntity;
    [Export] private Inventory inventory;
    public void UpdateActionEntity(Slot slot)
    {
        actionEntity = null;
        foreach (var child in GetChildren())
        {
            child.QueueFree(); //flush the children into the toilet
        }

        if (slot.Item.ItemData == null || slot.Item.ItemData.EntityData == null)
            return;

        var entityData = slot.Item.ItemData.EntityData;
        var entityScene = GD.Load<PackedScene>(entityData.EntityScenePath);
        var node = entityScene.Instantiate();
        if (node is ActionEntity)
            CallDeferred("SpawnEntity", node, slot);
    }

    private void SpawnEntity(Node2D node, Slot slot)
    {
        actionEntity = node as ActionEntity;
        actionEntity.Initialize(slot.Item.ItemData.EntityData);
        AddChild(actionEntity);
        actionEntity.SetChildNodes();
        actionEntity.itemSlot = inventory.inventorySlots.Where(a => a.index == slot.index).FirstOrDefault();
    }
}