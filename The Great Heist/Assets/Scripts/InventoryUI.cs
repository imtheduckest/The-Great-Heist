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
    }

    private void OnDisable()
    {
        PlayerInventory.OnInventoryChanged -= RefreshAllRows;
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
}
