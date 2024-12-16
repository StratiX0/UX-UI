using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [SerializeField] private List<KitchenObject> kitchenObjects;
    public GameObject prefab;
    
    public void AddObject(KitchenObject kitchenObject)
    {
        kitchenObjects.Add(kitchenObject);
    }
    
    public void RemoveObject(KitchenObject kitchenObject)
    {
        kitchenObjects.Remove(kitchenObject);
    }
    
    public List<KitchenObject> GetObjects()
    {
        return kitchenObjects;
    }
}
