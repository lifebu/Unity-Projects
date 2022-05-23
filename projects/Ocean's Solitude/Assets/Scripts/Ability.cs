using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability : System.Object 
{
	[HideInInspector]
	public string abilityName;
	public float cooldown;
	public float _curCooldown;
	[HideInInspector]
	public float currCooldown
	{
		get {return _curCooldown;}
		set { UIManager.instance.SupervisedValueChanged(value, "Ability.currCooldown" + abilityName, this); _curCooldown = value;}
	}
	public int staminaUsage;

	[Header("Specific:")]
	public float force;

	public void Update()
	{
		if (currCooldown > 0.0f)
		{
			currCooldown -= Time.fixedDeltaTime;
			if (currCooldown <= 0.0f) { currCooldown = 0.0f; }
		}
	}

	public bool HasCoolDown()
	{
		if(currCooldown > 0.0f)
		{
			return true;
		}	
		return false;
	}

	public bool IsUsable(int currStamina, bool usesCoolDown)
	{
		bool returnVal = true;
		if(usesCoolDown && HasCoolDown()) returnVal = false;
		if(currStamina < staminaUsage) returnVal = false;
		
		return returnVal;
	}

	public void UseCooldown()
	{
		currCooldown = cooldown;
	}

	public void Dash(Vector2 moveVec, Rigidbody2D rigid, bool facingRight)
	{
		if (moveVec.magnitude != 0.0f)
			{
				rigid.AddForce(moveVec.normalized * force * Time.fixedDeltaTime);
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

				rigid.AddForce(faceVector.normalized * force * Time.fixedDeltaTime);
			}
	}

	public void InkCloudBomb(Player player, GameObject spawnPrefab)
	{
		if (spawnPrefab != null)  MonoBehaviour.Instantiate(spawnPrefab, player.transform.position, player.transform.rotation);
	}
}
