using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		SoundManager.instance.Play("Water Temple", SoundManager.SoundType.Music, true);
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}
