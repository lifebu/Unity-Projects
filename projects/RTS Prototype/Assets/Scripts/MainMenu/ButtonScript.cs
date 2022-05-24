using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<AudioSource>().Play();
    }
}
