using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour 
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            SoundManager.instance.Play("Get_Key",SoundManager.SoundType.SFX, false);
            other.transform.GetComponent<Player>().addSmallKey();
            Destroy(transform.gameObject);
        }
        else if (other.tag == "Sword")
        {
            SoundManager.instance.Play("Get_Key",SoundManager.SoundType.SFX, false);
            other.transform.parent.GetComponent<Player>().addSmallKey();
            Destroy(transform.gameObject);
        }
    }
}
