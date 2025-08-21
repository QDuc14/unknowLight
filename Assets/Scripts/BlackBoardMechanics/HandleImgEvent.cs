using UnityEngine;
using UnityEngine.EventSystems;

public class HandleImgEvent : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    void Awake()
    {
        // This method is called when the script instance is being loaded
        Debug.Log("NewMonoBehaviourScript Awake called");
    }

    private void OnEnable()
    {
        Debug.Log("NewMonoBehaviourScript OnEnable called");
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("NewMonoBehaviourScript Start called");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        // This method is called when the script instance is being disabled
        Debug.Log("NewMonoBehaviourScript OnDisable called");
    }

    public void EnableClick()
    {
        // This method can be called to enable some functionality
        Debug.Log("EnableClick method called");
    }

    public void DisableClick()
    {
        // This method can be called to disable some functionality
        Debug.Log("DisableClick method called");
    }

    public void DragHandler(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData) data;

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out position
        );

        transform.position = canvas.transform.TransformPoint(position);
    }
}
