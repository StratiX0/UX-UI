using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ObjectButton : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public KitchenObject kitchenObject;
    public GameObject descriptionPanel;
    private Sprite icon;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI descriptionText;
    public bool isSelected;

    private void Start()
    {
        descriptionPanel = GameObject.Find("DescriptionPanel");
        nameText = descriptionPanel.transform.Find("ObjectName").GetComponent<TextMeshProUGUI>();
        descriptionText = descriptionPanel.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        DisplayIcon();
        isSelected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DisplayDescription();
        isSelected = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponentInParent<LayoutElement>().ignoreLayout = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        GetComponentInParent<RectTransform>().position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponentInParent<LayoutElement>().ignoreLayout = false;
        Debug.Log("End Drag");
    }

    private void DisplayDescription()
    {
        nameText.text = kitchenObject.GetName();
        descriptionText.text = kitchenObject.GetDescription();
        descriptionPanel.transform.Find("Icon").gameObject.SetActive(true);
        descriptionPanel.transform.Find("Icon").GetComponent<Image>().sprite = icon;
    }

    private void DisplayIcon()
    {
        icon = kitchenObject.GetIcon();
        transform.GetComponentInChildren<Image>().sprite = icon;
    }
}