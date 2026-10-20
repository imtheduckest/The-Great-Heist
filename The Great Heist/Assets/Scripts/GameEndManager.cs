using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    private bool gameEnded;

    public bool GameEnded => gameEnded;

    public void CheckWinCondition()
    {
        if (gameEnded)
            return;

        PlayerToken[] players = FindObjectsOfType<PlayerToken>();

        int activeRobbers = 0;

        foreach (PlayerToken player in players)
        {
            if (player == null || !player.gameObject.activeInHierarchy)
                continue;

            if (!player.isCop)
                activeRobbers++;
        }

        if (activeRobbers == 0)
        {
            gameEnded = true;
            Debug.Log("GAME OVER: cop wins");
        }
    }
}
