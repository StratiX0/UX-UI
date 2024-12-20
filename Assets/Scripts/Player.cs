using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using LitMotion;

public class Player : MonoBehaviour
{
    public static Player Instance;
    
    private Rigidbody rb;
    [Header("Input Actions")]
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference rotation;
    [SerializeField] private InputActionReference leftClick;
    private float move;
    private float rotate;
    private float click;
    
    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    
    [Header("References")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas objectCanvas;
    public Canvas containerCanvas;
    public Canvas storageCanvas;
    
    [Header("Objects Settings")]
    [SerializeField] private Material highlightMat;
    [SerializeField] private List<Material> hoveredObjectMats;
    private bool isAnimating;
    private Vector3 selectedObjectPos;
    private CompositeMotionHandle compMotionHandle;
    private GameObject lastHoveredObject;
    private GameObject lastHoveredContainer;
    private GameObject lastHoveredStorage;
    [SerializeField] private GameObject selectedObject;
    public GameObject[] objectsInHands;
    [SerializeField] private Camera[] camerasObject;
    [SerializeField] private bool isPlacingObj;
    [SerializeField] private bool inMenu;
    private int placingObjIndex;
    [SerializeField] private GameObject[] placingMenus;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        movement.action.Enable();
        rotation.action.Enable();
        leftClick.action.Enable();
    }
    
    private void OnDisable()
    {
        movement.action.Disable();
        rotation.action.Disable();
        leftClick.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        isAnimating = false;
        selectedObjectPos = new Vector3(9999999, 9999999, 9999999);
        compMotionHandle = new CompositeMotionHandle();
        objectsInHands = new GameObject[2];
        mainCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!inMenu)
        {
            GetInputs();
            Move();
            SelectObject();
            // ContainerManagement();
            StorageManagement();
        }
        
        if (isPlacingObj)
        {
            PlaceObject(placingObjIndex);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void GetInputs()
    {
        move = movement.action.ReadValue<float>();
        rotate = rotation.action.ReadValue<float>();
        click = leftClick.action.ReadValue<float>();
    }
    
    private void Move()
    {
        Vector3 targetVelocity = transform.forward * (move * speed);
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * 5f);

        Quaternion targetRotation = Quaternion.Euler(0, rotate * rotationSpeed, 0);
        // transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * targetRotation, 5f);
        transform.Rotate(transform.up, rotationSpeed * rotate * Time.fixedDeltaTime);
    }
    
