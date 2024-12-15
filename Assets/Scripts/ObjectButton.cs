using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectButton : MonoBehaviour, IPointerDownHandler
{
    public KitchenObject kitchenObject;

    public void OnPointerDown(PointerEventData eventData)
    {
        DisplayDescription();
    }

    private void DisplayDescription()
    {
        Debug.Log(kitchenObject.Description);
    }
}