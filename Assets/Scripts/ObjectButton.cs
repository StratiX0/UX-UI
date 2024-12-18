using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ObjectButton : MonoBehaviour,IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    public GameObject parent;
    public Furniture furniture;
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

    public void OnDrag(PointerEventData eventData)
    {
        parent.GetComponent<LayoutElement>().ignoreLayout = true; 
        parent.GetComponent<RectTransform>().position = Input.mousePosition;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        parent.GetComponent<LayoutElement>().ignoreLayout = false;
        parent.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

        float distance = 100000;
        GameObject closestElement = null;

        foreach (var element in furniture.gridElements)
        {
            if (element.transform.parent.gameObject == element) continue;

            if (Vector3.Distance(element.transform.position, Input.mousePosition) < distance)
            {
                distance = Vector3.Distance(element.transform.position, Input.mousePosition);
                closestElement = element;
            }
        }

        if (closestElement != null)
        {
            int closestIndex = furniture.gridElements.IndexOf(closestElement);
            int currentIndex = furniture.gridElements.IndexOf(transform.parent.gameObject);

            if (currentIndex < closestIndex)
            {
                for (int i = currentIndex; i < closestIndex; i++)
                {
                    furniture.gridElements[i] = furniture.gridElements[i + 1];
                }
            }
            else
            {
                for (int i = currentIndex; i > closestIndex; i--)
                {
                    furniture.gridElements[i] = furniture.gridElements[i - 1];
                }
            }

            furniture.gridElements[closestIndex] = transform.parent.gameObject;
        }

        foreach (Shelf shelf in furniture.transform.GetComponentsInChildren<Shelf>())
        {
            if (shelf.index == furniture.currentShelf)
            {
                shelf.GetObjects().Clear();
                for (int i = 0; i < furniture.gridElements.Count; i++)
                {
                    shelf.AddObject(furniture.gridElements[i].GetComponentInChildren<ObjectButton>().kitchenObject);
                }
            }
        }
        
        furniture.UpdateGrid(furniture.currentShelf);

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