using Godot;

namespace EtherRealm.scripts.util;
public partial class Map : TileMapLayer
{
    public override void _Ready()  
    {  
    }  
      
    public override void _Input(InputEvent @event)  
    {  
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)  
        {  
            Vector2 globalMousePos = GetGlobalMousePosition();  
            Vector2I tilePos = LocalToMap(ToLocal(globalMousePos));  
              
            if (mouseEvent.ButtonIndex == MouseButton.Left)  
                PlaceBlock(tilePos);  
            else if (mouseEvent.ButtonIndex == MouseButton.Right)  
                BreakBlock(tilePos);  
        }  
    }  
      
    private void PlaceBlock(Vector2I tilePos)  
    {  
        // Check if tile already exists  
        if (GetCellSourceId(tilePos) != -1)  
            return;  
              
        // Place tile and let autotiling handle it  
        SetCell(tilePos, 0, Vector2I.Zero, 0);  
          
        // Update surrounding tiles for autotiling  
        UpdateSurroundingTiles(tilePos);  
    }  
      
    private void BreakBlock(Vector2I tilePos)  
    {  
        // Check if tile exists  
        if (GetCellSourceId(tilePos) == -1)  
            return;  
              
        EraseCell(tilePos);  
          
        // Update surrounding tiles for autotiling  
        UpdateSurroundingTiles(tilePos);  
    }  
      
    private void UpdateSurroundingTiles(Vector2I centerPos)  
    {  
        // Update the 8 surrounding tiles to refresh autotiling  
        Vector2I[] neighbors = {  
            new(-1, -1), new(0, -1), new(1, -1),  
            new(-1, 0),              new(1, 0),  
            new(-1, 1),  new(0, 1),  new(1, 1)  
        };  
          
        foreach (var offset in neighbors)  
        {  
            Vector2I neighborPos = centerPos + offset;  
            if (GetCellSourceId(neighborPos) != -1)  
            {  
                // Force tile update by resetting it  
                var sourceId = GetCellSourceId(neighborPos);  
                var atlasCoords = GetCellAtlasCoords(neighborPos);  
                var alternative = GetCellAlternativeTile(neighborPos);  
                SetCell(neighborPos, sourceId, atlasCoords, alternative);  
            }  
        }  
    }
}