using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToken : MonoBehaviour
{
    public string playerName = "Player";
    public bool isCop = false;
    public bool hasExited = false;

    [Header("Current Grid Position")]
    public int gridX = 0;
    public int gridY = 0;

    void Awake()
    {
        // Automatically check if the object is tagged as "Cop"
        if (gameObject.CompareTag("Cop"))
        {
            isCop = true;
            playerName = "Cop";
        }
        else
        {
            isCop = false;
        }
    }

    public void SetupDefaultName(int index)
    {
        if (!isCop)
        {
            playerName = $"Player {index}";
        }
    }

    // Call this at the start of the game to sync position with the grid
    public void InitializePosition(GridManager gridManager)
    {
        float centerOffset = (gridManager.width - 1) * 0.5f;

        // Optional: If you placed the token manually in the Scene view, 
        // this calculates its starting grid coordinates based on its world position!
        gridX = Mathf.RoundToInt((transform.position.x - gridManager.gameBoard.x) / gridManager.tileSpacing + centerOffset);
        gridY = Mathf.RoundToInt((transform.position.z - gridManager.gameBoard.z) / gridManager.tileSpacing + centerOffset);

        // Snap it precisely to the center of that tile
        UpdateWorldPosition(gridManager.tileSpacing, centerOffset, gridManager.gameBoard);
    }

    // Method to snap the token visually to its grid coordinates
    public void UpdateWorldPosition(float tileSpacing, float centerOffset, Vector3 gameBoard)
    {
        float xPos = gameBoard.x + ((gridX - centerOffset) * tileSpacing);
        float zPos = gameBoard.z + ((gridY - centerOffset) * tileSpacing);
        transform.position = new Vector3(xPos, gameBoard.y + 0.2f, zPos); // Slightly above tile
    }
}
