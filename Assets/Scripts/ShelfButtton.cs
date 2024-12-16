using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShelfButton : MonoBehaviour, IPointerDownHandler
{
    public GameObject prefab;
    private Shelf shelf;
    private bool isActive;
    public int index;
    public GameObject currentStorage;
    
    
    
    public void OnPointerDown(PointerEventData eventData)
    {
        currentStorage.GetComponent<Furniture>().UpdateGrid(index);
    }
    
    public void SetActive(bool value)
    {
        isActive = value;
    }
}