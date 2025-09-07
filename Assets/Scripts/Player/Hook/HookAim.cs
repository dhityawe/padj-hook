using UnityEngine;

public class HookAim : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    private LineRenderer lineRenderer;

    [SerializeField] private PlayerBaseStats playerBaseStats; // Manually assign in Inspector
    [SerializeField] private Transform hookPoint; // Manually assign HookPoint
    [SerializeField] private Transform hookGameObject; // Manually assign the hook GameObject that needs to rotate

    void Start()
    {
        mainCam = Camera.main;

        if (playerBaseStats == null)
        {
            Debug.LogError("playerBaseStats is not assigned! Assign it manually in the Inspector.");
            return;
        }

        if (hookPoint == null)
        {
            Debug.LogError("hookPoint is not assigned! Assign it manually in the Inspector.");
            return;
        }

        if (hookGameObject == null)
        {
            Debug.LogWarning("hookGameObject is not assigned! The separate hook GameObject won't rotate.");
        }

        // Add a LineRenderer component if not already attached
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Setup LineRenderer properties
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 50;
        lineRenderer.loop = true;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;

        DrawHookRange();
    }

    void Update()
    {
        if (playerBaseStats == null || hookPoint == null) return; // Prevent errors
        DrawHookArea();
        DrawHookRange();
        RotateHookGameObject();
    }

    private void DrawHookArea()
    {
        // Rotate towards the mouse position
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void RotateHookGameObject()
    {
        // Rotate the separate hook GameObject to match this GameObject's rotation
        if (hookGameObject != null)
        {
            // Add 180 degrees to flip the rotation since it's backwards/upside down
            Quaternion flippedRotation = transform.rotation * Quaternion.Euler(0f, 0f, 180f);
            hookGameObject.rotation = flippedRotation;
        }
    }

    private void DrawHookRange()
    {
        if (hookPoint == null) return;

        float radius = playerBaseStats.HookRange;
        Vector3 center = hookPoint.position; // Ensure it uses HookPoint

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float angle = i * Mathf.PI * 2f / lineRenderer.positionCount;
            Vector3 position = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            lineRenderer.SetPosition(i, position);
        }
    }

}
