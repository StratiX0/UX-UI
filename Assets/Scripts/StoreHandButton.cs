using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreHandButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject gridShelf;
    public Furniture furniture;
    public int handIndex;

    private void Start()
    {
        gridShelf = GameObject.Find("GridShelf");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StoreObjectInHand();
    }
    
    private void StoreObjectInHand()
    {
        if (Player.Instance.objectsInHands[handIndex] != null && StoreButton.Instance.isSelected)
        {
            furniture.AddObjectToShelf(Player.Instance.objectsInHands[handIndex].GetComponent<StorableObject>().kitchenObject);
            Player.Instance.RemoveObjectFromHand(handIndex);
            StoreButton.Instance.UnSelect();
        }
    }
}
