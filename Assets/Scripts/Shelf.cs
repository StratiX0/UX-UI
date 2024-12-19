using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public List<KitchenObject> objectsToCreate;
    [SerializeField] private List<KitchenObject> kitchenObjects;
    public KitchenObjectList kitchenObjectList;
    public int index;
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

    public void AddRandomObjects(int min, int max)
    {
        int randomObjectNbr = Random.Range(min, max);
        kitchenObjects = new List<KitchenObject>();

        for (int j = 0; j < randomObjectNbr; j++)
        {
            int randomObjectIndex = Random.Range(0, kitchenObjectList.kitchenObjectsList.Count);
            AddObject(kitchenObjectList.kitchenObjectsList[randomObjectIndex]);
        }
    }
}