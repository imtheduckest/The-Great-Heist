using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardEntry
{
    public CardDefinition definition;
    public int quantity;
}

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

    [Header("Deck Setup")]
    public List<CardEntry> deckEntries = new List<CardEntry>();

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

        foreach (CardEntry entry in deckEntries)
        {
            if (entry.definition == null || entry.quantity <= 0)
            {
                Debug.LogWarning("CardManager: Invalid deck entry.");
                continue;
            }

            for (int i = 0; i < entry.quantity; i++)
            {
                CardDefinition definition = entry.definition;

                deck.Add(new CardData(
                    definition.cardName,
                    definition.cardType,
                    definition.value,
                    definition.description
                ));
            }
        }

        Debug.Log("Created deck with " + deck.Count + " cards.");

        if (deck.Count != 96)
        {
            Debug.LogWarning(
                "Expected 96 cards, but deck contains " + deck.Count + "."
            );
        }
    }

    void AddCards(string cardName, int quantity, CardType cardType, string description)
    {
        for (int i = 0; i < quantity; i++)
        {
            deck.Add(new CardData(
                cardName,
                cardType,
                0,
                description
            ));
        }
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
                    cardPrefab,
                    cardPosition,
                    Quaternion.identity,
                    pileObject.transform);

                cardVisual.name = card.cardName;

                CardVisual visualData = cardVisual.GetComponent<CardVisual>();

                if (visualData == null)
                {
                    visualData = cardVisual.AddComponent<CardVisual>();
                }

                visualData.cardData = card;

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
