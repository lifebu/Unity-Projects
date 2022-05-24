using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUp : MonoBehaviour 
{
    public GameObject StairTo;
    public GameObject StairFrom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            other.transform.position = other.transform.position + new Vector3(0.0f, 0.0f, -4.0f);
            SoundManager.instance.Play("Stairs_Up", SoundManager.SoundType.SFX, false);
            StairTo.SetActive(true);
            StairFrom.SetActive(false);
        }
    }
}
