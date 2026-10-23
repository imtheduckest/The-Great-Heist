using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script connects the card's data to its spawned Plane. It does not implement any card actions. Simple connector
public class CardVisual : MonoBehaviour
{
    public CardData cardData;

    [Header("State")]
    public bool isTopCard;
    private bool isRevealed;

    void OnMouseDown()
    {
        if (!isTopCard)
            return;

        RevealCard();
    }

    void RevealCard()
    {
        if (isRevealed)
            return;

        isRevealed = true;

        if (cardData != null)
            Debug.Log("Revealed card: " + cardData.cardName);

        else
            Debug.LogWarning("CardVisual: CardData is missing!");
    }
}
