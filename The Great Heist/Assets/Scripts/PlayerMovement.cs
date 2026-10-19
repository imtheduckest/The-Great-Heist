using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public DiceRoll diceRoll;
    public CaptureDetector captureDetector;

    [Header("Movement")]
    public int gridX;
    public int gridY;
    public float tokenHeight = 0.5f;

    [Header("Legal Move Visual")]
    public GameObject highlightPrefab;
    public float highlightHeight = 0.06f;

    private readonly List<GameObject> highlights =
        new List<GameObject>();

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();

        if (diceRoll == null)
            diceRoll = FindObjectOfType<DiceRoll>();

        if (captureDetector == null)
            captureDetector = FindObjectOfType<CaptureDetector>();

        if (gridManager == null)
        {
            Debug.LogError("[PlayerMovement] GridManager missing!");
            enabled = false;
            return;
        }

        // Calculate the starting tile from the token's position.
        float centerOffset = (gridManager.width - 1) * 0.5f;

        gridX = Mathf.RoundToInt(
            (transform.position.x - gridManager.gameBoard.x)
            / gridManager.tileSpacing + centerOffset
        );

        gridY = Mathf.RoundToInt(
            (transform.position.z - gridManager.gameBoard.z)
            / gridManager.tileSpacing + centerOffset
        );

        SnapToGrid();
    }

    void Update()
    {
        if (diceRoll == null ||
            !diceRoll.hasRolledThisTurn ||
            diceRoll.remainingMoves <= 0)
        {
            ClearHighlights();
            return;
        }

        // Only the active player's token can move.
        if (diceRoll.turnManager == null ||
            diceRoll.turnManager.ActivePlayer != GetComponent<PlayerToken>())
        {
            ClearHighlights();
            return;
        }

        ShowLegalMoves();

        if (Input.GetMouseButtonDown(0))
        {
            // Ignore clicks on UI, such as the dice button.
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            TryClickTile();
        }
    }

    void SnapToGrid()
    {
        Vector3 position =
            gridManager.GetTileWorldPosition(gridX, gridY);

        position.y += tokenHeight;
        transform.position = position;
    }

    void ShowLegalMoves()
    {
        ClearHighlights();

        // Four directions only; no diagonals.
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int direction in directions)
        {
            int targetX = gridX + direction.x;
            int targetY = gridY + direction.y;

            if (!gridManager.CanMoveTo(targetX, targetY))
                continue;

            // Do not allow moving onto another figure.
            //if (IsOccupiedByAnotherPlayer(targetX, targetY))
            //    continue;

            CreateHighlight(targetX, targetY);
        }
    }

    void CreateHighlight(int x, int y)
    {
        if (highlightPrefab == null)
            return;

        Vector3 position =
            gridManager.GetTileWorldPosition(x, y);

        position.y += highlightHeight;

        GameObject highlight = Instantiate(
            highlightPrefab,
            position,
            Quaternion.identity
        );

        // Prevent the highlight from intercepting other interactions.
        Collider[] colliders =
            highlight.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
            col.enabled = false;

        highlights.Add(highlight);
    }

    void ClearHighlights()
    {
        foreach (GameObject highlight in highlights)
        {
            if (highlight != null)
                Destroy(highlight);
        }

        highlights.Clear();
    }

    bool IsOccupiedByAnotherPlayer(int x, int y)
    {
        PlayerToken[] tokens =
            FindObjectsOfType<PlayerToken>();

        foreach (PlayerToken token in tokens)
        {
            if (token.gameObject == gameObject)
                continue;

            PlayerMovement movement =
                token.GetComponent<PlayerMovement>();

            if (movement != null &&
                movement.gridX == x &&
                movement.gridY == y)
                return true;
        }

        return false;
    }

    void TryClickTile()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(
            Input.mousePosition
        );

        Plane boardPlane = new Plane(
            Vector3.up,
            new Vector3(
                0f,
                gridManager.gameBoard.y,
                0f
            )
        );

        if (!boardPlane.Raycast(ray, out float distance))
            return;

        Vector3 hitPoint = ray.GetPoint(distance);

        float centerOffset = (gridManager.width - 1) * 0.5f;

        int targetX = Mathf.RoundToInt(
            (hitPoint.x - gridManager.gameBoard.x)
            / gridManager.tileSpacing + centerOffset
        );

        int targetY = Mathf.RoundToInt(
            (hitPoint.z - gridManager.gameBoard.z)
            / gridManager.tileSpacing + centerOffset
        );

        TryMoveStep(targetX, targetY);
    }

    public bool TryMoveStep(int targetX, int targetY)
    {
        if (diceRoll == null ||
            !diceRoll.hasRolledThisTurn ||
            diceRoll.remainingMoves <= 0)
            return false;

        // Exactly one tile in a cardinal direction.
        int distance =
            Mathf.Abs(targetX - gridX) +
            Mathf.Abs(targetY - gridY);

        if (distance != 1)
            return false;

        if (!gridManager.CanMoveTo(targetX, targetY))
            return false;

        //if (IsOccupiedByAnotherPlayer(targetX, targetY))
        //    return false;

        gridX = targetX;
        gridY = targetY;

        SnapToGrid();
        ClearHighlights();

        if (captureDetector != null)
            captureDetector.CheckForCapture(this);

        // Consume exactly one move after a successful move.
        diceRoll.ConsumeMove();

        return true;
    }

    void OnDisable()
    {
        ClearHighlights();
    }
}
