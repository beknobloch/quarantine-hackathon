using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{

    GameObject gamecontrol = GameObject.Find("GameControl").GetComponent<LevelGameControl>();

    private float DEF_SPEED;

    private float DEF_RADIUS;
    private float speed;
    private float radius;
    private List<GameObject> waypoints = new List<GameObject>();
    private GameObject waypointPrefab;
    private Rigidbody rb;
    private bool drawingLine = false;
    private Vector3 moveVector = Vector3.zero;

    private static int GLOBAL_TIMER= 0;

    private List<int> timerStartValue = new List<int>(){
        0,
        0,
        0
    };

    private List<bool> timerEvents = new List<bool>(){
        false,
        false,
        false
    };

    private bool finished = false;
    private int deliveryFlags;
    
    [SerializeField]
    private string type;
    [SerializeField]
    private string color;
    [SerializeField]
    private float timeBeforeSpawn;
    [SerializeField]
    private string startingDirection;
    private Vector3 startingVector;

    SpriteRenderer rend;

    private void Awake()
    {
        
        rend = GetComponent<SpriteRenderer>();
        color = "red";
        Debug.Log(rend.color);
        waypointPrefab = (GameObject)Resources.Load("Prefabs/Waypoint");
        rb = gameObject.GetComponent<Rigidbody>();


        if(type.Equals("masked")){
            DEF_SPEED = 25;
            DEF_RADIUS = 1f;
        }
        else if(type.Equals("unmasked"))
        {
            DEF_SPEED = 25;
            DEF_RADIUS = 2f;
        }
        else if(type.Equals("kid"))
        {
            DEF_SPEED = 32;
            DEF_RADIUS = 1f;
        }
        else if(type.Equals("sick"))
        {
            DEF_SPEED = 18;
            DEF_RADIUS = 2f;
        }
        else if(type.Equals("old"))
        {
            DEF_SPEED = 18;
            DEF_RADIUS = 2f;
        }
        else if(type.Equals("delivery"))
        {
            DEF_SPEED = 25;
            DEF_RADIUS = 1f;
        }
        else {
            DEF_SPEED = 32;
            DEF_RADIUS = 0;
        }

        speed = DEF_SPEED;
        radius = DEF_RADIUS;

        if(startingDirection.Equals("left"))
		{
            startingVector = Vector3.left;
		}
        else if(startingDirection.Equals("up"))
		{
            startingVector = Vector3.up;
		}
        else if(startingDirection.Equals("right"))
		{
            startingVector = Vector3.right;
		}
		else if(startingDirection.Equals("down"))
		{
            startingVector = Vector3.down;
		}
    }

	void Update()
	{
        GLOBAL_TIMER ++;
        //  Determines when to move the character into the screen.
        if(Time.timeSinceLevelLoad >= timeBeforeSpawn)
		{
            rb.velocity = startingVector;
        }

        
		//  Creates waypoints.
		if (Input.GetMouseButton(0))
		{
			RaycastHit hitInfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			bool hit = Physics.Raycast(ray, out hitInfo);

			if (!hit) return;

			if (!drawingLine && !gamecontrol.currentlyDrawing && hitInfo.collider.gameObject.Equals(gameObject))
			{
				foreach (GameObject wp in waypoints) {
					Destroy(wp);
				}
				waypoints.Clear();
				waypoints.Add(Instantiate(waypointPrefab, Helpers.Vector3To2(hitInfo.point), Quaternion.identity));
				drawingLine = true;
                gamecontrol.currentlyDrawing = true;
			}
			else if (drawingLine && gamecontrol.currentlyDrawing && waypoints.Count > 0 && Vector3.Distance(waypoints[waypoints.Count - 1].transform.position, hitInfo.point) > .5f)
			{
				waypoints.Add(Instantiate(waypointPrefab, Helpers.Vector3To2(hitInfo.point), Quaternion.identity));
			}
		}
		else if (drawingLine) {
			drawingLine = false;
            gamecontrol.currentlyDrawing = false;
		}


        checkTimer();
    }

        


        //  Moves the player.
    void FixedUpdate()
	{
        if(waypoints.Count > 0){
            moveVector = waypoints[0].transform.position - transform.position;
            if(waypoints.Count > 1 || (waypoints.Count == 1 && !drawingLine)){
                if(moveVector.magnitude < .25){
                    Destroy(waypoints[0]);
                    waypoints.RemoveAt(0);
                }
            }
        }

        rb.velocity = Vector3.zero;
        rb.AddForce(speed * moveVector.normalized, ForceMode.VelocityChange);
    }

    public string getColor(){
        return color;
    }
    public bool getFinished()
	{
        return finished;
	}
    public void stop()
	{
        speed = 0;
	}

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Flag") && color.IndexOf(collision.gameObject.GetComponent<Flag>().getColor()) != -1)
        {
            if (type.Equals("delivery") && deliveryFlags == 1 || !type.Equals("delivery"))
            {
                foreach (GameObject waypoint in waypoints)
                {
                    Destroy(waypoint);
                }
                waypoints.Clear();
                finished = true;
                gamecontrol.checkIfWon();
                gameObject.SetActive(false);

            }
            else if(type.Equals("delivery") && deliveryFlags == 0)
            {
                deliveryFlags++;
                if (color.IndexOf(collision.gameObject.GetComponent<Flag>().getColor()) == 0)
                    color = color.Substring(color.IndexOf(" ") + 1, color.Length);
                else color = color.Substring(0, color.IndexOf(" "));
            }
        }
        else if(collision.gameObject.CompareTag("Hand Sanitizer"))
        {
            //Decrease radius for set amount of time
            Debug.Log("3");
            Destroy(collision.gameObject);
            radius = radius/2; //change value as needed
            timerStartValue[0] = GLOBAL_TIMER;
            timerEvents[0] = true;
            rend.color = Color.green; 
            Debug.Log("Hi!");
        }
        else if(collision.gameObject.CompareTag("Toilet Paper"))
        {
            //increase speed for set amount of time
            Debug.Log("4");
            Destroy(collision.gameObject);
            speed = speed+5; //change value as needed
            timerStartValue[1] = GLOBAL_TIMER;
            timerEvents[1] = true;
            rend.material.color = Color.black;
        }
        else if (collision.gameObject.CompareTag("Sink"))
        {
            Debug.Log("5");
            Destroy(collision.gameObject);
            radius = radius/2; //change value as needed
            timerStartValue[2] = GLOBAL_TIMER;
            timerEvents[2] = true;
            rend.color = Color.blue;
        }
    }

    void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Character"))
		{
            //Lose level
            try
            {
                collision.gameObject.GetComponent<Character>().stop();
                stop();
			}
			catch
			{

			}


            gamecontrol.levelLost();
        }
        else{
            moveVector = rb.velocity;
        }
    }

    void checkTimer(){

        //timer conditions
        //handsanitizer
        if (timerEvents[0] && GLOBAL_TIMER - timerStartValue[0]>=600){

            Debug.Log("Yeet");
            //set back to default (ADJUST VALUES)
            radius = DEF_RADIUS;
            timerEvents[0] = false;
            rend.color = Color.red;
        }

        //toilet paper
        if (timerEvents[1] && GLOBAL_TIMER - timerStartValue[1]>=600){

            speed = speed - 5;
            rend.color = Color.red;

            
        }
        if (timerEvents[2] && GLOBAL_TIMER - timerStartValue[2]>=600)
        {
            radius = radius * 2;
            rend.color = Color.red;
            
        }
        //sneezing person
        if (type.Equals("sick"))
        {
            if (GLOBAL_TIMER - timerStartValue[3]>=300)
            {
                //some alarm
            }
            if (GLOBAL_TIMER - timerStartValue[3] >=405)
                {
                speed = 0;
            }
            else if (GLOBAL_TIMER - timerStartValue[3] >= 400)
            {
                speed = DEF_SPEED;
            }
        }
    }
}
