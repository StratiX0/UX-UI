using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LitMotion;

public class StoreButton : MonoBehaviour, IPointerDownHandler
{
    public static StoreButton Instance;
    public bool isSelected;
    [SerializeField] private GameObject leftHandButton;
    [SerializeField] private GameObject rightHandButton;
    [SerializeField] private TextMeshProUGUI storeFromText;
    private List<bool> handsAvailability;
    private CompositeMotionHandle compMotionHandle;
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        handsAvailability = new List<bool> {false, false};
        leftHandButton = GameObject.Find("StoreLeftHand");
        rightHandButton = GameObject.Find("StoreRightHand");
        storeFromText.GameObject().SetActive(false);
        compMotionHandle = new CompositeMotionHandle();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isSelected)
        {
            isSelected = true;
            CheckHandsAvailability();
            storeFromText.GameObject().SetActive(true);
            LMotion.Create(0f, 1f, 1f).WithLoops(-1, LoopType.Yoyo).Bind(x => storeFromText.color = new Color(255, 255, 255, x)).AddTo(compMotionHandle);

        }
        else if (isSelected)
        {
            UnSelect();
        }
    }

    private void CheckHandsAvailability()
    {
        if (Player.Instance.objectsInHands[0] == null)
        {
            handsAvailability[0] = true;
            leftHandButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            handsAvailability[0] = false;
            leftHandButton.GetComponent<Image>().color = Color.green;
        }
        
        if (Player.Instance.objectsInHands[1] == null)
        {
            handsAvailability[1] = true;
            rightHandButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            handsAvailability[1] = false;
            rightHandButton.GetComponent<Image>().color = Color.green;
        }
    }
    
    public void UnSelect()
    {
        isSelected = false;
        leftHandButton.GetComponent<Image>().color = Color.white;
        rightHandButton.GetComponent<Image>().color = Color.white;
        storeFromText.GameObject().SetActive(false);
        storeFromText.color = new Color(255, 255, 255, 1);
        compMotionHandle.Cancel();
        compMotionHandle.Clear();
    }
    
}
