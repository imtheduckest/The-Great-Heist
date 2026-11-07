using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitManager : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public GameEndManager gameEndManager;
    public GameObject highlightPrefab;

    [Header("Exit Settings")]
    public float highlightHeightOffset = 0.05f;

    private readonly Vector2Int[] exitPositions =
    {
        new Vector2Int(0, 3),
        new Vector2Int(3, 0),
        new Vector2Int(6, 3),
        new Vector2Int(3, 6)
    };

    private readonly List<GameObject> activeHighlights =
        new List<GameObject>();

    private int completedRounds;
    private bool exitsActive;

    public bool ExitsActive => exitsActive;

    private void Start()
    {
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();

        if (gameEndManager == null)
            gameEndManager = FindObjectOfType<GameEndManager>();
    }

    // Called whenever the Cop finishes a turn.
    public void OnCopTurnEnded()
    {
        if (gameEndManager != null && gameEndManager.GameEnded)
            return;

        // If exits were active, this round has finished.
        if (exitsActive)
        {
            DeactivateExits();
            return;
        }

        completedRounds++;

        Debug.Log(
            "[ExitManager] Completed rounds: " + completedRounds
        );

        if (completedRounds >= 3)
        {
            completedRounds = 0;
            ActivateExits();
        }
    }

    private void ActivateExits()
    {
        if (gridManager == null || highlightPrefab == null)
        {
            Debug.LogError(
                "[ExitManager] GridManager or Highlight Prefab is missing!"
            );
            return;
        }

        exitsActive = true;

        foreach (Vector2Int position in exitPositions)
        {
            Vector3 worldPosition =
                gridManager.GetTileWorldPosition(
                    position.x,
                    position.y
                );

            worldPosition.y += highlightHeightOffset;

            GameObject highlight = Instantiate(
                highlightPrefab,
                worldPosition,
                Quaternion.identity
            );

            activeHighlights.Add(highlight);
        }

        Debug.Log("[ExitManager] Four exits are now ACTIVE!");
    }

    private void DeactivateExits()
    {
        foreach (GameObject highlight in activeHighlights)
        {
            if (highlight != null)
                Destroy(highlight);
        }

        activeHighlights.Clear();

        exitsActive = false;

        Debug.Log("[ExitManager] Exits are now INACTIVE.");
    }

    public bool IsExitTile(int x, int y)
    {
        if (!exitsActive)
            return false;

        foreach (Vector2Int position in exitPositions)
        {
            if (position.x == x && position.y == y)
                return true;
        }

        return false;
    }

    public void RegisterExitedPlayer(PlayerToken player)
    {
        if (gameEndManager == null)
        {
            Debug.LogError("[ExitManager] GameEndManager is missing!");
            return;
        }

        gameEndManager.RegisterExitedPlayer(player);
    }
}
