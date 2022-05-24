using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, ISelectHandler,
    IPointerExitHandler
{

    public EventTrigger.TriggerEvent pointerEnterCallBack;
    public EventTrigger.TriggerEvent pointerExitCallBack;

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerEnterCallBack.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerExitCallBack.Invoke(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("You selected!");
    }

}
