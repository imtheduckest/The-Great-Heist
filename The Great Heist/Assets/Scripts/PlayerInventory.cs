using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<CardData> valuableCards = new List<CardData>();
    public List<CardData> itemCards = new List<CardData>();
    public static event System.Action OnInventoryChanged;
    public static event System.Action<PlayerToken> OnPlayerCaptured;

    public int TotalMoney
    {
        get
        {
            int total = 0;

            foreach (CardData card in valuableCards)
            {
                total += card.value;
            }

            return total;
        }
    }

    public void AddCard(CardData card)
    {
        if (card == null)
            return;

        switch (card.cardType)
        {
            case CardType.Valuable:
                valuableCards.Add(card);
                break;

            case CardType.Item:
                itemCards.Add(card);
                break;

            case CardType.Event:
                Debug.Log("Event card received: " + card.cardName);
                break;
        }

        Debug.Log(
            $"{GetComponent<PlayerToken>().playerName} received {card.cardName}. " +
            $"Total money: ${TotalMoney}"
        );

        OnInventoryChanged?.Invoke();
    }

    public void ClearForCapture()
    {
        valuableCards.Clear();
        itemCards.Clear();

        PlayerToken player = GetComponent<PlayerToken>();

        Debug.Log(player.playerName + "'s inventory has been cleared.");

        OnInventoryChanged?.Invoke();
        OnPlayerCaptured?.Invoke(player);
    }
}
