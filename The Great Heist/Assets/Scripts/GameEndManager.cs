using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    public GameObject endScreen;
    public List<PlayerToken> exitedPlayers = new List<PlayerToken>();
    private bool gameEnded;

    public bool GameEnded => gameEnded;

    void Start()
    {
        if (endScreen != null)
            endScreen.SetActive(false);
    }

    public void RegisterExitedPlayer(PlayerToken player)
    {
        if (player == null || player.isCop || player.hasExited)
            return;

        player.hasExited = true;
        exitedPlayers.Add(player);

        Debug.Log(player.playerName + " has escaped!");

        CheckWinCondition();
    }

    public void CheckWinCondition()
    {
        if (gameEnded)
            return;

        PlayerToken[] players = FindObjectsOfType<PlayerToken>();

        int activeRobbers = 0;

        foreach (PlayerToken player in players)
        {
            if (player == null ||
                !player.gameObject.activeInHierarchy ||
                player.isCop ||
                player.hasExited)
                continue;

            activeRobbers++;
        }

        if (activeRobbers > 0)
            return;

        gameEnded = true;

        // At least one robber escaped.
        if (exitedPlayers.Count > 0)
        {
            PlayerToken winner = null;
            int highestMoney = -1;

            foreach (PlayerToken player in exitedPlayers)
            {
                if (player == null)
                    continue;

                PlayerInventory inventory =
                    player.GetComponent<PlayerInventory>();

                int money = inventory != null
                    ? inventory.TotalMoney
                    : 0;

                if (money > highestMoney)
                {
                    highestMoney = money;
                    winner = player;
                }
            }

            if (winner != null)
            {
                Debug.Log(
                    "GAME OVER: " + winner.playerName +
                    " wins with $" + highestMoney + "!"
                );
            }
            else
            {
                Debug.Log("GAME OVER: No valid escaped players.");
            }
        }
        else
        {
            // Nobody escaped and all robbers were caught.
            Debug.Log("GAME OVER: Cop wins!");
        }

        if (endScreen != null)
            endScreen.SetActive(true);
        else
            Debug.LogWarning("Game Over Panel is not assigned!");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
