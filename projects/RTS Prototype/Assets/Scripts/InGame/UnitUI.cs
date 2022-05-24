using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    private bool visibility;
    private Slider healthSlider;
    private Text healthText;
    private Text UnitStateText;
    // What do we need to handle here?
    /*
    - changing the Unit Icon from "Unselected" to "Selected" (later)
    - The UnitUI System should only exist with a specific Units Lifetime (or pooling???)
    */

	// Use this for initialization
	void Start ()
    {
	    visibility = false;

        transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);

        // may create an null exception!
        healthSlider = transform.Find("UnitHealth").GetComponent<Slider>();
        healthText = transform.Find("Healthtext").GetComponent<Text>();
        UnitStateText = transform.Find("UnitState").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void updatePosition(Vector3 unitPos)
    {
        ((RectTransform)transform).anchoredPosition3D = transform.GetComponentInParent<Canvas>().WorldToCanvasPoint(unitPos);
    }

    public void changeVisibility(bool newVisibility)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(newVisibility);
            visibility = newVisibility;
        }
    }

    public bool isVisible()
    {
        return visibility;
    }

    public void updateLifebar(float currHealth, float maxHealth)
    {
        healthSlider.value = currHealth / maxHealth; 
        healthText.text = currHealth + "/" + maxHealth;
    }

    public void setUnitStateText(string newText)
    {
        UnitStateText.text = newText;
    }

}
