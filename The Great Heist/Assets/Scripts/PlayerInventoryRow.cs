using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerInventoryRow : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text playerInfoText;
    public Transform itemIconsContainer;

    [Header("Icon Settings")]
    public Vector2 iconSize = new Vector2(32, 32);

    private PlayerInventory inventory;
    private PlayerToken player;

    public PlayerToken Player => player;

    public void Initialize(PlayerToken player)
    {
        this.player = player;

        inventory = player.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogError(player.playerName + " has no PlayerInventory!");
            return;
        }

        Refresh();
    }

    public void Refresh()
    {
        if (inventory == null) return;

        PlayerToken player = inventory.GetComponent<PlayerToken>();

        playerInfoText.text =
            player.playerName + ": " + inventory.TotalMoney + "$";

        // Remove old icons before rebuilding them.
        foreach (Transform child in itemIconsContainer)
        {
            Destroy(child.gameObject);
        }

        // Create an icon for every item in the inventory.
        foreach (CardData card in inventory.itemCards)
        {
            if (card.cardDefinition == null ||
                card.cardDefinition.inventoryIcon == null)
            {
                continue;
            }

            GameObject iconObject = new GameObject(
                card.cardName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );

            iconObject.transform.SetParent(itemIconsContainer, false);

            Image iconImage = iconObject.GetComponent<Image>();
            iconImage.sprite = card.cardDefinition.inventoryIcon;
            iconImage.preserveAspect = true;

            RectTransform rect = iconObject.GetComponent<RectTransform>();
            rect.sizeDelta = iconSize;
        }
    }
}
