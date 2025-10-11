using UnityEngine;
using UnityEngine.InputSystem;

public class TrackBar : MonoBehaviour
{
    [SerializeField] GameObject runner;
    [SerializeField] GameObject sweetSpot;
    private InputAction getSpacePress;

    void OnEnable()
    {
        // Create an action that listens for the Space key
        getSpacePress = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
        getSpacePress.performed += spacePressCallback;
        getSpacePress.Enable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float randomValue = Random.Range(-6f, 6f);
        sweetSpot.transform.position = new Vector3(randomValue, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        runner.transform.Translate(0.01f, 0, 0);
        if (runner.transform.position.x > 7.0f)
        {
            runner.transform.position = new Vector3(-6.0f, 0, 0);
        }
    }

    void spacePressCallback(InputAction.CallbackContext ctx)
    {
        //  float y = bound_sweetSpot.size.y;
        // if Space pressed
        if (ctx.performed)
        {
            SpriteRenderer SR_sweetSpot = sweetSpot.GetComponent<SpriteRenderer>();
            Bounds bound_sweetSpot = SR_sweetSpot.bounds;
            float edgeToMiddleDis = bound_sweetSpot.size.x / 2;
            Vector2 runnerPosition = runner.transform.position;
            Vector2 sweetSpotPosition = sweetSpot.transform.position;
            float yesAreaPos = sweetSpotPosition.x + edgeToMiddleDis;
            float yesAreaNeg = sweetSpotPosition.x - edgeToMiddleDis;

            if (runnerPosition.x >= yesAreaNeg &&
                runnerPosition.x <= yesAreaPos)
            {
                Debug.Log("SCORED!!!");
            }
            else
            {
                Debug.Log("Noooob!!");
            }
        }
    }
}
