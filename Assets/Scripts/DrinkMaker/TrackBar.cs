using UnityEngine;

public class TrackBar : MonoBehaviour
{
    [SerializeField] GameObject runner;
    [SerializeField] GameObject sweetSpot;
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
}
