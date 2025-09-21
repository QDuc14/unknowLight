using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class AgentGridMover2D : MonoBehaviour
{
    public AStar2D pathfinder;
    public float speed = 2.3f;
    public float arriveRadius = 0.05f;
    public float waypointRadius = 0.05f;

    Rigidbody2D rb;
    readonly List<Vector2> path = new();

    int idx = -1;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    public bool SetDestination(Vector2 goal)
    {
        if (pathfinder == null) return false;
        bool ok = pathfinder.FindPath(transform.position, goal, path);
        idx = ok ? 0 : -1;
        return ok;
    }

    void FixedUpdate()
    {
        if (idx < 0 || idx >= path.Count)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.2f);
            return;
        }

        Vector2 target = path[idx];
        Vector2 to = target - (Vector2)transform.position;

        // reached waypoint?
        if (to.sqrMagnitude <= waypointRadius * waypointRadius)
        {
            idx++;
            if (idx >= path.Count)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }
            target = path[idx];
            to = target - (Vector2)transform.position;
        }

        Vector2 v = to.normalized * speed;
        rb.linearVelocity = v;
    }

    public bool ReachedDestination()
    {
        if (idx < 0) return false;
        // if last point reached
        if (idx >= path.Count) return true;
        if (idx == path.Count - 1)
        {
            return (Vector2.Distance(transform.position, path[^1]) <= arriveRadius);
        }
        return false;
    }
}
