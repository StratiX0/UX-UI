using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreGrab : MonoBehaviour, IPointerDownHandler
{
    public static StoreGrab Instance;
    public bool isSelected;
    [SerializeField] private GameObject leftHandButton;
    [SerializeField] private GameObject rightHandButton;
    private List<bool> handsAvailability;
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        handsAvailability = new List<bool> {false, false};
        leftHandButton = GameObject.Find("StoreLeftHand");
        rightHandButton = GameObject.Find("StoreRightHand");
        isSelected = false;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isSelected)
        {
            isSelected = true;
            CheckHandsAvailability();
        }
        else if (isSelected)
        {
            UnSelect();
        }
    }

    public void CheckHandsAvailability()
    {
        if (Player.Instance.objectsInHands[0] == null)
        {
            handsAvailability[0] = true;
            leftHandButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            handsAvailability[0] = false;
            leftHandButton.GetComponent<Image>().color = Color.red;
        }
        
        if (Player.Instance.objectsInHands[1] == null)
        {
            handsAvailability[1] = true;
            rightHandButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            handsAvailability[1] = false;
            rightHandButton.GetComponent<Image>().color = Color.red;
        }
    }
    
    public void UnSelect()
    {
        isSelected = false;
        leftHandButton.GetComponent<Image>().color = Color.white;
        rightHandButton.GetComponent<Image>().color = Color.white;
    }
    
}
