using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
    [System.Serializable]
    public enum DropTypes
    {
        rupee1,
        rupee5
    }

    [System.Serializable]
    public class EnemyDrop
    {
        public EnemyDrop(DropTypes type, int chance)
        {
            this.type = type;
            this.chance = chance;
        }

        public DropTypes type;
        public int chance;
    }


    [ReadOnly]
    [SerializeField]
    bool invincible = false;
    private float invincibilityCooldown = 0.3f;
    private float currInvincibilityTime;
    SpriteRenderer sprite;
    private float moveSpeed = 0.8f;
    public int health;
    [ReadOnly]
    [SerializeField]
    private int currhealth;
    //private float walkPathCooldown = 5.0f;
    private float currwalkPathTime;
    Vector3 walkPath;
    public GameObject explosionPrefab;
    public List<EnemyDrop> drops = new List<EnemyDrop>();
    public GameObject callback;
    public float knockbackForce = 1.0f;


	// Use this for initialization
	void Start ()
    {
		sprite = GetComponent<SpriteRenderer>();
        walkPath = Random.insideUnitCircle;
        walkPath.Normalize();
        currhealth = health;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Invincibility-Check
		if(invincible && Time.time >= currInvincibilityTime + invincibilityCooldown)
        {
            invincible = false;
            sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);      
        }

        // Walk Random
        /*
        if(Time.time >= currwalkPathTime + walkPathCooldown + Random.Range(-2.0f, 2.0f))
        {
            walkPath = Random.insideUnitCircle;
            walkPath.Normalize();
            currwalkPathTime = Time.time;
        }
        */

        transform.Translate(walkPath * Time.deltaTime * moveSpeed);
	}

    private void OnCollisionEnter2D(Collision2D other)
    {
        walkPath = Vector3.Reflect(walkPath, other.contacts[0].normal);
        walkPath.Normalize();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Sword")
        {
            if(!invincible)
            {
                invincible = true;
                sprite.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                // Knockback
                Vector3 knockback = (transform.position - other.transform.position).normalized;
                walkPath = knockback;
                walkPath.Normalize();
                GetComponent<Rigidbody2D>().AddForce(knockback * knockbackForce);
                currInvincibilityTime = Time.time;
                Debug.Log("Damage Done");
                currhealth--;
                if(currhealth < 0) currhealth = 0;
                if(currhealth > 0)
                {
                    SoundManager.instance.Play("Enemy_Hit", SoundManager.SoundType.SFX, false);
                }
                else if(currhealth <= 0)
                {
                    SoundManager.instance.Play("Enemy_Kill", SoundManager.SoundType.SFX, false);
                    Debug.Log("Enemy Dead");
                    if (callback != null)
                    {
                        callback.GetComponent<EnemyCallback>().enemyDead();
                    }
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    foreach (var drop in drops)
                    {
                        int rnd = Random.Range(1, 101);
                        Debug.Log("Random Value for Drop:" + rnd);
                        if (rnd <= drop.chance)
                        {
                            switch (drop.type)
                            {
                                case DropTypes.rupee1:
                                    {
                                        Instantiate((GameObject)Resources.Load("Prefabs/Rupee1"), 
                                            transform.position, Quaternion.identity, transform.parent);
                                    }
                                    break;
                                case DropTypes.rupee5:
                                    {
                                        Instantiate((GameObject)Resources.Load("Prefabs/Rupee5"), 
                                            transform.position, Quaternion.identity, transform.parent);
                                    }
                                    break;
                                default:
                                    {
                                        Debug.LogError("When Dropping Loot for Enemy: undefined Loot." +
                                            " Is it new?");
                                    }
                                    break;
                            }  
                        }
                    }          
                    Destroy(transform.gameObject);
                }
            }
        }
    }
}
