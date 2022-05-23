using UnityEngine;
using System.Collections;

public class SoundObject : MonoBehaviour 
{
    [ReadOnly]
    public SoundManager.SoundType type;
	
	// Update is called once per frame
	void Update ()
    {
	    // As the Audio is set to be played automatically i can destroy the soundobject once it stops playing!
        if(!GetComponent<AudioSource>().isPlaying)
        {
            Destroy(gameObject);
        }
	}
}
