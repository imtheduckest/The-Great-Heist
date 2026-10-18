using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    [Header("References")]
    public DiceRoll diceRoll;
    public GridManager gridManager;

    [Header("Turn State")]
    public List<PlayerToken> players = new List<PlayerToken>();
    public int currentTurnIndex = 0;

    public PlayerToken ActivePlayer
    {
        get
        {
            if (players == null || players.Count == 0) return null;

            return players[currentTurnIndex];
        }
    }

    void Start()
    {
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();

        FindAndSortPlayersClockwise();
        AssignDefaultNames();

        if (players.Count == 0)
        {
            Debug.LogError("[TurnManager] No PlayerToken objects found!");
            return;
        }

        if (diceRoll != null) diceRoll.turnManager = this;

        else Debug.LogError("[TurnManager] DiceRoll reference is missing!");

        Debug.Log($"[TurnManager] Game started! First turn: {ActivePlayer.playerName}");
    }

    // Automatically sort players so index 0 is the player to the left of the Cop in clockwise order
    void FindAndSortPlayersClockwise()
    {
        players = new List<PlayerToken>(
            FindObjectsOfType<PlayerToken>()
        );

        if (players.Count == 0) return;

        int copIndex = players.FindIndex(p => p.isCop);

        if (copIndex == -1)
        {
            Debug.LogError("[TurnManager] No Cop found! Check the Cop tag.");
            return;
        }

        // Find the center of the board.
        Vector3 center = gridManager != null
            ? gridManager.gameBoard
            : Vector3.zero;

        // Sort figures clockwise around the board center.
        players.Sort((a, b) =>
        {
            float angleA = Mathf.Atan2(
                a.transform.position.z - center.z,
                a.transform.position.x - center.x
            );

            float angleB = Mathf.Atan2(
                b.transform.position.z - center.z,
                b.transform.position.x - center.x
            );

            return angleB.CompareTo(angleA);
        });

        // Find the Cop again after sorting.
        copIndex = players.FindIndex(p => p.isCop);

        // Start with the player immediately after the Cop
        // in clockwise order. The Cop goes last.
        List<PlayerToken> orderedPlayers = new List<PlayerToken>();

        for (int i = 1; i <= players.Count; i++)
        {
            int index = (copIndex + i) % players.Count;
            orderedPlayers.Add(players[index]);
        }

        players = orderedPlayers;
        currentTurnIndex = 0;

        Debug.Log("[TurnManager] Turn order:");

        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log($"{i + 1}. {players[i].playerName}");
        }
    }

    void AssignDefaultNames()
    {
        int robberCounter = 1;

        foreach (PlayerToken player in players)
        {
            if (!player.isCop)
            {
                player.SetupDefaultName(robberCounter);
                robberCounter++;
            }
        }
    }

    public void EndTurn()
    {
        if (players == null || players.Count == 0) return;

        currentTurnIndex = (currentTurnIndex + 1) % players.Count;

        if (diceRoll != null)
            diceRoll.ResetDice();

        Debug.Log($"[TurnManager] It is now {ActivePlayer.playerName}'s turn.");
    }
}
