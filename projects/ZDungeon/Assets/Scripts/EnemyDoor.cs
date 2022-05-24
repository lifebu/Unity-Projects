using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoor : MonoBehaviour, EnemyCallback
{
    public int numEnemies;

    public void enemyDead()
    {
        numEnemies--;
        if (numEnemies < 1)
        {
            SoundManager.instance.Play("Door_Unlock",SoundManager.SoundType.SFX, false);
            SoundManager.instance.Play("Door_Open",SoundManager.SoundType.SFX, false);
            SoundManager.instance.Play("Secret",SoundManager.SoundType.SFX, false);
            transform.gameObject.SetActive(false);
        }
    }
}
