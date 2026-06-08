using UnityEngine;

public class Path : MonoBehaviour
{
    public Transform[] GetWaypoints()
    {
        Transform[] points = new Transform[transform.childCount];
        for (int i = 0; i < points.Length; i++)
            points[i] = transform.GetChild(i);
        return points;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position,
                            transform.GetChild(i + 1).position);
            Gizmos.DrawWireSphere(transform.GetChild(i).position, 0.15f);
        }
        if (transform.childCount > 0)
            Gizmos.DrawWireSphere(transform.GetChild(transform.childCount - 1).position, 0.15f);
    }
}
