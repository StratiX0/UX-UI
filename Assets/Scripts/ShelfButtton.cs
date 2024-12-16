using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShelfButton : MonoBehaviour, IPointerDownHandler
{
    public GameObject prefab;
    private Shelf shelf;
    private bool isActive;
    
    
    
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
    
    public void SetActive(bool value)
    {
        isActive = value;
    }
}