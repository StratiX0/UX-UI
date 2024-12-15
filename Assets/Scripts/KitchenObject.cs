using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : ScriptableObject
{
    public string Description { get; set; }

    public GameObject Prefab { get; set; }

    public Sprite Icon { get; set; }
}
