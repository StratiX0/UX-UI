using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Container : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsInContainer;
    [SerializeField] private Player player;
    [SerializeField] private Canvas canvas;
    [SerializeField] private List<Button> buttons;
    private State state;

    public enum State
    {
        Adding,
        Removing,
        Clearing
    }

    private void Awake()
    {
        if (player == null)
        {
            player = Player.Instance;
        }
        
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

    private void OnButtonClicked(Button button)
    {
        switch (button.name)
        {
            case "AddButton":
                break;
            case "RemoveObjectButton":
                if (objectsInContainer.Count > 0)
                {
                    RemoveObject(objectsInContainer[0]);
                }
                break;
            case "ClearContainerButton":
                ClearContainer();
                break;
            case "AddLeftHandButton":
                AddObject(player.objectsInHands[0], true);
                player.objectsInHands[0] = null;
                break;
            case "AddRightHandButton":
                AddObject(player.objectsInHands[1], false);
                Destroy(player.objectsInHands[1]);
                player.objectsInHands[1] = null;
                break;
            default:
                break;
        }
    }

    private void AddObject(GameObject obj, bool isLeftHand)
    {
        if (obj == null) return;
        
        obj.transform.SetParent(transform);
        obj.transform.position = transform.position;
        obj.gameObject.SetActive(false);

        objectsInContainer.Add(obj);

        if (isLeftHand)
        {
            player.objectsInHands[0] = null;
        }
        else
        {
            player.objectsInHands[1] = null;
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