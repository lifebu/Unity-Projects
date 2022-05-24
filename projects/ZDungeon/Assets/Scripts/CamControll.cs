using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControll : MonoBehaviour 
{
    public GameObject player;
    public float margin;
    public float followSpeed;

	// Update is called once per frame
	void Update ()
    {
        Camera cam = GetComponent<Camera>();
		Vector3 playerViewPos = cam.WorldToViewportPoint(player.transform.position);
        if(playerViewPos.x > margin || playerViewPos.x < -margin
            || playerViewPos.y > margin || playerViewPos.y < -margin)
        {
            Vector3 lerpige = Vector3.Lerp(transform.position, player.transform.position, 
                Time.deltaTime * followSpeed);
            transform.position = new Vector3(lerpige.x, lerpige.y, 
                player.transform.position.z - 2.0f);
        }
	}
}
