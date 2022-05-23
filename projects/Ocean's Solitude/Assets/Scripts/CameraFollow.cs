using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{	
	public Transform target;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 currentPos;
		currentPos.x = target.position.x;
		currentPos.y = target.position.y;
		currentPos.z = transform.position.z;
		transform.position = currentPos;
	}
}
