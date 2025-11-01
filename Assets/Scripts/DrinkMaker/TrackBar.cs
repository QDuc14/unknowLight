using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackBar : MonoBehaviour
{
    private class SweetSpot
    {
        public GameObject gameObject;

        private float yesAreaPos;
        private float yesAreaNeg;

        private float edgeToMiddleDis;


        public SweetSpot(GameObject frefab, Transform transform)
        {
            gameObject = Instantiate(frefab, transform);
            setSweetSpotPosX(0.0f);

        }

        public float getAreaPos()
        {
            return yesAreaPos;
        }
        public float getAreaNeg()
        {
            return yesAreaNeg;
        }
        public float getEdgeToMiddleDis()
        {
            return edgeToMiddleDis;
        }
        public Vector2 getSweetSpotPos()
        {
            return gameObject.transform.position;
        }
        public void setSweetSpotPosX(float pos)
        {
            this.gameObject.transform.position = new Vector3(pos, 0.0f, 0.0f);

            SpriteRenderer SR_sweetSpot = gameObject.GetComponent<SpriteRenderer>();
            Bounds bound_sweetSpot = SR_sweetSpot.bounds;
            edgeToMiddleDis = bound_sweetSpot.size.x / 2;
            Vector2 sweetSpotPosition = gameObject.transform.position;
            this.yesAreaPos = sweetSpotPosition.x + edgeToMiddleDis;
            this.yesAreaNeg = sweetSpotPosition.x - edgeToMiddleDis;
        }
    }
    [SerializeField] GameObject runner;
    [SerializeField] GameObject sweetSpotFrefab;
    [SerializeField] Transform container;
    private InputAction getSpacePress;
    private List<SweetSpot> listSweetSpotSpawn = new List<SweetSpot>();
    private byte whichSweetSpotIsAct = 0;
    private bool isLastOne = false;

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
        byte recipeChosen = (byte)UnityEngine.Random.Range(3u, 5u);
        for (byte idx = 0; idx < recipeChosen; idx++)
        {
            SweetSpot sweetSpot = new SweetSpot(sweetSpotFrefab, this.transform);

            // SsP created wont be allowed to overlap each others
            float randomPos;
            bool isNotAccept;
            do
            {
                isNotAccept = false;
                randomPos = UnityEngine.Random.Range(-6f, 6f);
                foreach (SweetSpot each_sSp in listSweetSpotSpawn)
                {
                    if (Math.Abs(randomPos - each_sSp.getSweetSpotPos().x) < each_sSp.getEdgeToMiddleDis() * 2)
                    {
                        isNotAccept = true;
                        break;
                    }
                }
            } while (isNotAccept);
            
            // if found a appropriate position, add to list
            sweetSpot.setSweetSpotPosX(randomPos);
            listSweetSpotSpawn.Add(sweetSpot);
        }
        listSweetSpotSpawn.Sort((x, y) => x.getSweetSpotPos().x.CompareTo(y.getSweetSpotPos().x));

        // re-check sorting
        Debug.Log("Number of SP: "+listSweetSpotSpawn.Count);
    }

    // Update is called once per frame
    void Update()
    {
        // runner is his way
        if (runner.transform.position.x < 7.0f)
        {
            runner.transform.Translate(0.004f, 0, 0);
        }


        // consider the determined sweetspot
        if ((false == isLastOne) && (runner.transform.position.x >= listSweetSpotSpawn[whichSweetSpotIsAct].getAreaPos()))
        {
            whichSweetSpotIsAct++;
            if (whichSweetSpotIsAct == listSweetSpotSpawn.Count-1)
            {
                isLastOne = true;
            }
        }
    }

    void spacePressCallback(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //if Space pressed
            Vector2 runnerPosition = runner.transform.position;
            float yesAreaNeg = listSweetSpotSpawn[whichSweetSpotIsAct].getAreaNeg();
            float yesAreaPos = listSweetSpotSpawn[whichSweetSpotIsAct].getAreaPos();

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
