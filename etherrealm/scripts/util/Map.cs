using EtherRealm.scripts.entity.itemEntities;
using EtherRealm.scripts.resource.item;
using Godot;
using Godot.Collections;

namespace EtherRealm.scripts.util;
public partial class Map : TileMapLayer
{
    private float maxPlaceDistance = 100f;
    private float maxBreakDistance = 100f;
    
    public override void _Ready()  
    {  
    }  
      
    public bool CanPlaceBlock(Vector2I tilePos)  
    {  
        // Check if tile already exists  
        if (GetCellSourceId(tilePos) != -1 || !IsWithinPlayerReach(tilePos, "place")) 
            return false;
        // Check for entity overlap
        if (IsEntityOverlapping(tilePos))
            return false;
        return true;
    }
    
    public bool PlaceBlock(Vector2I tilePos, int terrain)  
    {  
        // Place tile and let autotiling handle it  
        SetCell(tilePos, 1, new Vector2I(9, 4), 0);  
          
        // Update surrounding tiles for autotiling  
        UpdateTiles(tilePos, terrain);
        return true;
    }
      
    public bool TryBreakBlock(Vector2I tilePos)  
    {  
        var tileId = GetCellSourceId(tilePos);
        
        //Check if tile exists  
        if (tileId == -1 || !IsWithinPlayerReach(tilePos, "break"))  
            return false;
            
        //Update surrounding tiles for autotiling  
        UpdateTiles(tilePos, -1);
        
        //spawn item drop
        if (tileId == 1)
        {
            var iData = GD.Load<ItemData>("res://resources/items/blocks/TemplateBlock.tres");
            var pos = ToGlobal(MapToLocal(tilePos));
            CallDeferred("SpawnItemDrop", pos, iData);
        }
        return true;
    }  
    
    private void UpdateTiles(Vector2I cell, int terrain)
    {
        var tileData = GetCellTileData(cell);
        if (tileData != null)
        {
            var cellsToUpdate = new Godot.Collections.Array<Vector2I>();
            cellsToUpdate.Add(cell);
            SetCellsTerrainConnect(cellsToUpdate, 0, terrain, true);
        }
    }
    
    public bool IsEntityOverlapping(Vector2I tilePos)  //AI crafted method to check for CharacterBody2D overlap
    {  
        // Convert tile position to world position  
        Vector2 worldPos = MapToLocal(tilePos);  
          
        // Get the tile size (assuming square tiles, adjust if needed)  
        var tileSize = TileSet.TileSize;  
          
        // Create a rectangle representing the tile area  
        Rect2 tileRect = new Rect2(worldPos - tileSize / 2, tileSize);  
          
        // Get all CharacterBody2D nodes in the scene  
        var spaceState = GetWorld2D().DirectSpaceState;
        var query =  new PhysicsShapeQueryParameters2D();  
          
        // Create a rectangle shape for the query  
        var rectShape = new RectangleShape2D();  
        rectShape.Size = tileSize;  
        query.Shape = rectShape;  
        query.Transform = new Transform2D(0, worldPos);  
        query.CollideWithBodies = true;  
        query.CollideWithAreas = false;  
          
        // Query for overlapping bodies  
        var results = spaceState.IntersectShape(query);  
          
        // Check if any result is a CharacterBody2D  
        foreach (var result in results)  
        {  
            if (result["collider"].Obj is CharacterBody2D)  
            {  
                return true;  
            }  
        }  
          
        return false;  
    }
    
    private void SpawnItemDrop(Vector2 position, ItemData itemData)
    {
        var itemDropScene = GD.Load<PackedScene>("res://scenes/items/itemDrop.tscn");
        var itemDrop = itemDropScene.Instantiate<ItemDrop>();
        itemDrop.GlobalPosition = position;
        itemDrop.itemData = itemData;
        GetTree().CurrentScene.AddChild(itemDrop);
    }
    
    private bool IsWithinPlayerReach(Vector2I tilePos, string action)  
    {  
        // Get player node (adjust the path/group name as needed)  
        var player = GetTree().GetFirstNodeInGroup("player") as Node2D;  
          
        if (player == null)  
            return false;  
          
        // Convert tile position to world position  
        Vector2 worldPos = ToGlobal(MapToLocal(tilePos));  
          
        // Check distance  
        float distance = player.GlobalPosition.DistanceTo(worldPos);  
        
        if (action == "break")
            return distance <= maxBreakDistance;
        return distance <= maxPlaceDistance;
    }
}