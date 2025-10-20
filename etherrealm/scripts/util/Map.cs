using Godot;
using Godot.Collections;

namespace EtherRealm.scripts.util;
public partial class Map : TileMapLayer
{
    public override void _Ready()  
    {  
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
      
    public void BreakBlock(Vector2I tilePos)  
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
        Array<Vector2I> cells = new Array<Vector2I>();

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
                //var sourceId = GetCellSourceId(neighborPos);
                //var atlasCoords = GetCellAtlasCoords(neighborPos);
                //var alternative = GetCellAlternativeTile(neighborPos);
                cells.Add(neighborPos);
            }
        }

        var terrainSet = 0;
        var terrain = 0;

        SetCellsTerrainConnect(cells, terrainSet, terrain);
        QueueRedraw();
    }
    
    public void ClearCellFromPosition(Vector2I cell)
    {
        Vector2I[] ADJ_VECS = {
            new(-1, -1), new(0, -1), new(1, -1),
            new(-1, 0),              new(1, 0),
            new(-1, 1),  new(0, 1),  new(1, 1)
        };
        
        // In C#, Vector2I is a struct (not nullable). Check tile presence via tile data.
        var tileData = GetCellTileData(cell);
        if (tileData != null)
        {
            GD.Print("Cell found ", cell);

            GD.Print(tileData);
            // Clear the cell on layer 0 by setting source id to -1.
            SetCell(cell, -1);
            GD.Print(GetCellTileData(cell));
            GD.Print(GetSurroundingCells(cell));

            var cellsToUpdate = new Godot.Collections.Array<Vector2I>();
            foreach (var nb in ADJ_VECS)
            {
                var nbData = GetCellTileData(cell + nb);
                if (nbData != null)
                    cellsToUpdate.Add(cell + nb);
            }

            GD.Print(cellsToUpdate);

            // Reconnect terrain for the single cleared cell (terrain_set = 0, terrain = -1).
            SetCellsTerrainConnect(cellsToUpdate, 0, -1, true);
        }
        else
        {
            GD.Print("No Cell Found at ", cell);
        }
    }
}