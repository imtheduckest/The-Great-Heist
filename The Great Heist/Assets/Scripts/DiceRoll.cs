using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class DiceRoll : MonoBehaviour
{
    [Header("References")]
    public TurnManager turnManager;

    [Header("UI & State")]
    public TextMeshProUGUI diceResultText;
    public TextMeshProUGUI diceResultTemporaryText;

    public int remainingMoves { get; private set; } = 0;

    public bool hasRolledThisTurn = false;

    void Start()
    {
        if (diceResultTemporaryText != null) diceResultTemporaryText.gameObject.SetActive(false);

        UpdateUI();
    }

    // PlayerMovement now handles board clicks and movement

    public void RollDice()
    {
        if (CardVisual.ControlsLocked)
            return;

        if (turnManager == null || turnManager.ActivePlayer == null)
        {
            Debug.LogWarning("[DiceRoll] No active player.");
            return;
        }

        if (hasRolledThisTurn)
        {
            Debug.Log("[DiceRoll] You already rolled! Finish your moves first.");
            return;
        }

        // Roll random number from 1 to 6
        int rawRoll = Random.Range(1, 7);

        if (turnManager.ActivePlayer.isCop)
        {
            remainingMoves = Mathf.Max(0, rawRoll - 1);
            Debug.Log($"[DiceRoll] COP rolled {rawRoll} (-1 rule) = {remainingMoves} steps.");
        }

        else
        {
            remainingMoves = rawRoll;
            Debug.Log($"[DiceRoll] {turnManager.ActivePlayer.playerName} rolled {remainingMoves} steps.");
        }

        hasRolledThisTurn = true;

        Debug.Log($"[DiceRoll] {turnManager.ActivePlayer.playerName} rolled {rawRoll}.");

        UpdateUI();

        // Show the result temporarily, then hide the text while keeping the int value
        StartCoroutine(ShowAndHideRoll(rawRoll, 1.5f));

        if (remainingMoves <= 0)
        {
            Debug.Log("[DiceRoll] 0 moves remaining. Ending turn automatically.");
            StartCoroutine(AutoEndTurnAfterDelay(1.5f));
        }
    }

    IEnumerator AutoEndTurnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (turnManager != null)
        {
            turnManager.EndTurn();
        }
        else
        {
            ResetDice();
        }
    }

    IEnumerator ShowAndHideRoll(int rolledNumber, float displayDuration)
    {
        if (diceResultTemporaryText != null)
        {
            diceResultTemporaryText.gameObject.SetActive(true);
            diceResultTemporaryText.text = rolledNumber.ToString();
        }

        yield return new WaitForSeconds(displayDuration);

        if (diceResultTemporaryText != null)
        {
            diceResultTemporaryText.gameObject.SetActive(false);
        }
    }

    void UseMove()
    {
        remainingMoves--;

        Debug.Log($"[DiceRoll] Move used. Remaining: {remainingMoves}");

        UpdateUI();

        if (remainingMoves <= 0)
        {
            Debug.Log("[DiceRoll] All moves used. Ending turn.");

            if (turnManager != null) turnManager.EndTurn();
            else ResetDice();
        }
    }

    public void ConsumeMove()
    {
        if (!hasRolledThisTurn || remainingMoves <= 0)
            return;

        remainingMoves--;

        Debug.Log(
            $"[DiceRoll] Move used! Remaining: {remainingMoves}"
        );

        UpdateUI();

        if (remainingMoves <= 0)
        {
            Debug.Log("[DiceRoll] All moves used. Ending turn.");

            if (turnManager != null)
                turnManager.EndTurn();
            else
                ResetDice();
        }
    }

    public void ResetDice()
    {
        remainingMoves = 0;
        hasRolledThisTurn = false;

        if (diceResultTemporaryText != null)
        {
            diceResultTemporaryText.gameObject.SetActive(false);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (diceResultText != null)
        {
            diceResultText.text = hasRolledThisTurn ? remainingMoves.ToString() : "";
        }
    }
}
