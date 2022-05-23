using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour 
{

	private Slider healthSlider;
	private Slider staminaSlider;
	private Text dashText;
	private Text inkCloudText;
	private Text inkBombText;

	private static UIManager _instance = null;
	public static UIManager instance
	{
		get
		{
			if (!_instance)
			{
				UIManager foundInstance = FindObjectOfType<UIManager>();
				if (foundInstance)
				{
					_instance = foundInstance;
				}
				else
				{
					Debug.LogError("Error, there is no UiManager in this Scene!");
					Debug.Break();
				}
			}
			
			return _instance;
		}
	}

	// Use this for initialization
	void Start () 
	{
		healthSlider = transform.Find("Health").Find("Slider").GetComponent<Slider>();
		staminaSlider = transform.Find("Stamina").Find("Slider").GetComponent<Slider>();
		dashText = transform.Find("Dash").GetComponent<Text>();
		inkCloudText = transform.Find("InkCloud").GetComponent<Text>();
		inkBombText = transform.Find("InkBomb").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void SupervisedValueChanged(object value, string name, object caller)
	{
		switch (name)
		{
			case "Player.currHealth":
			{
				healthSlider.value = (int)value;
			}break;
			case "Player.currStamina":
			{
				staminaSlider.value = (int)value;
			}break;
			case "Player.currDashCooldown":
			{
				if ((float)value == 0.0f) {dashText.text = "Dash(" + ((Player)caller).dashStaminaUsage 
														+ ", CD:" + ((Player)caller).dashCooldown + "): Ready"; break;}

				dashText.text = "Dash(" + ((Player)caller).dashStaminaUsage  + ", CD:" + ((Player)caller).dashCooldown 
																	+  "): " + ((float)value).ToString("F1");
			}break;
			case "Player.currInkCloudCooldown":
			{
				if ((float)value == 0.0f) {inkCloudText.text = "InkCloud(" + ((Player)caller).inkCloudStaminaUsage 
															+ ", CD:" + ((Player)caller).inkCloudCoolDown + "): Ready"; break;}

				inkCloudText.text = "InkCloud(" + ((Player)caller).inkCloudStaminaUsage 
											+ ", CD:" + ((Player)caller).inkCloudCoolDown +  "): " + ((float)value).ToString("F1");
			}break;
			case "Player.currInkBombCooldown":
			{
				if ((float)value == 0.0f) {inkBombText.text = "InkBomb(" + ((Player)caller).inkBombStaminaUsage 
															+ ", CD:" + ((Player)caller).inkBombCoolDown + "): Ready"; break;}

				inkBombText.text = "InkBomb(" + ((Player)caller).inkBombStaminaUsage 
											+ ", CD:" + ((Player)caller).inkBombCoolDown +  "): " + ((float)value).ToString("F1");
			}break;
			default:
			{
				Debug.LogError("Unknown Supervised Value for the uiManager: " + name);
				Debug.Break();
			}break;
		}
	}
}
