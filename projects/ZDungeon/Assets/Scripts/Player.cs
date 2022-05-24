using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour 
{
    public enum Facing
    {
        Up,
        Down,
        Left,
        Right
    }

    private Animator animator;
    private Transform playerLight;
    private Transform swordTrigger;
    [ReadOnly]
    [SerializeField]
    private Facing facing = Facing.Down;
    public float walkSpeed = 0.8f;
    public float runFactor = 2.0f;
    private float swordCooldown = 0.15f;
    private float currSwordTime;
    public int health;
    [ReadOnly]
    [SerializeField]
    private int currhealth;
    bool isDead = false;

	[ReadOnly]
	[SerializeField]
	private int numKeys = 0;
    [ReadOnly]
    [SerializeField]
    private int numRupees = 0;


    [ReadOnly]
    [SerializeField]
    bool invincible = false;
    private float invincibilityCooldown = 0.3f;
    private float currInvincibilityTime;
    SpriteRenderer sprite;
    Tiled2Unity.TiledMap tileMap;
    private UIManager uiManager;

    /* TODO:
     * - need to seperate Movement Logic from Facing Logic so that you can
     * face a different direction while attacking.
     */
	
	// Use this for initialization
	void Start ()
    {
		animator = GetComponent<Animator>();
        playerLight = transform.Find("Light");
        swordTrigger = transform.Find("Sword");
        swordTrigger.gameObject.SetActive(false);
        playerLight.gameObject.SetActive(true);
        currhealth = health;
        sprite = GetComponent<SpriteRenderer>();        
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        tileMap = Tiled2Unity.TiledMap.instance;
	}
        
    // Update is called once per frame
    void Update ()
    {
        if (!isDead)
        {
            // 1st: Debug: Walking through walls
            if (Application.isEditor && (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.JoystickButton3)))
            {
                transform.GetComponent<BoxCollider2D>().enabled = false;
            }
            else 
            {
                transform.GetComponent<BoxCollider2D>().enabled = true;
            }


            // 2nd: Movement and Facing Calculation
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!(animInfo.IsName("Attack_Down")
                || animInfo.IsName("Attack_Up")
                || animInfo.IsName("Attack_Left")
                || animInfo.IsName("Attack_Right")))
            {
                // End of Sword Animation so you can Move
                swordTrigger.gameObject.SetActive(false);
                Vector3 movement = new Vector3(horizontal, vertical);
                movement.Normalize();
                float actualWalkSpeed = walkSpeed;
                if(Input.GetButton("Run")) actualWalkSpeed *= runFactor;
                transform.Translate(movement * Time.deltaTime * actualWalkSpeed);
                //GetComponent<Rigidbody2D>().AddForce(movement * Time.deltaTime);


                doMovementAnim(horizontal, vertical);
                rotateLight();
                rotateSword();
            }


            // 3rd: Attacking or Interacting
            if (Input.GetButtonDown("Attack"))
            {
                bool interacted = false;

                // 2.1: Raycast slightly left of Link
                Vector2 tilePos = tileMap.unityToTile(transform.position);
                Vector3 start = tileMap.tileToUnity(tilePos + tileFacingInteractLeftOffset());
                Vector3 end = tileMap.tileToUnity(tilePos + tileFacingOffset() + tileFacingInteractLeftOffset());
                Vector3 endDir = (end - start);
                Debug.DrawLine(start, start + endDir);
                interacted = rayCastInteractable(start, endDir);

                // 2.2: Raycast slightly right of Link
                if (!interacted)
                {
                    start = tileMap.tileToUnity(tilePos - tileFacingInteractLeftOffset());
                    end = tileMap.tileToUnity(tilePos + tileFacingOffset() - tileFacingInteractLeftOffset());
                    endDir = (end - start);
                    Debug.DrawLine(start, start + endDir);
                    interacted = rayCastInteractable(start, endDir);
                }

                // 2.3 Try attacking if not interacted already
                if (!interacted && Time.time >= currSwordTime + swordCooldown)
                {
                    SoundManager.instance.Play("Sword", SoundManager.SoundType.SFX, false);
                    animator.SetBool("Attack", true);
                    currSwordTime = Time.time;
                    swordTrigger.gameObject.SetActive(true);
                }
            }
            else
            {
                animator.SetBool("Attack", false);
            }

            // 4th: Invincibility-Check
            if (invincible && Time.time >= currInvincibilityTime + invincibilityCooldown)
            {
                invincible = false;
                sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.transform.tag == "Enemy")
        {
            if(!invincible)
            {
                invincible = true;
                currInvincibilityTime = Time.time;
                Debug.Log("Damage Done");
                currhealth--;
                if(currhealth < 0) currhealth = 0;
                updateHearts();
                if(currhealth > 0)
                {
                    SoundManager.instance.Play("Link_Hurt", SoundManager.SoundType.SFX, false);
                    sprite.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                }
                else if(currhealth <= 0)
                {
                    SoundManager.instance.Play("Link_Dying", SoundManager.SoundType.SFX, false);
                    isDead = true;
                    animator.SetBool("isDead", true);
                    playerLight.gameObject.SetActive(false);
                    SoundManager.instance.StopAllSoundsOfType(SoundManager.SoundType.Music);
                    SoundManager.instance.Play("Game_Over", SoundManager.SoundType.Music, true);
                }
            }
        }
    }
      
    // Helper Functions for Update()
    private void doMovementAnim(float horizontal, float vertical)
    {
        // Reset all bools:
        animator.SetBool("Idle", true);
        animator.SetBool("Walk_Left", false);
        animator.SetBool("Walk_Right", false);
        animator.SetBool("Walk_Up", false);
        animator.SetBool("Walk_Down", false);

        // Deadzone TODO: do i need that???
        if(horizontal > 0.1f || horizontal < -0.1f
            || vertical > 0.1f || vertical < -0.1f)
        {
            if(Math.Abs(horizontal) > Math.Abs(vertical))
            {
                // Horizontal is Stronger
                if(horizontal < 0.0f)
                {
                    // Walking
                    animator.SetBool("Idle", false);
                    animator.SetBool("Walk_Left", true);

                    //Facing
                    animator.SetBool("Face_Left", true);
                    animator.SetBool("Face_Right", false);
                    animator.SetBool("Face_Up", false);
                    animator.SetBool("Face_Down", false);
                    facing = Facing.Left;
                }
                else if(horizontal > 0.0f)
                {
                    // Walking
                    animator.SetBool("Idle", false);
                    animator.SetBool("Walk_Right", true);

                    //Facing
                    animator.SetBool("Face_Left", false);
                    animator.SetBool("Face_Right", true);
                    animator.SetBool("Face_Up", false);
                    animator.SetBool("Face_Down", false);
                    facing = Facing.Right;
                }
            }
            else
            {
                // Vertical is Stronger or Same
                if(vertical < 0.0f)
                {
                    // Walking
                    animator.SetBool("Idle", false);
                    animator.SetBool("Walk_Down", true);

                    //Facing
                    animator.SetBool("Face_Left", false);
                    animator.SetBool("Face_Right", false);
                    animator.SetBool("Face_Up", false);
                    animator.SetBool("Face_Down", true);
                    facing = Facing.Down;
                }
                else if(vertical > 0.0f)
                {
                    // Walking
                    animator.SetBool("Idle", false);
                    animator.SetBool("Walk_Up", true);

                    //Facing
                    animator.SetBool("Face_Left", false);
                    animator.SetBool("Face_Right", false);
                    animator.SetBool("Face_Up", true);
                    animator.SetBool("Face_Down", false);
                    facing = Facing.Up;
                }
            }                 
        }             
    }
    private void rotateLight()
    {
        switch(facing)
        {
            case Facing.Up:
                {
                    playerLight.transform.eulerAngles = new Vector3(
                        270.0f, 90.0f, 90.0f);
                }break;
            case Facing.Down:
                {
                    playerLight.transform.eulerAngles = new Vector3(
                        90.0f, 90.0f, 90.0f);
                }break;
            case Facing.Left:
                {
                    playerLight.transform.eulerAngles = new Vector3(
                        180.0f, 90.0f, 90.0f);
                }break;
            case Facing.Right:
                {
                    playerLight.transform.eulerAngles = new Vector3(
                        0.0f, 90.0f, 90.0f);
                }break;
        }
    }
    private void rotateSword()
    {
        switch(facing)
        {
            case Facing.Up:
                {
                    swordTrigger.transform.eulerAngles = new Vector3(
                        0.0f, 0.0f, 180.0f);
                }break;
            case Facing.Down:
                {
                    swordTrigger.transform.eulerAngles = new Vector3(
                        0.0f, 0.0f, 0.0f);
                }break;
            case Facing.Left:
                {
                    swordTrigger.transform.eulerAngles = new Vector3(
                        0.0f, 0.0f, 270.0f);
                }break;
            case Facing.Right:
                {
                    swordTrigger.transform.eulerAngles = new Vector3(
                        0.0f, 0.0f, 90.0f);
                }break;
        }
    }
    private void updateHearts()
    {
        UIManager.heartFullness h1 = UIManager.heartFullness.Empty;
        if(currhealth >= 1) h1 = UIManager.heartFullness.Half;
        if(currhealth >= 2) h1 = UIManager.heartFullness.Full;

        UIManager.heartFullness h2 = UIManager.heartFullness.Empty;
        if(currhealth >= 3) h2 = UIManager.heartFullness.Half;
        if(currhealth >= 4) h2 = UIManager.heartFullness.Full;

        UIManager.heartFullness h3 = UIManager.heartFullness.Empty;
        if(currhealth >= 5) h3 = UIManager.heartFullness.Half;
        if(currhealth >= 6) h3 = UIManager.heartFullness.Full;

        UIManager.heartFullness h4 = UIManager.heartFullness.Empty;
        if(currhealth >= 7) h4 = UIManager.heartFullness.Half;
        if(currhealth >= 8) h4 = UIManager.heartFullness.Full;

        UIManager.heartFullness h5 = UIManager.heartFullness.Empty;
        if(currhealth >= 9) h5 = UIManager.heartFullness.Half;
        if(currhealth >= 10) h5 = UIManager.heartFullness.Full;

        uiManager.setHearts(h1, h2, h3, h4, h5);
    }
	private Vector2 tileFacingOffset()
	{
		Vector2 returnVal = new Vector2();
		switch(facing)
		{
		case Facing.Up:
			{
				returnVal = new Vector2(0.0f, -0.5f);
			}break;
		case Facing.Down:
			{
				returnVal = new Vector2(0.0f, 0.5f);
			}break;
		case Facing.Left:
			{
				returnVal = new Vector2(-0.5f, 0.0f);
			}break;
		case Facing.Right:
			{
				returnVal = new Vector2(0.5f, 0.0f);
			}break;
		}
		return returnVal;
	}
    private Vector2 tileFacingInteractLeftOffset()
    {
        Vector2 returnVal = new Vector2();
        switch(facing)
        {
            case Facing.Up:
                {
                    returnVal = new Vector2(-0.5f, 0.0f);
                }break;
            case Facing.Down:
                {
                    returnVal = new Vector2(0.5f, 0.0f);
                }break;
            case Facing.Left:
                {
                    returnVal = new Vector2(0.0f, 0.5f);
                }break;
            case Facing.Right:
                {
                    returnVal = new Vector2(0.0f, -0.5f);
                }break;
        }
        return returnVal;
    }
    private bool rayCastInteractable(Vector3 start, Vector3 endDir)
    {
        bool interacted = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, endDir, 
            tileMap.tileToUnity(tileFacingOffset()).magnitude);
        foreach (var hit in hits)
        {
            if (hit.transform.name.Contains("chest:"))
            {
                Debug.Log("Open Chest");
                interacted = true;
                if (hit.transform.GetComponent<Chest>().loot == Chest.Loot.SmallKey)
                {
                    numKeys++;
                    uiManager.setNumKeys(numKeys);
                }
                hit.transform.GetComponent<Chest>().playerInteracted(this);
            }
            if (hit.transform.name.Contains("door:"))
            {
                if (hit.transform.GetComponent<KeyDoor>() != null)
                {
                    Debug.Log("Try to use Key on Door");
                    interacted = true;
                    if (numKeys > 0)
                    {
                        numKeys--;
                        uiManager.setNumKeys(numKeys);
                        hit.transform.GetComponent<KeyDoor>().playerInteracted(this);
                    }
                }
            }
        }
        return interacted;
    }
    public void addRupee(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("Playerscript: You tried to add a non-positive amount of Rupees: " + amount);
            return;
        }
        numRupees += amount;
        uiManager.setNumRupees(numRupees);
    }
    public void addSmallKey()
    {
        numKeys++;
        uiManager.setNumKeys(numKeys);
    }
		
}
