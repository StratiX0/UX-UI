using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShelfButton : MonoBehaviour, IPointerDownHandler
{
    public GameObject prefab;
    
    private ShelfGroup shelfGroup;
    private Shelf shelf;
    private bool active;
    
    
    
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
    
    public void SetActive(bool value)
    {
        active = value;
    }
}