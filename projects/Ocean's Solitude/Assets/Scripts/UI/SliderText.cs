using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
	}

	public void OnValueChanged()
	{
		Slider slider = GetComponent<Slider>();
		Text text = transform.Find("NumText").GetComponent<Text>();
		text.text = slider.value + " / " + slider.maxValue;
	}
}
