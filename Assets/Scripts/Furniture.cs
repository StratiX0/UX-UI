using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        
        foreach (var obj in shelves[shelfIndex].objects)
        {
            GameObject gridElement = gridElementPrefab;
            gridElement.GetComponent<ObjectButton>().kitchenObject = obj;
            gridElement.GetComponent<ObjectButton>().descriptionPanel = descriptionPanel;
            
            Instantiate(gridElementPrefab, gridParent.transform);
        }
    }
}