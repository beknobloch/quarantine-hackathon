using UnityEngine;
using System.Collections.Generic;

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

    [SerializeField]
    private string type;
    [SerializeField]
    private string color;

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
        //etc
    }

	void Update()
	{

        
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

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("collide!");
        if(collision.gameObject.CompareTag("Flag") && collision.gameObject.GetComponent<Flag>().getColor().Equals(color)){
            Debug.Log("1");
            gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Character"){
            Debug.Log("2");
            //Lose level
            GameObject.Find("LosePanel").SetActive(true);
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
        else
		{
            Debug.Log("5");
			moveVector = rb.velocity;
		}
    }
    }

