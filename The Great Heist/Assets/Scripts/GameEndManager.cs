using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    public GameObject endScreen;

    private bool gameEnded;

    public bool GameEnded => gameEnded;

    void Start()
    {
        if (endScreen != null)
            endScreen.SetActive(false);
    }

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

            if (endScreen != null)
                endScreen.SetActive(true);
            else
                Debug.LogWarning("Game Over Panel is not assigned!");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
