using UnityEngine;

[CreateAssetMenu(
    fileName = "NewCard",
    menuName = "Heist/Card Definition"
)]

public class CardDefinition : ScriptableObject
{
    [Header("Card Information")]
    public string cardName;
    public CardType cardType;
    public int value;

    [TextArea(3, 6)]
    public string description;

    [Header("Card Visuals")]
    public GameObject cardPrefab;
    public Sprite inventoryIcon;
}
