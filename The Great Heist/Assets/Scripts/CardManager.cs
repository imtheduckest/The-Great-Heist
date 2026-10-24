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
    private CardVisual activeCard;

    [Header("Card Stack Settings")]
    public float cardSpacing = 0.03f;
    public float startHeight = 0.1f;

    [Header("Deck Setup")]
    public List<CardEntry> deckEntries = new List<CardEntry>();

    private List<CardData> deck = new List<CardData>();

    [Header("Pile Tracking")]
    private Dictionary<GameObject, List<CardVisual>> cardsByPile =
    new Dictionary<GameObject, List<CardVisual>>();

    private Dictionary<CardVisual, GameObject> pileByCard =
        new Dictionary<CardVisual, GameObject>();

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
                    definition.description,
                    definition
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
        cardsByPile.Clear();
        pileByCard.Clear();

        int cardIndex = 0;

        for (int pileIndex = 0; pileIndex < pilePositions.Length; pileIndex++)
        {
            Vector2Int gridPosition = pilePositions[pileIndex];

            Vector3 tilePosition = gridManager.GetTileWorldPosition(gridPosition.x, gridPosition.y);

            GameObject pileObject = new GameObject(
                "CardPile_" + (pileIndex + 1));

            pileObject.transform.SetParent(transform);

            List<CardVisual> pileCards = new List<CardVisual>();
            cardsByPile.Add(pileObject, pileCards);

            for (int i = 0; i < 24; i++)
            {
                CardData card = deck[cardIndex++];

                if (card.cardDefinition == null ||
                    card.cardDefinition.cardPrefab == null)
                {
                    Debug.LogError(
                        "Missing CardDefinition or card prefab for: " +
                        card.cardName
                    );

                    continue;
                }

                Vector3 cardPosition = tilePosition +
                    Vector3.up * (startHeight + i * cardSpacing);

                GameObject cardVisual = Instantiate(
                    card.cardDefinition.cardPrefab,
                    cardPosition,
                    card.cardDefinition.cardPrefab.transform.rotation,
                    pileObject.transform
                );

                cardVisual.name = card.cardName;

                CardVisual visualData =
                    cardVisual.GetComponent<CardVisual>();

                if (visualData == null)
                {
                    visualData = cardVisual.AddComponent<CardVisual>();
                }

                visualData.Initialize(this, card);

                // Register this card in its pile.
                pileCards.Add(visualData);
                pileByCard.Add(visualData, pileObject);
            }

            // The last spawned card is the top card.
            if (pileCards.Count > 0)
            {
                pileCards[pileCards.Count - 1].isTopCard = true;
            }
        }

        Debug.Log("Spawned 4 card piles with 24 cards each.");
    }

    public void RevealCard(CardVisual card)
    {
        if (card == null || activeCard != null)
            return;

        if (!card.isTopCard)
            return;

        if (!pileByCard.TryGetValue(
            card,
            out GameObject pileObject))
            return;

        if (!cardsByPile.TryGetValue(
            pileObject,
            out List<CardVisual> pileCards))
            return;

        // Remember which card is currently being displayed.
        activeCard = card;

        // Remove it from the logical pile immediately.
        pileCards.Remove(card);
        pileByCard.Remove(card);

        card.isTopCard = false;

        // Enable the next card in this pile.
        if (pileCards.Count > 0)
        {
            pileCards[pileCards.Count - 1].isTopCard = true;
        }

        Debug.Log(
            pileObject.name + ": " +
            pileCards.Count + " cards remaining."
        );

        // Keep the visual alive while it animates.
        card.StartReveal();
    }

    public void RemoveCard(CardVisual card)
    {
        if (card == null)
            return;

        if (activeCard != card)
            return;

        activeCard = null;

        // Temporarily remove the visual from the game.
        Destroy(card.gameObject);
    }
}
