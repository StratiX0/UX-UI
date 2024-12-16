using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ObjectButton : MonoBehaviour, IPointerDownHandler
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

    private void DisplayDescription()
    {
        nameText.text = kitchenObject.name;
        descriptionText.text = kitchenObject.GetDescription();
    }
    
    private void DisplayIcon()
    {
        icon = kitchenObject.GetIcon();
        transform.GetComponent<Image>().sprite = icon;
    }
}