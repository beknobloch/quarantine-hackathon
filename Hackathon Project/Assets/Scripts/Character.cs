using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    private float speed = 1;
    private List<GameObject> waypoints = new List<GameObject>();
    private GameObject waypointPrefab;

    private void Awake()
    {
        waypointPrefab = (GameObject)Resources.Load("Prefabs/Waypoint");
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			bool hit = Physics.Raycast(ray, out hitInfo);

			if (!hit) return;

			if (hitInfo.collider.tag.Equals("Character"))
            {
                waypoints.Add(Instantiate(waypointPrefab, hitInfo.point, Quaternion.identity));
            }
            else if (waypoints.Count != 0 && Vector3.Distance(waypoints[waypoints.Count - 1].transform.position, hitInfo.point) > .25f)
            {
                waypoints.Add(Instantiate(waypointPrefab, hitInfo.point, Quaternion.identity));
            }
        }
    }
}
