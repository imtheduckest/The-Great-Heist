using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCameraOrbit : MonoBehaviour
{
    [Header("Target")]
    public Transform boardCenter;
    public Camera cam;

    [Header("Orbit")]
    public float sensitivity = 3f;
    public float minPitch = -80f;
    public float maxPitch = -10f;

    [Header("Zoom")]
    public float zoomSensitivity = 5f;
    public float minDistance = 5f;
    public float maxDistance = 25f;

    [Header("Smooth Movement")]
    public float focusSmoothTime = 0.35f;
    public float rotationSmoothSpeed = 8f;

    [Header("Double Click")]
    public float doubleClickInterval = 0.3f;

    private Transform currentTarget;
    private Vector3 focusPoint;
    private Vector3 focusVelocity;

    private float yaw;
    private float pitch;
    private float distance;

    private float targetYaw;
    private float targetPitch;

    private float lastClickTime = -1f;
    private PlayerToken lastClickedPlayer;

    void Start()
    {
        if (boardCenter == null)
        {
            Debug.LogError(
                "BoardCameraOrbit: Board Center is not assigned."
            );
            enabled = false;
            return;
        }

        if (cam == null)
            cam = GetComponent<Camera>();

        if (cam == null)
            cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("BoardCameraOrbit: Camera not found.");
            enabled = false;
            return;
        }

        currentTarget = boardCenter;
        focusPoint = boardCenter.position;

        Vector3 offset = transform.position - focusPoint;

        distance = Mathf.Clamp(
            offset.magnitude,
            minDistance,
            maxDistance
        );

        targetYaw = yaw = Mathf.Atan2(
            offset.x,
            offset.z
        ) * Mathf.Rad2Deg;

        float horizontalDistance =
            new Vector2(offset.x, offset.z).magnitude;

        targetPitch = pitch = Mathf.Clamp(
            Mathf.Atan2(offset.y, horizontalDistance)
                * Mathf.Rad2Deg,
            minPitch,
            maxPitch
        );

        UpdateCameraPosition();
    }

    void Update()
    {
        HandleOrbit();
        HandleZoom();
        HandleDoubleClick();
    }

    void LateUpdate()
    {
        // Smoothly move the camera's focus point.
        if (currentTarget != null)
        {
            focusPoint = Vector3.SmoothDamp(
                focusPoint,
                currentTarget.position,
                ref focusVelocity,
                focusSmoothTime
            );
        }

        // Smoothly rotate toward the requested angles.
        yaw = Mathf.LerpAngle(
            yaw,
            targetYaw,
            rotationSmoothSpeed * Time.deltaTime
        );

        pitch = Mathf.Lerp(
            pitch,
            targetPitch,
            rotationSmoothSpeed * Time.deltaTime
        );

        UpdateCameraPosition();
    }

    void HandleOrbit()
    {
        if (!Input.GetMouseButton(1))
            return;

        targetYaw += Input.GetAxis("Mouse X") * sensitivity;
        targetPitch += Input.GetAxis("Mouse Y") * sensitivity;

        targetPitch = Mathf.Clamp(
            targetPitch,
            minPitch,
            maxPitch
        );
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) < 0.001f)
            return;

        distance -= scroll * zoomSensitivity;

        distance = Mathf.Clamp(
            distance,
            minDistance,
            maxDistance
        );
    }

    void HandleDoubleClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            lastClickedPlayer = null;
            lastClickTime = -1f;
            return;
        }

        PlayerToken player =
            hit.collider.GetComponentInParent<PlayerToken>();

        if (player == null)
        {
            lastClickedPlayer = null;
            lastClickTime = -1f;
            return;
        }

        bool isDoubleClick =
            player == lastClickedPlayer &&
            Time.time - lastClickTime <= doubleClickInterval;

        if (isDoubleClick)
        {
            if (currentTarget == player.transform)
            {
                // Double-click the focused player again
                // to return to the board.
                currentTarget = boardCenter;
            }
            else
            {
                // Focus on the selected player.
                currentTarget = player.transform;
            }

            // Prevent the same click from being reused.
            lastClickTime = -1f;
            lastClickedPlayer = null;
        }
        else
        {
            lastClickedPlayer = player;
            lastClickTime = Time.time;
        }
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(
            pitch,
            yaw,
            0f
        );

        Vector3 offset = rotation * Vector3.forward * distance;

        transform.position = focusPoint + offset;
        transform.LookAt(focusPoint);
    }
}
