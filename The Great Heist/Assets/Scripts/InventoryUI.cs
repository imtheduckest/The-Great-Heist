using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public TurnManager turnManager;
    public Transform rowsContainer;
    public PlayerInventoryRow rowPrefab;

    private List<PlayerInventoryRow> rows =
        new List<PlayerInventoryRow>();

    private void OnEnable()
    {
        PlayerInventory.OnInventoryChanged += RefreshAllRows;

        PlayerInventory.OnPlayerCaptured += RemovePlayerRow;
    }

    private void OnDisable()
    {
        PlayerInventory.OnInventoryChanged -= RefreshAllRows;

        PlayerInventory.OnPlayerCaptured -= RemovePlayerRow;
    }

    private IEnumerator Start()
    {
        // Wait for TurnManager.Start() to finish sorting players.
        yield return null;

        CreateRows();
    }

    private void CreateRows()
    {
        if (turnManager == null || rowsContainer == null ||
            rowPrefab == null)
        {
            Debug.LogError("InventoryUI references are missing!");
            return;
        }

        foreach (Transform child in rowsContainer)
        {
            Destroy(child.gameObject);
        }

        rows.Clear();

        foreach (PlayerToken player in turnManager.players)
        {
            PlayerInventoryRow row = Instantiate(
                rowPrefab,
                rowsContainer
            );

            row.Initialize(player);
            rows.Add(row);
        }
    }

    public void RefreshAllRows()
    {
        foreach (PlayerInventoryRow row in rows)
        {
            row.Refresh();
        }
    }

    private void RemovePlayerRow(PlayerToken player)
    {
        foreach (PlayerInventoryRow row in rows)
        {
            if (row.Player == player)
            {
                rows.Remove(row);
                Destroy(row.gameObject);

                Debug.Log(player.playerName + "'s inventory row removed.");
                return;
            }
        }
    }
}
