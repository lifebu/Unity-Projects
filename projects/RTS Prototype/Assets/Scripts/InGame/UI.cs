using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{

    private static UI _instance;

    public static UI instance
    {
        get
        {
            if(!_instance)
            {
                UI foundManager = FindObjectOfType<UI>();
                if(foundManager)
                {
                    _instance = foundManager;
                }
                else
                {
                    Debug.LogError("Error, there is no UI-Script in this Scene!");
                    Debug.Break();
                }
            }

            return _instance;
        }
    }

    public GameObject unitUI{ private set; get; }

    // Use this for initialization
    void Start()
    {
        unitUI = transform.Find("UnitUI").gameObject;
        if(!unitUI)
        {
            Debug.LogError("This UI does not have a 'UnitUI' child!");
        }
    }
	

}
