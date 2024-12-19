using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Objects", menuName = "Scriptable List")]
public class KitchenObjectList : ScriptableObject
{
    public List<KitchenObject> kitchenObjectsList;
}