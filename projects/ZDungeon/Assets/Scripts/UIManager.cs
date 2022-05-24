    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Image heart1;
    private Image heart2;
    private Image heart3;
    private Image heart4;
    private Image heart5;

    public Sprite heartFull;
    public Sprite heartHalf;
    public Sprite heartEmpty;

    private Image keyAmount;

    public Sprite H0;
    public Sprite H1;
    public Sprite H2;
    public Sprite H3;
    public Sprite H4;
    public Sprite H5;
    public Sprite H6;
    public Sprite H7;
    public Sprite H8;
    public Sprite H9;

    private Image rupeeAmount1;
    private Image rupeeAmount2;
    private Image rupeeAmount3;


    public enum heartFullness
    {
        Full,
        Half,
        Empty
    }

	// Use this for initialization
	void Start ()
    {
        Transform life = transform.Find("Life");
        Transform hearts = life.Find("Hearts");
        heart1 = hearts.Find("Heart1").GetComponent<Image>();
        heart2 = hearts.Find("Heart2").GetComponent<Image>();
        heart3 = hearts.Find("Heart3").GetComponent<Image>();
        heart4 = hearts.Find("Heart4").GetComponent<Image>();
        heart5 = hearts.Find("Heart5").GetComponent<Image>();

        Transform keys = transform.Find("Keys");
        keyAmount = keys.Find("Number").GetComponent<Image>();

        Transform rupees = transform.Find("Rupees");
        rupeeAmount1 = rupees.Find("Number1").GetComponent<Image>();
        rupeeAmount2 = rupees.Find("Number2").GetComponent<Image>();
        rupeeAmount3 = rupees.Find("Number3").GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void setHearts(heartFullness h1,
        heartFullness h2,
        heartFullness h3,
        heartFullness h4,
        heartFullness h5)
    {
        //h1
        if(h1 == heartFullness.Full)
        {
            heart1.sprite = heartFull;
        }
        else if(h1 == heartFullness.Half)
        {
            heart1.sprite = heartHalf;
        }
        else if(h1 == heartFullness.Empty)
        {
            heart1.sprite = heartEmpty;
        }

        //h2
        if(h2 == heartFullness.Full)
        {
            heart2.sprite = heartFull;
        }
        else if(h2 == heartFullness.Half)
        {
            heart2.sprite = heartHalf;
        }
        else if(h2 == heartFullness.Empty)
        {
            heart2.sprite = heartEmpty;
        }

        //h3
        if(h3 == heartFullness.Full)
        {
            heart3.sprite = heartFull;
        }
        else if(h3 == heartFullness.Half)
        {
            heart3.sprite = heartHalf;
        }
        else if(h3 == heartFullness.Empty)
        {
            heart3.sprite = heartEmpty;
        }

        //h4
        if(h4 == heartFullness.Full)
        {
            heart4.sprite = heartFull;
        }
        else if(h4 == heartFullness.Half)
        {
            heart4.sprite = heartHalf;
        }
        else if(h4 == heartFullness.Empty)
        {
            heart4.sprite = heartEmpty;
        }

        //h5
        if(h5 == heartFullness.Full)
        {
            heart5.sprite = heartFull;
        }
        else if(h5 == heartFullness.Half)
        {
            heart5.sprite = heartHalf;
        }
        else if(h5 == heartFullness.Empty)
        {
            heart5.sprite = heartEmpty;
        }
    }

    private void setUINumber(Image imageToChange, int number)
    {
        switch (number)
        {
            case 0:
                {
                    imageToChange.sprite = H0;
                }break;
            case 1:
                {
                    imageToChange.sprite = H1;
                }break;
            case 2:
                {
                    imageToChange.sprite = H2;
                }break;
            case 3:
                {
                    imageToChange.sprite = H3;
                }break;
            case 4:
                {
                    imageToChange.sprite = H4;
                }break;
            case 5:
                {
                    imageToChange.sprite = H5;
                }break;
            case 6:
                {
                    imageToChange.sprite = H6;
                }break;
            case 7:
                {
                    imageToChange.sprite = H7;
                }break;
            case 8:
                {
                    imageToChange.sprite = H8;
                }break;
            case 9:
                {
                    imageToChange.sprite = H9;
                }break;
            default:
                {
                    Debug.LogError("Error on trying to change an Number Image: Not a number € [0,9]");
                }
                break;
        }
    }

    public void setNumKeys(int numKeys)
    {
        if (numKeys < 0 || numKeys > 9)
        {
            Debug.LogError("You tried to change the amount of keys in the UI to: " + numKeys);
        }

        setUINumber(keyAmount, numKeys);
    }

    public void setNumRupees(int numRupees)
    {
        if (numRupees < 0 || numRupees > 999)
        {
            Debug.LogError("You tried to change the amount of rupees in the UI to: " + numRupees);
        }

        int firstDigit = numRupees % 10;
        setUINumber(rupeeAmount1, firstDigit);
        int secondDigit = (numRupees / 10) % 10;
        setUINumber(rupeeAmount2, secondDigit);
        int thirdDigit = (numRupees / 100) % 10;
        setUINumber(rupeeAmount3, thirdDigit);

    }
}
