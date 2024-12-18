using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Objects", menuName = "Scriptable Item")]
public class KitchenObject : ScriptableObject
{
    [SerializeField] private string objectName;
    [SerializeField] private string description;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite icon;
    
    public void SetName(string value)
    {
        objectName = value;
    }
    
    public string GetName()
    {
        return objectName;
    }
    
    public void SetDescription(string value)
    {
        description = value;
    }
    
    public string GetDescription()
    {
        return description;
    }
    
    public void SetPrefab(GameObject value)
    {
        prefab = value;
    }
    
    public GameObject GetPrefab()
    {
        return prefab;
    }
    
    public void SetIcon(Sprite value)
    {
        icon = value;
    }
    
    public Sprite GetIcon()
    {
        return icon;
    }
}
