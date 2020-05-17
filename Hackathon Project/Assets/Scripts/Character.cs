﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
    private LevelGameControl gamecontrol;
    private float speed = 32;
    private float radius = 0;
    private List<GameObject> waypoints = new List<GameObject>();
    private GameObject waypointPrefab;
    private Rigidbody rb;
    private bool drawingLine = false;
    private Vector3 moveVector = Vector3.zero;
    private bool finished = false;

    [SerializeField]
    private string type;
    [SerializeField]
    private string color;
    [SerializeField]
    private float timeBeforeSpawn;
    [SerializeField]
    private string startingDirection;
    private Vector3 startingVector;


    private void Awake()
    {
        gamecontrol = GameObject.Find("GameControl").GetComponent<LevelGameControl>();

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
			else if (drawingLine && waypoints.Count > 0 && Vector3.Distance(waypoints[waypoints.Count - 1].transform.position, hitInfo.point) > .5f)
			{
				waypoints.Add(Instantiate(waypointPrefab, Helpers.Vector3To2(hitInfo.point), Quaternion.identity));
			}
		}
		else if (drawingLine) {
			drawingLine = false;
            gamecontrol.currentlyDrawing = false;
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
        if(collision.gameObject.CompareTag("Flag") && collision.gameObject.GetComponent<Flag>().getColor().Equals(color)){
            foreach(GameObject waypoint in waypoints)
			{
                Destroy(waypoint);
			}
            waypoints.Clear();
            finished = true;
            gamecontrol.checkIfWon();
            gameObject.SetActive(false);
        }
        else
		{
			moveVector = rb.velocity;
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
	}
}
