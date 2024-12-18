using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Furniture : MonoBehaviour
{
    [System.Serializable] public class ShelvesObjects
    {
        public List<KitchenObject> objects;
    }

    public List<ShelvesObjects> shelves;
    [SerializeField] GameObject tabsParent;
    [SerializeField] GameObject tabPrefab;
    [SerializeField] GameObject gridParent;
    [SerializeField] GameObject gridElementPrefab;
    [SerializeField] GameObject descriptionPanel;

    private void GetShelves()
    {
        shelves = new List<ShelvesObjects>();
        foreach (var shelf in transform.GetComponentsInChildren<Shelf>())
        {
            ShelvesObjects shelfObjects = new ShelvesObjects
            {
                objects = shelf.GetObjects()
            };
            shelves.Add(shelfObjects);
        }
    }
    
    public void UpdateMenu()
    {
        GetShelves();
        
        foreach (Transform child in tabsParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var obj in shelves)
        {
            GameObject tab = tabPrefab;
            tab.GetComponentInChildren<TextMeshProUGUI>().text = $"Shelf {shelves.IndexOf(obj) + 1}";
            ShelfButton button = tab.GetComponent<ShelfButton>();
            button.currentStorage = gameObject;
            button.index = shelves.IndexOf(obj);
            Instantiate(tabPrefab, tabsParent.transform);
        }
        
        UpdateGrid(0);
    }
    
    public void UpdateGrid(int shelfIndex)
    {
        foreach (Transform child in gridParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        RectTransform rectTransform = gridParent.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 725);
        GridLayoutGroup gridLayoutGroup = gridParent.GetComponent<GridLayoutGroup>();
    
        float cells = gridLayoutGroup.cellSize.y;
        float spacing = gridLayoutGroup.spacing.y * 2;
        
        int count = 0;
        foreach (var obj in shelves[shelfIndex].objects)
        {
            if (count % 6 == 0 && count != 0)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + spacing);
            }
            
            GameObject gridElement = Instantiate(gridElementPrefab, gridParent.transform);
            gridElement.GetComponentInChildren<ObjectButton>().kitchenObject = obj;
            gridElement.GetComponentInChildren<ObjectButton>().descriptionPanel = descriptionPanel;
            
            count++;
        }
    }

    public void ClearUi()
    {
        descriptionPanel.transform.Find("Icon").gameObject.SetActive(false);
        descriptionPanel.transform.Find("ObjectName").GetComponent<TextMeshProUGUI>().text = "";
        descriptionPanel.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "";
    }
}