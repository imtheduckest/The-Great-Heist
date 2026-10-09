using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 7;
    public int height = 7;
    public float tileSize = 1.45f;
    public float tileSpacing = 1.42f;
    public Vector3 gameBoard = Vector3.zero;

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject cellPrefab;
    public GameObject drawPilePrefab;

    [Header("Debug")]
    public bool showGizmos = true;

    // Dictionary to easily look up cells by their X, Y coordinates
    public Dictionary<Vector2Int, TileData> gridCells = new Dictionary<Vector2Int, TileData>();

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        float centerOffset = (width - 1) * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Determine tile type based on your layout rules
                int type = 0; // Default is normal floor

                //BANK WALLS (Type 1)
                if ((x == 1 && y == 4) || (x == 5 && y == 4) ||
                    (x == 1 && y == 2) || (x == 5 && y == 2) ||
                    (x == 2 && y == 5) || (x == 4 && y == 5) ||
                    (x == 2 && y == 1) || (x == 4 && y == 1) ||
                    (x == 3 && y == 3))
                {
                    type = 1;
                }

                //DEFINE CELLS (Type 2)
                if ((x == 2 && y == 2) || (x == 4 && y == 2) ||
                    (x == 4 && y == 4) || (x == 2 && y == 4))
                {
                    type = 2;
                }

                //DRAW PILES (Type 3)
                if ((x == 1 && y == 5) || (x == 1 && y == 1) ||
                    (x == 5 && y == 1) || (x == 5 && y == 5))
                {
                    type = 3;
                }

                // Pick the right prefab to instantiate
                GameObject prefabToSpawn = floorPrefab;
                if (type == 1) prefabToSpawn = wallPrefab;
                else if (type == 2) prefabToSpawn = cellPrefab;
                else if (type == 3) prefabToSpawn = drawPilePrefab;

                // Create the data object first (Logic is independent of visuals)
                TileData newTile = new TileData(x, y, type);

                // Only instantiate visuals IF a prefab has actually been assigned in the Inspector
                if (prefabToSpawn != null)
                {
                    float xPos = gameBoard.x + ((x - centerOffset) * tileSpacing);
                    float zPos = gameBoard.z + ((y - centerOffset) * tileSpacing);
                    Vector3 spawnPosition = new Vector3(xPos, gameBoard.y, zPos);

                    GameObject tileGO = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, transform);
                    tileGO.transform.localScale = new Vector3(tileSize, tileGO.transform.localScale.y, tileSize);
                    tileGO.name = $"Tile_{x}_{y}_Type_{type}";
                    newTile.tile = tileGO;
                }

                // Store in dictionary so game rules always work
                gridCells.Add(new Vector2Int(x, y), newTile);
            }
        }
    }

    public bool CanMoveTo(int x, int y)
    {
        Vector2Int pos = new Vector2Int(x, y);

        // Check if the coordinate exists within our grid bounds
        if (gridCells.ContainsKey(pos))
        {
            int tileType = gridCells[pos].tileType;
            // If tileType is 1 (Wall), player CANNOT move there
            if (tileType == 1)
            {
                return false;
            }
            return true;
        }
        return false; // Out of bounds
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        float centerOffset = (width - 1) * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xPos = gameBoard.x + ((x - centerOffset) * tileSpacing);
                float zPos = gameBoard.z + ((y - centerOffset) * tileSpacing);
                Vector3 cellCenter = new Vector3(xPos, gameBoard.y + 0.05f, zPos);

                // Color code based on cell type logic
                Vector2Int pos = new Vector2Int(x, y);

                // Default color for floor
                Gizmos.color = Color.white;

                // Check types for visualization
                if ((x == 1 && y == 4) || (x == 5 && y == 4) ||
                    (x == 1 && y == 2) || (x == 5 && y == 2) ||
                    (x == 2 && y == 5) || (x == 4 && y == 5) ||
                    (x == 2 && y == 1) || (x == 4 && y == 1) ||
                    (x == 3 && y == 3))
                {
                    Gizmos.color = Color.black; // Walls
                }
                else if ((x == 2 && y == 2) || (x == 4 && y == 2) ||
                         (x == 4 && y == 4) || (x == 2 && y == 4))
                {
                    Gizmos.color = Color.green; // Cells
                }

                if ((x == 1 && y == 5) || (x == 1 && y == 1) ||
                    (x == 5 && y == 1) || (x == 5 && y == 5))
                {
                    Gizmos.color = Color.blue; // Draw piles
                }

                // Draw wire cube for each square on the board
                Gizmos.DrawWireCube(cellCenter, new Vector3(tileSize * 0.95f, 0.1f, tileSize * 0.95f));
            }
        }
    }
}
