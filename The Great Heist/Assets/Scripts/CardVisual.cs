using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script connects the card's data to its spawned Plane. It does not implement any card actions. Simple connector
public class CardVisual : MonoBehaviour
{
    public CardData cardData;
    private CardManager cardManager;
    private Camera cam;

    [Header("State")]
    public bool isTopCard;
    private bool isAnimating;
    private bool isDisplayed;

    [Header("Reveal Animation")]
    public float moveToCameraDuration = 0.6f;
    public float displayDistance = 3f;
    public float slideDownDistance = 8f;
    public float slideDownDuration = 0.4f;

    public void Initialize(CardManager manager, CardData data)
    {
        cardManager = manager;
        cardData = data;
        isTopCard = false;
        isDisplayed = false;
        isAnimating = false;
    }

    void Awake()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        Debug.Log("Card clicked: " + gameObject.name);

        if (!isTopCard || isAnimating || isDisplayed)
            return;

        if (cardManager != null)
            cardManager.RevealCard(this);
    }

    public void StartReveal()
    {
        if (cam == null)
            cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("CardVisual: Main Camera not found.");
            return;
        }

        isAnimating = true;

        if (cardData != null)
        {
            Debug.Log("Revealed card: " + cardData.cardName);
        }

        StartCoroutine(RevealAnimation());
    }

    IEnumerator RevealAnimation()
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        // Move the card in front of the camera.
        Vector3 targetPosition =
            cam.transform.position +
            cam.transform.forward * displayDistance;

        // Unity Plane faces along its local Y axis.
        // Rotate that face toward the camera.
        Quaternion targetRotation = Quaternion.LookRotation(
            cam.transform.up,
            -cam.transform.forward
        );

        float elapsed = 0f;

        while (elapsed < moveToCameraDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / moveToCameraDuration
            );

            // Smooth start and finish.
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                smoothT
            );

            transform.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                smoothT
            );

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        isAnimating = false;
        isDisplayed = true;
    }

    void Update()
    {
        // Wait until the reveal animation has finished.
        if (!isDisplayed || isAnimating)
            return;

        // Any left-click removes the displayed card.
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(SlideDownAndRemove());
        }
    }

    IEnumerator SlideDownAndRemove()
    {
        isAnimating = true;
        isDisplayed = false;

        Vector3 startPosition = transform.position;

        Vector3 targetPosition =
            startPosition -
            cam.transform.up * slideDownDistance;

        float elapsed = 0f;

        while (elapsed < slideDownDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / slideDownDuration
            );

            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                smoothT
            );

            yield return null;
        }

        if (cardManager != null)
        {
            cardManager.RemoveCard(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
