using UnityEngine;
using System.Collections;

public class SunRotator : MonoBehaviour
{
    public float DaySpeed = 1.0f;
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate( new Vector3(DaySpeed * Time.deltaTime, 0.0f, 0.0f) );

        if(transform.rotation.x >= 190.0f && transform.rotation.x <= 350.0f)
        {
            GetComponent<Light>().enabled = false;
        }
        else
        {
            GetComponent<Light>().enabled = true;
        }
	}
}
