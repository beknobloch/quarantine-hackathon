using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{

    private string color;

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "character" && collision.gameObject.tag == color){
        }
    }

    public string getColor(){
        return color;
    }
}
