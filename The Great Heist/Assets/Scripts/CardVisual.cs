using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script connects the card's data to its spawned Plane. It does not implement any card actions. Simple connector
public class CardVisual : MonoBehaviour
{
    public CardData cardData;
    private CardManager cardManager;

    [Header("State")]
    public bool isTopCard;
    private bool isRevealed;

    public void Initialize(CardManager manager, CardData data)
    {
        cardManager = manager;
        cardData = data;
        isTopCard = false;
        isRevealed = false;
    }

    void OnMouseDown()
    {
        if (!isTopCard)
            return;

        if (cardData != null)
        {
            Debug.Log("Clicked card: " + cardData.cardName);
        }

        if (cardManager != null)
            cardManager.RemoveCard(this);
    }

    public void MarkRevealed() //placeholders
    {
        isRevealed = true;
    }
}
