using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureDetector : MonoBehaviour
{
    public void CheckForCapture(PlayerMovement movedPlayer)
    {
        if (movedPlayer == null)
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

        // Find robbers on the same tile.
        foreach (PlayerToken token in tokens)
        {
            if (token == null || !token.gameObject.activeInHierarchy)
                continue;

            if (token.isCop)
                continue;

            PlayerMovement movement = token.GetComponent<PlayerMovement>();

            if (movement == null)
                continue;

            if (movement.gridX == movedPlayer.gridX &&
                movement.gridY == movedPlayer.gridY)
            {
                Debug.Log(
                    "Capture detected! Cop and " +
                    token.playerName + " share tile (" +
                    movement.gridX + ", " + movement.gridY + ")."
                );
            }
        }
    }
}
