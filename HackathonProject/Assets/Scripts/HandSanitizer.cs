using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class HandSanitizer : MonoBehaviour
{
    private float radius = 0;
    private Rigidbody rb;
    private GameObject gameObject;

	

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Character")) {
            
            //shrinks
            int i = 9;

        }
        
    }
}
