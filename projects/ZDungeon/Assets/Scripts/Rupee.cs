using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rupee : MonoBehaviour 
{
    public int amount;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            SoundManager.instance.Play("Rupee1",SoundManager.SoundType.SFX, false);
            other.transform.GetComponent<Player>().addRupee(amount);
            Destroy(transform.gameObject);
        }
        else if (other.tag == "Sword")
        {
            SoundManager.instance.Play("Rupee1",SoundManager.SoundType.SFX, false);
            other.transform.parent.GetComponent<Player>().addRupee(amount);
            Destroy(transform.gameObject);
        }
    }
}
