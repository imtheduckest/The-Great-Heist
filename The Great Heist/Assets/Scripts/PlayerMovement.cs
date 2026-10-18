using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //public string playerName = "Player";
    //public bool isCop = false;

    //[Header("Grid Coordinates")]
    //public int gridX = 0;
    //public int gridY = 0;

    //// Move the player token physically in the Unity world
    //public void SnapToGrid(GridManager gridManager)
    //{
    //    float centerOffset = (gridManager.width - 1) * 0.5f;
    //    float xPos = gridManager.gameBoard.x + ((gridX - centerOffset) * gridManager.tileSpacing);
    //    float zPos = gridManager.gameBoard.z + ((gridY - centerOffset) * gridManager.tileSpacing);
    //    transform.position = new Vector3(xPos, gridManager.gameBoard.y + 0.2f, zPos);
    //}

    //// Attempts to step 1 tile towards a target coordinate
    //public bool TryMoveStep(int targetX, int targetY, GridManager gridManager)
    //{
    //    // Check adjacency (Manhattan distance = 1)
    //    int distance = Mathf.Abs(targetX - gridX) + Mathf.Abs(targetY - gridY);
    //    if (distance != 1) return false;

    //    // Check if the grid tile allows movement (not a wall or draw pile)
    //    if (gridManager.CanMoveTo(targetX, targetY))
    //    {
    //        gridX = targetX;
    //        gridY = targetY;
    //        SnapToGrid(gridManager);
    //        return true;
    //    }

    //    return false;
    //}
}
