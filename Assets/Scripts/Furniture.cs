using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Furniture : MonoBehaviour
{
    [System.Serializable] public class ShelvesObjects
    {
        public List<KitchenObject> objects;
    }

    public List<ShelvesObjects> shelves;
    [SerializeField] private GameObject tabsParent;
    [SerializeField] private GameObject tabPrefab;
    [SerializeField] private GameObject gridParent;
    [SerializeField] private GameObject gridElementPrefab;
    [SerializeField] private GameObject descriptionPanel;
    public List<GameObject> gridElements;
    public int currentShelf;
    public bool isMagic;
    
    [SerializeField] private int shelfRangeMin;
    [SerializeField] private int shelfRangeMax;
    [SerializeField] private int objNbrRangeMin;
    [SerializeField] private int objNbrRangeMax;
    [SerializeField] private KitchenObjectList kitchenObjectList;

    private void Start()
    {
        currentShelf = 0;
    }

    public void CreateShelves()
    {
        foreach (var shelf in transform.GetComponentsInChildren<Shelf>())
        {
            Destroy(shelf);
        }
        
        float randomShelfNbr = Random.Range(shelfRangeMin, shelfRangeMax);
        
        for (int i = 0; i < randomShelfNbr; i++)
        {
            Shelf shelf = gameObject.AddComponent<Shelf>();
            shelf.kitchenObjectList = kitchenObjectList;
            shelf.AddRandomObjects(objNbrRangeMin, objNbrRangeMax);
        }
    }

    private void GetShelves()
    {
        shelves = new List<ShelvesObjects>();
        
        int index = 0;
        foreach (var shelf in transform.GetComponentsInChildren<Shelf>())
        {
            shelf.index = index;
            ShelvesObjects shelfObjects = new ShelvesObjects
            {
                objects = shelf.GetObjects()
            };
            shelves.Add(shelfObjects);
            index++;
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
        
        GameObject.Find("StoreLeftHand").GetComponent<StoreHandButton>().furniture = this;
        GameObject.Find("StoreRightHand").GetComponent<StoreHandButton>().furniture = this;
        
        UpdateGrid(0);
    }
    
    public void UpdateGrid(int shelfIndex)
    {
        gridElements = new List<GameObject>();
        
        foreach (Transform child in gridParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        RectTransform rectTransform = gridParent.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 725);
        GridLayoutGroup gridLayoutGroup = gridParent.GetComponent<GridLayoutGroup>();
    
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
            gridElement.GetComponentInChildren<ObjectButton>().furniture = this;
            gridElements.Add(gridElement);
            
            count++;
        }
    }

    public void ClearUi()
    {
        descriptionPanel.transform.Find("Icon").gameObject.SetActive(false);
        descriptionPanel.transform.Find("ObjectName").GetComponent<TextMeshProUGUI>().text = "";
        descriptionPanel.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "";
    }
    
    public void AddObjectToShelf(KitchenObject kitchenObject)
    {
        foreach (Shelf shelf in transform.GetComponentsInChildren<Shelf>())
        {
            if (shelf.index == currentShelf)
            {
                shelf.AddObject(kitchenObject);
            }
        }
        
        UpdateMenu();
    }
}