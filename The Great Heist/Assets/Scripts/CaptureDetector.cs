using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureDetector : MonoBehaviour
{
    [Header("Refernces")]
    public TurnManager turnManager;
    public GameEndManager gameEndManager;

    private bool capturePending;
    private readonly List<PlayerToken> candidates = new List<PlayerToken>();

    public void CheckForCapture(PlayerMovement movedPlayer)
    {
        if (movedPlayer == null || capturePending)
            return;

        PlayerToken[] tokens = FindObjectsOfType<PlayerToken>();

        PlayerToken cop = null;

        // Find the Cop on the moved player's tile.
        foreach (PlayerToken token in tokens)
        {
            if (token == null || !token.gameObject.activeInHierarchy)
                continue;

            if (!token.isCop)
                continue;

            PlayerMovement movement = token.GetComponent<PlayerMovement>();

            if (movement == null)
                continue;

            if (movement.gridX == movedPlayer.gridX &&
                movement.gridY == movedPlayer.gridY)
            {
                cop = token;
                break;
            }
        }

        if (cop == null)
            return;

        candidates.Clear();

        // Find robbers on the same tile.
        foreach (PlayerToken token in tokens)
        {
            if (token == null || !token.gameObject.activeInHierarchy)
                continue;

            if (token.isCop)
                continue;

            PlayerMovement movement =
                token.GetComponent<PlayerMovement>();

            if (movement == null)
                continue;

            if (movement.gridX == movedPlayer.gridX &&
                movement.gridY == movedPlayer.gridY)
            {
                candidates.Add(token);
            }
        }

        if (candidates.Count == 0)
            return;

        capturePending = true;

        if (candidates.Count == 1)
        {
            CatchRobber(candidates[0]);
        }
        else
        {
            Debug.Log("Choose one robber to catch.");
        }
    }

    private void OnGUI()
    {
        if (!capturePending || candidates.Count <= 1)
            return;

        GUI.Box(
            new Rect(Screen.width / 2f - 150, 20, 300, 45),
            "Choose one robber to catch"
        );

        float buttonY = 75f;

        foreach (PlayerToken robber in candidates)
        {
            if (robber == null ||
                !robber.gameObject.activeInHierarchy)
                continue;

            string buttonLabel = "Catch " + robber.playerName;

            if (GUI.Button(
                new Rect(Screen.width / 2f - 100, buttonY, 200, 35),
                buttonLabel))
            {
                CatchRobber(robber);
                return;
            }

            buttonY += 40f;
        }
    }

    private void CatchRobber(PlayerToken robber)
    {
        if (!capturePending || robber == null)
            return;

        Debug.Log(robber.playerName + " has been caught!");

        PlayerInventory inventory = robber.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.ClearForCapture();
        }

        else
        {
            Debug.LogWarning(robber.playerName + " has no PlayerInventory!");
        }

        robber.gameObject.SetActive(false);

        if (gameEndManager != null)
        {
            gameEndManager.CheckWinCondition();
        }

        if (turnManager != null)
        {
            turnManager.EndTurn();
        }

        candidates.Clear();
        capturePending = false;
    }
}
