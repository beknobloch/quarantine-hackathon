using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
    private float speed = 32;
    private float radius = 0;
    private List<GameObject> waypoints = new List<GameObject>();
    private GameObject waypointPrefab;
    private Rigidbody rb;
    private bool drawingLine = false;
    private Vector3 moveVector = Vector3.zero;

    private int hSTimer = 0;

    private int tPTimer = 0;

    private bool handSanitized = false;

    private bool toiletPapered = false;

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

        if(type == "masked"){
            speed = 25;
            radius = 1f;
        }
        else if(type == "unmasked")
        {
            speed = 25;
            radius = 2f;
        }
        else if(type == "kid")
        {
            speed = 32;
            radius = 1f;
        }
        else if(type == "sick")
        {
            speed = 18;
            radius = 2f;
        }
        else if(type == "old")
        {
            speed = 18;
            radius = 2f;
        }
        else if(type == "delivery")
        {
            speed = 25;
            radius = 1f;
        }

        if(startingDirection == "left")
		{
            startingVector = Vector3.left;
		}
        else if(startingDirection == "up")
		{
            startingVector = Vector3.up;
		}
        else if(startingDirection == "right")
		{
            startingVector = Vector3.right;
		}
		else
		{
            startingVector = Vector3.down;
		}
    }

	void Update()
	{
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

			if (!drawingLine && hitInfo.collider.gameObject.Equals(gameObject))
			{
				foreach (GameObject wp in waypoints) {
					Destroy(wp);
				}
				waypoints.Clear();
				waypoints.Add(Instantiate(waypointPrefab, Helpers.Vector3To2(hitInfo.point), Quaternion.identity));
				drawingLine = true;
			}
			else if (drawingLine && waypoints.Count > 0 && Vector3.Distance(waypoints[waypoints.Count - 1].transform.position, hitInfo.point) > .5f)
			{
				waypoints.Add(Instantiate(waypointPrefab, Helpers.Vector3To2(hitInfo.point), Quaternion.identity));
			}
		}
		else if (drawingLine) {
			drawingLine = false;
		}


        //timer conditions
        //handsanitizer
        if (handSanitized){
            hSTimer ++;

            if (hSTimer >= 600){
                Debug.Log("Yeet");
                //set back to default (ADJUST VALUES)
                if (type == "masked"){
                    radius = 1f;
                }
                else {
                    radius = 0;
                }
                 
                handSanitized = false;
                rend.color = Color.red;
            }
        }

        //toilet paper
        if (toiletPapered){
            tPTimer ++;

            if (tPTimer >= 600){

                if (type == "masked"){
                    speed = 25;
                }
                else {
                    speed = 32;
                }


                toiletPapered = false;
                rend.color = Color.red;
            }
        }
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
        if (collision.gameObject.CompareTag("Flag") && collision.gameObject.GetComponent<Flag>().getColor().Equals(color))
        {
            if (type.Equals("delivery") && deliveryFlags == 1 || !type.Equals("delivery"))
            {
                foreach (GameObject waypoint in waypoints)
                {
                    Destroy(waypoint);
                }
                waypoints.Clear();
                finished = true;
                GameObject.Find("GameControl").GetComponent<LevelGameControl>().checkIfWon();
                gameObject.SetActive(false);

            }
            else if(type.Equals("delivery") && deliveryFlags == 0)
            {
                deliveryFlags++;
            }
        }
        else if(collision.gameObject.CompareTag("Hand Sanitizer"))
        {
            //Decrease radius for set amount of time
            Debug.Log("3");
            Destroy(collision.gameObject);
            radius = 1; //change value as needed
            hSTimer = 0;
            handSanitized = true;
            rend.color = Color.green; 
            Debug.Log("Hi!");
        }
        else if(collision.gameObject.CompareTag("Toilet Paper"))
        {
            //increase speed for set amount of time
            Debug.Log("4");
            Destroy(collision.gameObject);
            speed = speed+speed; //change value as needed
            tPTimer = 0;
            toiletPapered = true;
            rend.material.color = Color.black;
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


            GameObject.Find("GameControl").GetComponent<LevelGameControl>().levelLost();
        }
        else
            moveVector = rb.velocity;
    }
}
