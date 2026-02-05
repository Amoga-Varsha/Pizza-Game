using UnityEngine;

public class AiWorldFactsSensor : MonoBehaviour
{
    [SerializeField] private AiContext context;
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask lineOfSightMask = ~0;
    [SerializeField] private float updateInterval = 0.2f;
    [SerializeField] private float maxRayDistance = 50f;
    [SerializeField] private float coverScore;
    [Header("Debug")]
    [SerializeField] private bool drawDebugRay;
    [SerializeField] private Color debugRayColor = Color.cyan;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < updateInterval)
        {
            return;
        }

        timer = 0f;
        var origin = transform.position + Vector3.up * 1.6f;
        var targetPos = target != null ? GetTargetAimPoint(target) : origin + transform.forward * maxRayDistance;
        var direction = targetPos - origin;
        var distance = direction.magnitude;

        if (drawDebugRay)
        {
            var drawDistance = Mathf.Min(distance, maxRayDistance);
            Debug.DrawRay(origin, direction.normalized * drawDistance, debugRayColor, updateInterval);
        }

        if (context == null || target == null)
        {
            return;
        }

        var hasLos = false;
        if (distance <= maxRayDistance)
        {
            if (Physics.Raycast(origin, direction.normalized, out var hit, distance, lineOfSightMask))
            {
                hasLos = hit.transform == target || hit.transform.IsChildOf(target);
            }
        }

        context.SetWorldFacts(hasLos, distance, coverScore);
    }

    private static Vector3 GetTargetAimPoint(Transform targetTransform)
    {
        if (targetTransform == null)
        {
            return Vector3.zero;
        }

        var collider = targetTransform.GetComponentInChildren<Collider>();
        if (collider != null)
        {
            return collider.bounds.center;
        }

        var renderer = targetTransform.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.center;
        }

        return targetTransform.position;
    }
}
