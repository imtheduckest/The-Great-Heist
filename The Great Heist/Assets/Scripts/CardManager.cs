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

        // 60 valuable cards — placeholders to edit later.
        for (int i = 1; i <= 60; i++)
        {
            deck.Add(new CardData(
                "Valuable " + i.ToString("00"),
                CardType.Valuable,
                1000,
                "Placeholder valuable card."
            ));
        }

        // 20 item cards.

        AddCards("Pistol", 2, CardType.Item,
            "Within 2 squares: shoot and immobilize the player until they roll 6 on their next turn.");

        AddCards("Rifle", 2, CardType.Item,
            "Within 3 squares: immobilize the player until they roll 2 or higher on their next turn.");

        AddCards("Lasso", 4, CardType.Item,
            "Within 1 square: immobilize the player until they roll 2 or higher on their next turn.");

        AddCards("Trap", 3, CardType.Item,
            "Place on any square. A player who tries to pass it is immobilized until rolling 3 or higher. You may immediately roll the die and finish your move if you have moves remaining.");

        AddCards("Banana Peel", 4, CardType.Item,
            "Place anywhere. A player who tries to pass it trips and loses their remaining moves.");

        AddCards("Wall", 2, CardType.Item,
            "Build a temporary wall on any square for 2 full turn cycles. Players cannot pass through or occupy it. A Pickaxe can destroy it.");

        AddCards("Pickaxe", 2, CardType.Item,
            "Break bank walls or player-created walls. Board changes remain until the end of the game.");

        AddCards("Police Badge", 1, CardType.Item,
            "If the cop catches you, reveal the badge to avoid being caught. You can move on your next turn.");

        // 16 event cards.

        AddCards("SWAT Team", 1, CardType.Event,
            "SWAT raids the bank territory for 2 full turn cycles. Three operatives appear 6 tiles from the center in 3 directions. The cop may split movement between the cop and SWAT operatives. SWAT evacuates after 2 cycles if it catches no more than one robber. SWAT can catch only one robber.");

        AddCards("Fire Alarm", 1, CardType.Event,
            "The bank's emergency doors close for 2 full turn cycles.");

        AddCards("Luck", 4, CardType.Event,
            "Draw 1 extra card from a pile.");

        AddCards("Bomb", 1, CardType.Event,
            "You explode in the cell. All your cards are burned and removed from the game.");

        AddCards("Security Alarm", 2, CardType.Event,
            "Cells cannot be opened for 2 full turn cycles.");

        AddCards("Splinter", 4, CardType.Event,
            "Drop 1 card.");

        AddCards("Mousetrap", 3, CardType.Event,
            "Drop 3 cards.");

        Debug.Log(
            "Created deck: " + deck.Count +
            " cards. Expected: 96."
        );
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
