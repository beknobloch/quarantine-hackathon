using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
    private float speed = 32;
    private List<GameObject> waypoints = new List<GameObject>();
    private GameObject waypointPrefab;
    private Rigidbody rb;
    private bool drawingLine = false;


    private void Awake()
    {
        waypointPrefab = (GameObject)Resources.Load("Prefabs/Waypoint");
        rb = gameObject.GetComponent<Rigidbody>();
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

			if (!drawingLine && hitInfo.collider.tag.Equals("Character"))
            {
                foreach(GameObject wp in waypoints){
                    Destroy(wp);
                }
                waypoints.Clear();
                waypoints.Add(Instantiate(waypointPrefab, Helpers.Vector3To2(hitInfo.point), Quaternion.identity));
                drawingLine = true;
            }
            else if (drawingLine && waypoints.Count != 0 && Vector3.Distance(waypoints[waypoints.Count - 1].transform.position, hitInfo.point) > .25f)
            {
                waypoints.Add(Instantiate(waypointPrefab, Helpers.Vector3To2(hitInfo.point), Quaternion.identity));
            }
        }
        else if(drawingLine){
            drawingLine = false;
        }

        //  Moves the player.
        Vector3 direction = rb.velocity.normalized;
        while (waypoints.Count > 0 && !Input.GetMouseButton(0) || waypoints.Count > 1)
        {
            direction = waypoints[0].transform.position - transform.position;
			print(direction.magnitude);
            if (direction.magnitude > .5)
            {
                direction.Normalize();
                break;
            }
            Destroy(waypoints[0]);
            waypoints.RemoveAt(0);
        }

        rb.AddForce(speed * direction - rb.velocity, ForceMode.VelocityChange);
    }
}
