using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WaypointMover))]
public class CustomerFSM_Waypoints2D : MonoBehaviour
{
    public enum State { Entering, GoingToSeat, Eating, Leaving, Done }

    [Header("Paths")]
    public Transform goPathParent;
    public int seatWaypointIndex = -1;

    [Tooltip("If null, the leave path will be auto-built as the reverse of goPath up to Entry.")]
    public Transform leavePathParent;

    [Header("Timers")]
    public float eatSeconds = 8f;

    WaypointMover mover;
    State state;

    void Awake()
    {
        mover = GetComponent<WaypointMover>();
    }

    void Start()
    {
        // Wire events
        mover.OnWaypointReached += OnWaypointReached;
        mover.OnPathCompleted += OnPathCompleted;

        // Start the 'goPathParent' path
        mover.mode = WaypointMover.Mode.Once;
        mover.SetPathParent(goPathParent, autoChildren: true);

        // If seat index not set, assume last waypoint is the seat
        if (seatWaypointIndex < 0 && goPathParent != null)
            seatWaypointIndex = Mathf.Max(0, goPathParent.childCount - 1);

        state = State.Entering;
        mover.Play();
    }

    void OnDestroy()
    {
        if (mover != null)
        {
            mover.OnWaypointReached -= OnWaypointReached;
            mover.OnPathCompleted -= OnPathCompleted;
        }
    }

    void OnWaypointReached(int i)
    {
        if (state == State.Entering && i == seatWaypointIndex)
        {
            StartCoroutine(EatThenLeave());
        }
    }

    IEnumerator EatThenLeave()
    {
        state = State.Eating;
        mover.Pause();
        yield return new WaitForSeconds(eatSeconds);

        Transform[] leavePoints;

        if (leavePathParent != null && leavePathParent.childCount >= 1)
        {
            leavePoints = new Transform[leavePathParent.childCount];
            for (int k = 0; k < leavePoints.Length; k++)
                leavePoints[k] = leavePathParent.GetChild(k);
        }
        else
        {
            int count = seatWaypointIndex + 1;
            leavePoints = new Transform[count];
            for (int k = 0; k < count; k++)
                leavePoints[k] = goPathParent.GetChild(seatWaypointIndex - k);
        }

        mover.SetWaypoints(leavePoints, restart: true);
        state = State.Leaving;
    }

    void OnPathCompleted()
    {
        if (state == State.Leaving)
        {
            state = State.Done;
            Destroy(gameObject, 0.25f);
        }
    }
}
