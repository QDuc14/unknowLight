using UnityEngine;

public class guider : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;
    public GameObject container;

    private GameObject objectBInstance;

    void Awake()
    {
        Debug.Log("Guider Awake called");
    }
    void OnEnable()
    {
        Debug.Log("Guider OnEnable called");
    }
    void Start()
    {
        Debug.Log("Guider Start called");
    }
    void Update()
    {

    }
    void OnDisable()
    {
        Debug.Log("Guider OnDisable called");
    }

    public void EnableClick()
    {
        objectA.SetActive(true);
    }
    public void DisableClick()
    {
        objectA.SetActive(false);
    }
}
