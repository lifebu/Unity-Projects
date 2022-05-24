using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoor : MonoBehaviour 
{
    public void playerInteracted(Player player)
    {
        SoundManager.instance.Play("Door_Unlock",SoundManager.SoundType.SFX, false);
        SoundManager.instance.Play("Door_Open",SoundManager.SoundType.SFX, false);
        transform.gameObject.SetActive(false);
    }
}
