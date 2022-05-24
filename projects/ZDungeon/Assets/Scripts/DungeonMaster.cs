using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMaster : MonoBehaviour 
{
	// Use this for initialization
	void Start ()
    {
		SoundManager.instance.Play("Eastern Palace",SoundManager.SoundType.Music, true);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        Application.Quit();
	    }
	}
}
