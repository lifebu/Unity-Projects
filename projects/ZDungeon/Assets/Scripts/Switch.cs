using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour 
{
    public GameObject target;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            SoundManager.instance.Play("Secret",SoundManager.SoundType.SFX, false);
            SoundManager.instance.Play("Switch",SoundManager.SoundType.SFX, false);
            target.SetActive(false);
            transform.gameObject.SetActive(false);
        }
    }
}
