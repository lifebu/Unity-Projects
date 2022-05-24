using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawner : MonoBehaviour, EnemyCallback 
{
    
    public int numEnemies;

    public void enemyDead()
    {
        numEnemies--;
        if (numEnemies < 1)
        {
            Tiled2Unity.TiledMap Tilemap =  transform.parent.parent.GetComponent<Tiled2Unity.TiledMap>();
            Vector3 tileOffset = new Vector3( ((Tilemap.TileWidth / 2) * Tilemap.ExportScale),
                - ((Tilemap.TileWidth / 2) * Tilemap.ExportScale),
                0.0f);
            SoundManager.instance.Play("Secret",SoundManager.SoundType.SFX, false);
            Instantiate((GameObject)Resources.Load("Prefabs/Key"), 
                transform.position + tileOffset, Quaternion.identity, transform.parent);
            transform.gameObject.SetActive(false);
        }
    }
}
