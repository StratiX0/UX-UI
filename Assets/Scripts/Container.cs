using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Container : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsInContainer;
    private bool isLeftHand;
    [SerializeField] private Player player;
    [SerializeField] private Canvas canvas;
    private List<Button> buttons;
    private State state;

    public enum State
    {
        Adding,
        Removing,
        Clearing
    }

    private void Awake()
    {
        SetPlayer();
    }

    public void SetCanvas(Canvas value)
    {
        canvas = value;
        
        GetButtons();
    }

    private void GetButtons()
    {
        buttons = new List<Button>(canvas.GetComponentsInChildren<Button>());
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    private void OnButtonClicked(Button button)
    {
        switch (button.name)
        {
            case "AddButton":
                state = State.Adding;
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
            case "LeftHandButton":
                SetHand(true);
                AddObject(player.objectsInHands[0]);
                Destroy(player.objectsInHands[0]);
                player.objectsInHands[0] = null;
                Debug.Log("Left hand button clicked");
                Debug.Log("Objects in hands: " + player.objectsInHands[0] + " " + player.objectsInHands[1]);
                break;
            case "RightHandButton":
                SetHand(false);
                AddObject(player.objectsInHands[1]);
                Destroy(player.objectsInHands[1]);
                player.objectsInHands[1] = null;
                Debug.Log("Right hand button clicked");
                Debug.Log("Objects in hands: " + player.objectsInHands[0] + " " + player.objectsInHands[1]);
                break;
            default:
                Debug.Log("No action defined for this button.");
                break;
        }
    }
    
    public void AddObject(GameObject obj)
    {
        objectsInContainer.Add(obj);

        if (isLeftHand)
        {
            GameObject objInHand = player.objectsInHands[0];
            player.objectsInHands[0] = null;
            Destroy(objInHand);
        }
        else
        {
            GameObject objInHand = player.objectsInHands[1];
            player.objectsInHands[1] = null;
            Destroy(objInHand);
        }
    }

    public void RemoveObject(GameObject obj)
    {
        objectsInContainer.Remove(obj);
    }

    public List<GameObject> GetObjects()
    {
        return objectsInContainer;
    }

    public void ClearContainer()
    {
        objectsInContainer.Clear();
    }

    public void SetHand(bool value)
    {
        isLeftHand = value;
    }

    public void SetPlayer()
    {
        player = Player.Instance;
    }
}