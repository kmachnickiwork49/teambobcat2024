using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RangeUtils
{
    public static void VisualizePosition(Vector3 pos)
    {
        Gizmos.color = Color.red;
        float size = 0.2f;
        Vector3 offset1 = new(size, size, 0);
        Vector3 offset2 = new(size, -size, 0);
        Gizmos.DrawLine(pos, pos + offset1);
        Gizmos.DrawLine(pos, pos - offset2);
    }

    public static void VisualizeRange(Transform transform, float range)
    {
        int numSegments = 16;
        Gizmos.color = Color.white;
        for (float angle = 0; angle <= 2 * Mathf.PI; angle += 2 * Mathf.PI / numSegments)
        {
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(range * Mathf.Cos(angle), range * 0.5f * Mathf.Sin(angle), 0f));
        }
    }

    public static bool TargetInRange(Transform transform, Transform target, float range)
    {
        Vector3 direction = (new Vector3(target.position.x, target.position.y, transform.position.z)  - transform.position);
        float angleRad = Vector3.Angle(Vector3.up, direction) * Mathf.PI / 180;
        float rangeRadius = Mathf.Sqrt(Mathf.Pow(range * Mathf.Cos(angleRad), 2) + Mathf.Pow(range * 0.5f * Mathf.Sin(angleRad), 2));
        return direction.magnitude <= rangeRadius;
    }
}
