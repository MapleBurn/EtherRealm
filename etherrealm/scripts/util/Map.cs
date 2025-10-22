using Godot;
using Godot.Collections;

namespace EtherRealm.scripts.util;
public partial class Map : TileMapLayer
{
    public override void _Ready()  
    {  
    }  
      
    public void PlaceBlock(Vector2I tilePos, int terrain)  
    {  
        // Check if tile already exists  
        if (GetCellSourceId(tilePos) != -1)  
            return;  
              
        // Place tile and let autotiling handle it  
        SetCell(tilePos, 1, new Vector2I(9, 4), 0);  
          
        // Update surrounding tiles for autotiling  
        UpdateTiles(tilePos, terrain);  
    }  
      
    public void BreakBlock(Vector2I tilePos)  
    {  
        //Check if tile exists  
        if (GetCellSourceId(tilePos) == -1)  
            return;  
          
        //Update surrounding tiles for autotiling  
        UpdateTiles(tilePos, -1);  
    }  
    
    public void UpdateTiles(Vector2I cell, int terrain)
    {
        var tileData = GetCellTileData(cell);
        if (tileData != null)
        {
            var cellsToUpdate = new Godot.Collections.Array<Vector2I>();
            cellsToUpdate.Add(cell);
            SetCellsTerrainConnect(cellsToUpdate, 0, terrain, true);
        }
    }
}