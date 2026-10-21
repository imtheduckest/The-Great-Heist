using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public GameObject cardPrefab;

    [Header("Card Stack Settings")]
    public float cardSpacing = 0.03f;
    public float startHeight = 0.1f;

    [Header("Card Colors")]
    public Color valuableColor = Color.yellow;
    public Color itemColor = Color.green;
    public Color eventColor = Color.red;

    private List<CardData> deck = new List<CardData>();

    private readonly Vector2Int[] pilePositions =
    {
        new Vector2Int(1, 5),
        new Vector2Int(1, 1),
        new Vector2Int(5, 1),
        new Vector2Int(5, 5)
    };

    void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError("CardManager: GridManager is not assigned.");
            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogError("CardManager: Card Prefab is not assigned.");
            return;
        }

        CreateDeck();
        ShuffleDeck();
        SpawnCardPiles();
    }

    void CreateDeck()
    {
        deck.Clear();

        // 60 valuable cards
        for (int i = 0; i < 60; i++)
        {
            deck.Add(new CardData("Valuable " + (i + 1),
                CardType.Valuable, 1000));
        }

        // 20 item cards
        for (int i = 0; i < 20; i++)
        {
            deck.Add(new CardData("Item " + (i + 1),
                CardType.Item, 0));
        }

        // 16 event cards
        for (int i = 0; i < 16; i++)
        {
            deck.Add(new CardData("Event " + (i + 1),
                CardType.Event, 0));
        }

        Debug.Log("Created deck with " + deck.Count + " cards.");
    }

    void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            CardData temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void SpawnCardPiles()
    {
        int cardIndex = 0;

        for (int pileIndex = 0; pileIndex < pilePositions.Length; pileIndex++)
        {
            Vector2Int gridPosition = pilePositions[pileIndex];

            Vector3 tilePosition = gridManager.GetTileWorldPosition(
                gridPosition.x, gridPosition.y);

            GameObject pileObject = new GameObject(
                "CardPile_" + (pileIndex + 1));

            pileObject.transform.SetParent(transform);

            for (int i = 0; i < 24; i++)
            {
                CardData card = deck[cardIndex++];

                Vector3 cardPosition = tilePosition +
                    Vector3.up * (startHeight + i * cardSpacing);

                GameObject cardVisual = Instantiate(
                    cardPrefab, cardPosition, Quaternion.identity,
                    pileObject.transform);

                cardVisual.name = card.cardName;

                Renderer cardRenderer =
                    cardVisual.GetComponent<Renderer>();

                if (cardRenderer != null)
                {
                    switch (card.cardType)
                    {
                        case CardType.Valuable:
                            cardRenderer.material.color = valuableColor;
                            break;

                        case CardType.Item:
                            cardRenderer.material.color = itemColor;
                            break;

                        case CardType.Event:
                            cardRenderer.material.color = eventColor;
                            break;
                    }
                }
            }
        }

        Debug.Log("Spawned 4 card piles with 24 cards each.");
    }
}
