using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator anim;
	// Use this for initialization
	void Start ()
    {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            Destroy(transform.gameObject);
        }
	}
}
