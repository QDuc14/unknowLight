using UnityEngine;
using System;

public class WaypointMover : MonoBehaviour
{
    public enum Mode { Once, Loop, PingPong }

    [Header("Path")]
    public Transform pathParent;
    public Transform[] waypoints;
    public bool usePathParentChildren = true;

    [Header("Movement")]
    public Mode mode = Mode.Once;
    public float speed = 2.3f;
    public float arriveRadius = 0.06f;
    public float waitAtWaypoint = 0.0f;
    public bool faceVelocity = true;

    [Header("Start")]
    public bool playOnStart = true;
    public int startIndex = 0;
    public bool reverseAtStart = false;

    public event Action<int> OnWaypointReached;
    public event Action OnPathCompleted;
    public bool IsPlaying => playing;
    public int CurrentIndex => i;

    Rigidbody2D rb;
    int i = 0, dir = 1;
    float waitTimer = 0f;
    bool playing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (usePathParentChildren && pathParent != null)
        {
            int n = pathParent.childCount;
            waypoints = new Transform[n];
            for (int k = 0; k < n; k++) waypoints[k] = pathParent.GetChild(k);
        }
        if (waypoints == null || waypoints.Length == 0)
            Debug.LogWarning($"{name}: WaypointMover2D has no waypoints.");
    }

    void Start()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            i = Mathf.Clamp(startIndex, 0, waypoints.Length - 1);
            dir = reverseAtStart ? -1 : 1;
            if (playOnStart) Play();
        }
    }

    public void Play() { playing = true; }
    public void Pause() { playing = false; if (rb) rb.linearVelocity = Vector2.zero; }
    public void Stop()
    {
        playing = false;
        i = Mathf.Clamp(startIndex, 0, (waypoints?.Length ?? 1) - 1);
        if (rb) rb.linearVelocity = Vector2.zero;
    }

    public void SetPathParent(Transform parent, bool autoChildren = true)
    {
        pathParent = parent;
        usePathParentChildren = autoChildren;
        if (autoChildren && parent != null)
        {
            int n = parent.childCount;
            waypoints = new Transform[n];
            for (int k = 0; k < n; k++) waypoints[k] = parent.GetChild(k);
        }
    }

    public void SetWaypoints(Transform[] pts, bool restart = true)
    {
        waypoints = pts;
        usePathParentChildren = false;
        if (restart) { startIndex = 0; i = 0; dir = 1; waitTimer = 0f; Play(); }
    }

    public void SetWaypointsPositions(Vector2[] pts, bool restart = true)
    {
        var gos = new Transform[pts.Length];
        for (int k = 0; k < pts.Length; k++)
        {
            var go = new GameObject($"wp_{k}");
            go.transform.position = pts[k];
            gos[k] = go.transform;
        }
        SetWaypoints(gos, restart);
    }

    void FixedUpdate()
    {
        if (!playing || waypoints == null || waypoints.Length == 0) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (rb) rb.linearVelocity = Vector2.zero;
            return;
        }

        var target = (Vector2)waypoints[i].position;
        var pos = (Vector2)transform.position;
        var to = target - pos;

        if (to.sqrMagnitude <= arriveRadius * arriveRadius)
        {
            OnWaypointReached?.Invoke(i);

            int next = i + dir;
            bool atEnd = (next < 0 || next >= waypoints.Length);

            if (mode == Mode.Once && atEnd)
            {
                playing = false;
                if (rb) rb.linearVelocity = Vector2.zero;
                OnPathCompleted?.Invoke();
                return;
            }
            if (mode == Mode.Loop && atEnd)
            {
                next = (dir > 0) ? 0 : waypoints.Length - 1;
            }
            if (mode == Mode.PingPong && atEnd)
            {
                dir *= -1;
                next = i + dir;
            }

            i = Mathf.Clamp(next, 0, waypoints.Length - 1);
            waitTimer = waitAtWaypoint;
            return;
        }

        Vector2 v = to.normalized * speed;
        if (rb) rb.linearVelocity = v;
        else transform.position = Vector2.MoveTowards(pos, target, speed * Time.fixedDeltaTime);

        if (faceVelocity && v.sqrMagnitude > 0.0001f)
        {
            float ang = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, ang - 90f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Transform[] pts = waypoints;
        if (usePathParentChildren && pathParent != null)
        {
            int n = pathParent.childCount; pts = new Transform[n];
            for (int k = 0; k < n; k++) pts[k] = pathParent.GetChild(k);
        }
        if (pts == null || pts.Length < 2) return;
        Gizmos.color = new Color(0.1f, 0.9f, 0.3f, 0.9f);
        for (int k = 0; k < pts.Length - 1; k++) Gizmos.DrawLine(pts[k].position, pts[k + 1].position);
        foreach (var t in pts) Gizmos.DrawWireSphere(t.position, 0.1f);
    }
}
