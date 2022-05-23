using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
	[Header("Basics:")]
	public float moveSpeed = 20.0f;

	public float sprintFactor = 2.0f;

	public float animDeadzone = 0.1f;
	private Animator animator;
	private bool facingRight = true;

	private string bounceSFX;
	public float bounceSoundrate = 0.5f;
	private float nextBouncesound = 0.0f;

	[Header("Stats:")]
	public int maxHealth = 3;
	private int _currHealth;
	public int currHealth
	{
		get {return _currHealth;}
		set { UIManager.instance.SupervisedValueChanged(value, "Player.currHealth", this); _currHealth = value;}
	}

	public int maxStamina = 100;
	private int _currStamina;
	public int currStamina
	{
		get {return _currStamina;}
		set { UIManager.instance.SupervisedValueChanged(value, "Player.currStamina", this); _currStamina = value;}
	}
	public float staminaRegenRate = 1.0f;
	private float currStaminaRegenCooldown;

	public Ability dash;

	[Header("Dash:")]
	[Tooltip("In Seconds")]
	public float dashCooldown = 4.0f;
	public float _currDashCooldown;
	public float currDashCooldown
	{
		get {return _currDashCooldown;}
		set { UIManager.instance.SupervisedValueChanged(value, "Player.currDashCooldown", this); _currDashCooldown = value;}
	}
	public int dashStaminaUsage = 10;
	public float dashForce = 100.0f;

	[Header("InkCloud:")]	
	public float inkCloudCoolDown = 4.0f;
	public float _currInkCloudCooldown;
	public float currInkCloudCooldown
	{
		get {return _currInkCloudCooldown;}
		set { UIManager.instance.SupervisedValueChanged(value, "Player.currInkCloudCooldown", this); _currInkCloudCooldown = value;}
	}
	public int inkCloudStaminaUsage = 10;

	private GameObject inkCloudPrefab;

	[Header("InkBomb:")]
	public float inkBombCoolDown = 4.0f;
	public float _currInkBombCooldown;
	public float currInkBombCooldown
	{
		get {return _currInkBombCooldown;}
		set { UIManager.instance.SupervisedValueChanged(value, "Player.currInkBombCooldown", this); _currInkBombCooldown = value;}
	}
	public int inkBombStaminaUsage = 10;

	private GameObject inkBombPrefab;

	// Use this for initialization
	void Start ()
	{
		currHealth = maxHealth;
		currStamina = maxStamina;
		currDashCooldown = 0.0f;
		currStaminaRegenCooldown = 0.0f;
		currInkCloudCooldown = 0.0f;

		dash.abilityName = "Dash";

		animator = gameObject.GetComponent<Animator> ();
		if (animator == null)
		{
			Debug.LogError ("Couldn't find an Animator!!");
		}

		bounceSFX = "bounce";

		inkCloudPrefab = (GameObject)Resources.Load("Prefabs/InkCloud");
		inkBombPrefab = (GameObject)Resources.Load("Prefabs/InkBomb");
	}
	
	void FixedUpdate ()
	{
		float horizontal = 0.0f;
		float vertical = 0.0f;
		Rigidbody2D rigid = GetComponent<Rigidbody2D>();
		Vector2 moveVec;

		
		// 1st: Gather Input:
		horizontal = Input.GetAxis ("Horizontal");
		vertical = Input.GetAxis ("Vertical");

		// 2nd: Move Player
		moveVec = new Vector2 (horizontal, vertical);
		moveVec.Normalize();
		moveVec *= (moveSpeed * Time.fixedDeltaTime);


		rigid.AddForce (moveVec);

		// Make sure that this deadZone is not negative!
		if (animDeadzone < 0)
		{
			animDeadzone *= -1;
		}
		
		
		// 3rd: Animate the Movement:
		if (horizontal > animDeadzone || horizontal < -animDeadzone ||
			vertical > animDeadzone || vertical < -animDeadzone)
		{
			animator.SetBool ("Moving", true);
		}
		else
		{
			animator.SetBool ("Moving", false);
		}

		// 4th: Flipping
		if (horizontal > 0 && !facingRight)
		{
			Flip ();
		}
		else if (horizontal < 0 && facingRight)
		{
			Flip ();
		}

		// 4th: Dash Ability
		/*
		if (Input.GetButtonDown("Dash") && dash.IsUsable(currStamina, true))
		{
			currStamina -= dash.staminaUsage;
			dash.UseCooldown();
			dash.Dash(moveVec, rigid, facingRight);
			SoundManager.instance.Play("charge", SoundManager.SoundType.SFX, false);
		}*/


		
		if (Input.GetButtonDown("Dash") && currDashCooldown == 0.0f && currStamina >= dashStaminaUsage)
		{
			SoundManager.instance.Play("charge", SoundManager.SoundType.SFX, false);
			
			currDashCooldown = dashCooldown;
			currStamina -= dashStaminaUsage;

			if (moveVec.magnitude != 0.0f)
			{
				rigid.AddForce(moveVec.normalized * dashForce * Time.fixedDeltaTime);
			}
			else
			{
				Vector2 faceVector = new Vector2();
				if (facingRight)
				{
					faceVector = new Vector2(1.0f, 0.0f);
				}
				else
				{
					faceVector = new Vector2(-1.0f, 0.0f);
				}

				rigid.AddForce(faceVector.normalized * dashForce * Time.fixedDeltaTime);
			}
			
		}
		if (currDashCooldown > 0.0f)
		{
			currDashCooldown -= Time.fixedDeltaTime;
			if (currDashCooldown <= 0.0f) { currDashCooldown = 0.0f; }
		}
		
		

	}
	
	void Update ()
	{
		// 1st: Call the Update Function of the Abilities:
		dash.Update();

		// 1st: Stamina Regen
		if ((currStamina < maxStamina) && currStaminaRegenCooldown == 0.0f)
		{
			currStamina++;
			currStaminaRegenCooldown = staminaRegenRate;
		}

		if (currStaminaRegenCooldown > 0.0f)
		{
			currStaminaRegenCooldown -= Time.fixedDeltaTime;
			if (currStaminaRegenCooldown <= 0.0f) { currStaminaRegenCooldown = 0.0f; }
		}

		// 2nd: Ink Cloud Ability
		if (Input.GetButtonDown("InkCloud") && currInkCloudCooldown == 0.0f && currStamina >= inkCloudStaminaUsage)
		{
			currInkCloudCooldown = inkCloudCoolDown;
			Instantiate(inkCloudPrefab, transform.position, transform.rotation);
			currStamina -= inkCloudStaminaUsage;
		}
		if (currInkCloudCooldown > 0.0f)
		{
			currInkCloudCooldown -= Time.fixedDeltaTime;
			if (currInkCloudCooldown <= 0.0f) { currInkCloudCooldown = 0.0f; }
		}

		// 2nd: Ink Bomb Ability
		if (Input.GetButtonDown("InkBomb") && currInkBombCooldown == 0.0f && currStamina >= inkBombStaminaUsage)
		{
			currInkBombCooldown = inkBombCoolDown;
			Instantiate(inkBombPrefab, transform.position, transform.rotation);
			currStamina -= inkBombStaminaUsage;
		}
		if (currInkBombCooldown > 0.0f)
		{
			currInkBombCooldown -= Time.fixedDeltaTime;
			if (currInkBombCooldown <= 0.0f) { currInkBombCooldown = 0.0f; }
		}
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		if (Time.time > nextBouncesound)
		{
			nextBouncesound = Time.time + bounceSoundrate;
			SoundManager.instance.Play(bounceSFX, SoundManager.SoundType.SFX, false);
		}
	}

	void Flip ()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
