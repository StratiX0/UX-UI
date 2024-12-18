using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ObjectButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    public Player player;
    public GameObject gridshelf;
    public GameObject parent;
    public Furniture furniture;
    public KitchenObject kitchenObject;
    public GameObject descriptionPanel;
    public GameObject collectButton;
    public GameObject storeButton;
    public GameObject leftHandButton;
    public GameObject rightHandButton;
    private Sprite icon;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI descriptionText;
    public bool isSelected;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        leftHandButton = GameObject.Find("StoreLeftHand");
        rightHandButton = GameObject.Find("StoreRightHand");
        collectButton = GameObject.Find("Collect");
        storeButton = GameObject.Find("Store");
        gridshelf = GameObject.Find("GridShelf");
        descriptionPanel = GameObject.Find("DescriptionPanel");
        nameText = descriptionPanel.transform.Find("ObjectName").GetComponent<TextMeshProUGUI>();
        descriptionText = descriptionPanel.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        DisplayIcon();
        isSelected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DisplayDescription();
        isSelected = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        parent.GetComponent<LayoutElement>().ignoreLayout = true;
        parent.GetComponent<RectTransform>().position = Input.mousePosition;
        leftHandButton.GetComponent<Image>().color = Color.green;
        rightHandButton.GetComponent<Image>().color = Color.green;
    }
    
    private bool IsMouseOverParent()
    {
        if (gridshelf == null) return false; // Vérifie si le parent existe

        RectTransform gridshelftRect = gridshelf.GetComponent<RectTransform>();
        if (gridshelftRect == null) return false; // Vérifie si le parent a un RectTransform

        Vector2 localMousePosition = gridshelftRect.InverseTransformPoint(Input.mousePosition);
        return gridshelftRect.rect.Contains(localMousePosition); // Vérifie si la souris est dans les limites du RectTransform
    }
    
    private bool IsMouseOverLeftHand()
    {
        if (leftHandButton == null) return false; // Vérifie si le parent existe

        RectTransform leftHandRect = leftHandButton.GetComponent<RectTransform>();
        if (leftHandRect == null) return false; // Vérifie si le parent a un RectTransform

        Vector2 localMousePosition = leftHandRect.InverseTransformPoint(Input.mousePosition);
        return leftHandRect.rect.Contains(localMousePosition); // Vérifie si la souris est dans les limites du RectTransform
    }
    
    private bool IsMouseOveRightHand()
    {
        if (rightHandButton == null) return false; // Vérifie si le parent existe

        RectTransform rightHandRect = rightHandButton.GetComponent<RectTransform>();
        if (rightHandRect == null) return false; // Vérifie si le parent a un RectTransform

        Vector2 localMousePosition = rightHandRect.InverseTransformPoint(Input.mousePosition);
        return rightHandRect.rect.Contains(localMousePosition); // Vérifie si la souris est dans les limites du RectTransform
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        parent.GetComponent<LayoutElement>().ignoreLayout = false;
        parent.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        
        leftHandButton.GetComponent<Image>().color = Color.white;
        rightHandButton.GetComponent<Image>().color = Color.white;
        
        MouseOrderElements();
        AddToHands();

        Debug.Log("End Drag");
    }
    
    private void AddToHands()
    {
        if (IsMouseOverLeftHand() && player.objectsInHands[0] == null)
        {
            GameObject obj = Instantiate(kitchenObject.GetPrefab());
            player.HandlePickup(obj, 0);
            foreach (var shelf in furniture.GetComponentsInChildren<Shelf>())
            {
                if (shelf.index == furniture.currentShelf)
                {
                    shelf.RemoveObject(kitchenObject);
                }
            }
            // furniture.shelves[furniture.currentShelf].objects.Remove(kitchenObject);
            furniture.gridElements.Remove(transform.parent.gameObject);
            Destroy(parent);
        }
        else if (IsMouseOveRightHand() && player.objectsInHands[1] == null)
        {
            GameObject obj = Instantiate(kitchenObject.GetPrefab());
            player.HandlePickup(obj, 1);
            foreach (var shelf in furniture.GetComponentsInChildren<Shelf>())
            {
                if (shelf.index == furniture.currentShelf)
                {
                    shelf.RemoveObject(kitchenObject);
                }
            }
            furniture.gridElements.Remove(transform.parent.gameObject);
            Destroy(parent);
        }
    }
    
    private void MouseOrderElements()
    {
        if (!IsMouseOverParent()) return;
        
        float distance = 100000;
        GameObject closestElement = null;

        foreach (var element in furniture.gridElements)
        {
            if (element.transform.parent.gameObject == element) continue;

            if (Vector3.Distance(element.transform.position, Input.mousePosition) < distance)
            {
                distance = Vector3.Distance(element.transform.position, Input.mousePosition);
                closestElement = element;
            }
        }

        if (closestElement != null)
        {
            int closestIndex = furniture.gridElements.IndexOf(closestElement);
            int currentIndex = furniture.gridElements.IndexOf(transform.parent.gameObject);

            if (currentIndex < closestIndex)
            {
                for (int i = currentIndex; i < closestIndex; i++)
                {
                    furniture.gridElements[i] = furniture.gridElements[i + 1];
                }
            }
            else
            {
                for (int i = currentIndex; i > closestIndex; i--)
                {
                    furniture.gridElements[i] = furniture.gridElements[i - 1];
                }
            }

            furniture.gridElements[closestIndex] = transform.parent.gameObject;
        }

        foreach (Shelf shelf in furniture.transform.GetComponentsInChildren<Shelf>())
        {
            if (shelf.index == furniture.currentShelf)
            {
                shelf.GetObjects().Clear();
                for (int i = 0; i < furniture.gridElements.Count; i++)
                {
                    shelf.AddObject(furniture.gridElements[i].GetComponentInChildren<ObjectButton>().kitchenObject);
                }
            }
        }

        furniture.UpdateGrid(furniture.currentShelf);
    }

    private void DisplayDescription()
    {
        nameText.text = kitchenObject.GetName();
        descriptionText.text = kitchenObject.GetDescription();
        descriptionPanel.transform.Find("Icon").gameObject.SetActive(true);
        descriptionPanel.transform.Find("Icon").GetComponent<Image>().sprite = icon;
    }

    private void DisplayIcon()
    {
        icon = kitchenObject.GetIcon();
        transform.GetComponentInChildren<Image>().sprite = icon;
    }
}