using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Container : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private List<GameObject> objectsInContainer;
    private Player player;
    [SerializeField] private Canvas canvas;
    [SerializeField] private List<Button> buttons;
    private State state;

    public enum State
    {
        Adding,
        Removing,
        Clearing
    }

    private void Start()
    {
        player = Player.Instance;
        canvas = GameObject.Find("ContainerCanvas").GetComponent<Canvas>();
        
        if (canvas != null)
        {
            GetButtons();
        }
    }

    private void GetButtons()
    {
        if (canvas == null) return;

        buttons = new List<Button>(canvas.GetComponentsInChildren<Button>());
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
        
        // buttons.Where(name => name.name == "AddLeftHandButton").FirstOrDefault().gameObject.SetActive(false);
        // buttons.Where(name => name.name == "AddRightHandButton").FirstOrDefault().gameObject.SetActive(false);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject.Find("RemoveFromHands").SetActive(false);
        GameObject.Find("AddToHands").SetActive(false);
    }

    private void OnButtonClicked(Button button)
    {
        switch (button.name)
        {
            case "AddLeftHandButton":
                AddToHand(0);
                break;
            case "AddRightHandButton":
                AddToHand(1);
                break;
            case "RemoveFromLeftHandButton":
                AddObject(player.objectsInHands[0], true);
                break;
            case "RemoveFromRightHandButton":
                AddObject(player.objectsInHands[1], false);
                break;
            default:
                break;
        }
    }

    private void AddObject(GameObject obj, bool isLeftHand)
    {
        if (obj == null) return;
        
        GameObject newItem = Instantiate(obj, transform);
        newItem.transform.position = transform.position;
        newItem.gameObject.SetActive(false);

        objectsInContainer.Add(newItem);

        if (isLeftHand)
        {
            Destroy(player.objectsInHands[0]);
        }
        else
        {
            Destroy(player.objectsInHands[1]);
        }
    }
    
    private void AddToHand(int handIndex)
    {
        if (objectsInContainer.Count == 0) return;

        if (player.objectsInHands[0] == null)
        {
            objectsInContainer[^1].SetActive(true);
            player.HandlePickup(objectsInContainer[^1], handIndex);
            objectsInContainer.RemoveAt(objectsInContainer.Count - 1);
        }
        else if (player.objectsInHands[1] == null)
        {
            objectsInContainer[^1].SetActive(true);
            player.HandlePickup(objectsInContainer[^1], handIndex);
            objectsInContainer.RemoveAt(objectsInContainer.Count - 1);
        }
        
    }

    private void RemoveObject(GameObject obj)
    {
        if (obj == null) return;

        objectsInContainer.Remove(obj);
    }

    public List<GameObject> GetObjects()
    {
        return objectsInContainer;
    }

    private void ClearContainer()
    {
        objectsInContainer.Clear();
    }
}