    private void SelectObject()
    {
        RaycastHit hit;
        Vector2 mousePosition = InputSystem.GetDevice<Mouse>().position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
    
        bool hitGrabbable = Physics.Raycast(ray, out hit, 3f, LayerMask.GetMask("Grabbable"));
        bool hitNothing = !hitGrabbable && !Physics.Raycast(ray, out hit, 3f, LayerMask.GetMask("Grabbable"));
    
        if (hitGrabbable && !inMenu)
        {
            Highlight(hit.transform.gameObject, true);
            highlightMat = hit.transform.GetComponent<MeshRenderer>().materials[1];
            selectedObjectPos = hit.transform.position;
    
            if (click > 0)
            {
                if (!isAnimating)
                {
                    CreateHighlightAnimation();
                    isAnimating = true;
                }
    
                selectedObject = hit.transform.gameObject;
                objectCanvas.gameObject.SetActive(true);
                if (hit.transform.CompareTag("Ingredients"))
                {
                    GameObject.Find("AddObjects").SetActive(false);
                }
                containerCanvas.gameObject.SetActive(false);
                objectCanvas.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.75f));
                objectCanvas.transform.rotation = Quaternion.LookRotation(objectCanvas.transform.position - Camera.main.transform.position);
                containerCanvas.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.75f));
                containerCanvas.transform.rotation = Quaternion.LookRotation(containerCanvas.transform.position - Camera.main.transform.position);
                inMenu = true;
            }
        }
        else if (hitNothing && hoveredObjectMats != null)
        {
            Highlight(lastHoveredObject, false);
        }
    
        if (Vector3.Distance(transform.position, selectedObjectPos) > 2f)
        {
            if (isAnimating)
            {
                selectedObject = null;
                StopHighlightAnimation();
                highlightMat.SetFloat("_Thickness", 0);
                isAnimating = false;
            }
            objectCanvas.gameObject.SetActive(false);
            inMenu = false;
        }
    }
    
    public void HandlePickup(GameObject pickedObject, int hand)
    {
        if (objectsInHands[hand] != null) return;
        pickedObject.GetComponent<Rigidbody>().isKinematic = true;
        pickedObject.transform.SetParent(transform);
        pickedObject.transform.localPosition = camerasObject[hand].transform.localPosition - new Vector3(2, 2, 0);
        camerasObject[hand].transform.LookAt(pickedObject.transform);
        pickedObject.gameObject.layer = LayerMask.NameToLayer("Grabbed");
        foreach (var child in pickedObject.transform.gameObject.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer("Grabbed");
        }
        objectsInHands[hand] = pickedObject;
        mainCanvas.gameObject.SetActive(true);
    }
    
    private void ContainerManagement()
    {
        RaycastHit hit;
        Vector2 mousePosition = InputSystem.GetDevice<Mouse>().position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        bool hitContainer = Physics.Raycast(ray, out hit, 3f, LayerMask.GetMask("Grabbable"));
        bool container = hit.transform.CompareTag("Container");

        if (hitContainer && container && !inMenu)
        {
            Highlight(hit.transform.gameObject, true);
            highlightMat = hit.transform.GetComponent<MeshRenderer>().materials[1];
            selectedObjectPos = hit.transform.position;

            if (click > 0)
            {
                if (!isAnimating)
                {
                    CreateHighlightAnimation();
                    isAnimating = true;
                }

                selectedObject = hit.transform.gameObject;
                containerCanvas.gameObject.SetActive(true);
                // containerCanvas.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0.5f));
                containerCanvas.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.75f));
                containerCanvas.transform.rotation = Quaternion.LookRotation(containerCanvas.transform.position - Camera.main.transform.position);
                inMenu = true;
            }
        }
        else if (!hitContainer && container && hoveredObjectMats != null)
        {
            Highlight(lastHoveredContainer, false);
        }

        if (Vector3.Distance(transform.position, selectedObjectPos) > 2f)
        {
            if (isAnimating)
            {
                selectedObject = null;
                StopHighlightAnimation();
                highlightMat.SetFloat("_Thickness", 0);
                isAnimating = false;
            }
            containerCanvas.gameObject.SetActive(false);
            inMenu = false;
        }
    }

    public void PickupHand(int hand)
    {
        if (hand == 0)
        {
            HandlePickup(selectedObject, 0);
        }
        else if (hand == 1)
        {
            HandlePickup(selectedObject, 1);
        }
        
        compMotionHandle.Cancel();
        highlightMat.SetFloat("_Thickness", 0);
    }
    
    public void IsPlacingObject(bool state)
    {
        isPlacingObj = state;
    }
    public void IsPlacingIndex(int objIndex)
    {
        placingObjIndex = objIndex;
    }

    private void PlaceObject(int objIndex)
    {
        if (!isPlacingObj) return;
        Vector2 mousePosition = InputSystem.GetDevice<Mouse>().position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        int excludeLayer1 = LayerMask.GetMask("UI");
        
        if (click > 0 && Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            objectsInHands[objIndex].transform.position = hit.point;
            objectsInHands[objIndex].transform.SetParent(null);
            objectsInHands[objIndex].layer = LayerMask.NameToLayer("Grabbable");
            objectsInHands[objIndex].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            foreach (var child in objectsInHands[objIndex].transform.gameObject.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = LayerMask.NameToLayer("Grabbable");
            }
            objectsInHands[objIndex].GetComponent<Rigidbody>().isKinematic = false;
            objectsInHands[objIndex].SetActive(true);
            objectsInHands[objIndex] = null;
            isPlacingObj = false;
            placingMenus[objIndex].SetActive(false);
        }
    }
    
    private void StorageManagement()
    {
        RaycastHit hit;
        Vector2 mousePosition = InputSystem.GetDevice<Mouse>().position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (Physics.Raycast(ray, out hit, 3f, LayerMask.GetMask("Storage")) && !inMenu)
        {
            Highlight(hit.transform.gameObject, true);
            
            selectedObjectPos = hit.transform.position;

            if (click > 0 && selectedObject != hit.transform.gameObject)
            {
                Highlight(hit.transform.gameObject, false);
                storageCanvas.gameObject.SetActive(true);

                Furniture furniture = hit.transform.GetComponent<Furniture>();
                
                furniture.ClearUi();
                
                if (furniture.isMagic)
                {
                    furniture.CreateShelves();
                }
                furniture.UpdateMenu();
                
                
                inMenu = true;
            }
        }
        
        else if (!Physics.Raycast(ray, out hit, 3f, LayerMask.GetMask("Storage")))
        {
            if (hoveredObjectMats != null)
            {
                Highlight(lastHoveredStorage, false);
            }
        }
        
        else if (Vector3.Distance(transform.position, selectedObjectPos) > 3f)
        {
            storageCanvas.gameObject.SetActive(false);
            inMenu = false;
        }
    }
    
    private void Highlight(GameObject hoverObject, bool highlight)
    {
        if (hoverObject == null) return;

        ApplyHighlight(hoverObject, highlight);

        foreach (Transform child in hoverObject.transform)
        {
            ApplyHighlight(child.gameObject, highlight);
        }

        if (hoverObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            lastHoveredObject = hoverObject.gameObject;
        }

        if (hoverObject.layer == LayerMask.NameToLayer("Storage"))
        {
            lastHoveredStorage = hoverObject.gameObject;
        }

        if (hoverObject.layer == LayerMask.NameToLayer("Container"))
        {
            lastHoveredContainer = hoverObject.gameObject;
        }
    }

    private void ApplyHighlight(GameObject obj, bool highlight)
    {
        var renderer = obj.GetComponentInChildren<MeshRenderer>();
        if (renderer == null) return;

        foreach (Material mat in renderer.materials)
        {
            if (highlight)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", new Color(1, 1, 1, 1) * 0.25f);
            }
            else
            {
                mat.SetColor("_EmissionColor", Color.black);
                mat.DisableKeyword("_EMISSION");
            }
        }
    }

    private void CreateHighlightAnimation()
    {
        LMotion.Create(0f, 0.04f, 1f).WithLoops(-1, LoopType.Yoyo).Bind(x => highlightMat.SetFloat("_Thickness", x)).AddTo(compMotionHandle);
    }

    public void StopHighlightAnimation()
    {
        if (highlightMat == null) return;
        highlightMat.SetFloat("_Thickness", 0f);
        compMotionHandle.Cancel();
        compMotionHandle.Clear();
    }
    
    public void OpenMenu()
    {
        inMenu = true;
    }
    
    public void CloseMenu()
    {
        StopHighlightAnimation();
        inMenu = false;
    }
    
    public void RemoveObjectFromHand(int hand)
    {
        Destroy(objectsInHands[hand]);
        objectsInHands[hand] = null;
    }
}
