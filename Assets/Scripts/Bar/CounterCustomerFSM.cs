using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(WaypointMover))]
public class CounterCustomerFSM : MonoBehaviour
{
    public enum State { GoingToSeat, WaitingForClick, ExecutingActions, Leaving, Done }
    public enum SequenceMode { OrderThenTalk, TalkThenOrder, TalkOnly, OrderOnly }

    [Header("Paths")]
    public Transform goPathParent;         // Entry -> ... -> CounterSeat
    public int seatWaypointIndex = -1;     // if -1, uses last child of goPathParent
    public Transform leavePathParent;      // optional (else auto-reverse to Entry)

    [Header("Marker & UX")]
    public GameObject marker;              // child with ClickMarker
    public string drinkName = "House Ale";
    public float showSeconds = 5f;

    [Header("Scene Jump")]
    public SequenceMode sequence = SequenceMode.OrderThenTalk;
    [Tooltip("Dialogue scene name (must be in Build Settings)")]
    public string sceneToLoad = "DialogueScene";
    public string dialogueFilePath = "";

    WaypointMover mover;
    ClickMarker click;
    State state;

    void Awake()
    {
        mover = GetComponent<WaypointMover>();
        if (marker)
        {
            click = marker.GetComponent<ClickMarker>();
            if (click == null) click = marker.AddComponent<ClickMarker>();
            marker.SetActive(false);
        }
    }

    void Start()
    {
        // Prepare go-path
        mover.mode = WaypointMover.Mode.Once;
        mover.SetPathParent(goPathParent, autoChildren: true);

        if (seatWaypointIndex < 0 && goPathParent != null)
            seatWaypointIndex = Mathf.Max(0, goPathParent.childCount - 1);

        mover.OnWaypointReached += OnReachedWaypoint;
        mover.OnPathCompleted += OnPathCompleted;

        state = State.GoingToSeat;
        mover.Play();
    }

    void OnDestroy()
    {
        if (mover != null)
        {
            mover.OnWaypointReached -= OnReachedWaypoint;
            mover.OnPathCompleted -= OnPathCompleted;
        }
    }

    void OnReachedWaypoint(int idx)
    {
        if (state == State.GoingToSeat && idx == seatWaypointIndex)
        {
            // Stop at the counter seat and wait for player click
            mover.Pause();
            state = State.WaitingForClick;

            if (marker)
            {
                marker.SetActive(true);
                click.onClick = OnMarkerClicked;
            }
        }
    }

    void OnPathCompleted()
    {
        if (state == State.Leaving)
        {
            state = State.Done;
            Destroy(gameObject, 0.25f);
        }
    }

    void OnMarkerClicked()
    {
        if (state != State.WaitingForClick) return;

        if (marker) marker.SetActive(false);   // prevent double clicks
        state = State.ExecutingActions;
        StartCoroutine(RunSequenceThenLeave());
    }

    IEnumerator RunSequenceThenLeave()
    {
        // Execute the configured sequence
        switch (sequence)
        {
            case SequenceMode.OrderOnly:
                ShowDrink();
                yield return new WaitForSeconds(showSeconds);
                break;

            case SequenceMode.TalkOnly:
                yield return OpenDialogueAndWait(sceneToLoad);
                break;

            case SequenceMode.OrderThenTalk:
                ShowDrink();
                yield return new WaitForSeconds(showSeconds);
                yield return OpenDialogueAndWait(sceneToLoad);
                break;

            case SequenceMode.TalkThenOrder:
                yield return OpenDialogueAndWait(sceneToLoad);
                ShowDrink();
                yield return new WaitForSeconds(showSeconds);
                break;
        }

        // After actions, switch to leave path and go
        Transform[] leavePoints;
        if (leavePathParent != null && leavePathParent.childCount >= 1)
        {
            leavePoints = new Transform[leavePathParent.childCount];
            for (int k = 0; k < leavePoints.Length; k++)
                leavePoints[k] = leavePathParent.GetChild(k);
        }
        else
        {
            // auto reverse seat..0 back to Entry
            int count = seatWaypointIndex + 1;
            leavePoints = new Transform[count];
            for (int k = 0; k < count; k++)
                leavePoints[k] = goPathParent.GetChild(seatWaypointIndex - k);
        }

        mover.SetWaypoints(leavePoints, restart: true);
        state = State.Leaving;
        mover.Play();
    }

    void ShowDrink()
    {
        if (DrinkUI.I != null && !string.IsNullOrEmpty(drinkName))
            DrinkUI.I.Show(drinkName, showSeconds);
    }
    
    IEnumerator OpenDialogueAndWait(string dialogueSceneName)
    {
        if (string.IsNullOrEmpty(dialogueSceneName))
            yield break;

        DialogueManager.dialogueFilePath = dialogueFilePath;
        DialogueManager.currentLineIndex = 0;
        var load = SceneManager.LoadSceneAsync(dialogueSceneName, LoadSceneMode.Additive);
        while (!load.isDone) yield return null;

        bool done = false;
        void Handler() { done = true; }
        DialogueBridge.OnDialogueClosed += Handler;
        while (!done) yield return null;
        DialogueBridge.OnDialogueClosed -= Handler;

        var unload = SceneManager.UnloadSceneAsync(dialogueSceneName);
        while (!unload.isDone) yield return null;
    }
}
