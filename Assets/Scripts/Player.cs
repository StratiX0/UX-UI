using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using LitMotion;

public class Player : MonoBehaviour
{
    public static Player Instance;
    
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
    [SerializeField] private Canvas objectCanvas;
    public Canvas containerCanvas;
    
    [Header("Objects Settings")]
    [SerializeField] private Material highlightMat;
    [SerializeField] private List<Material> hoveredObjectMats;
    private bool isAnimating;
    private Vector3 selectedObjectPos;
    private CompositeMotionHandle compMotionHandle;
    private GameObject lastHoveredObject;
    private GameObject lastHoveredContainer;
    private GameObject lastHoveredStorage;
    private GameObject selectedObject;
    public GameObject[] objectsInHands;
    [SerializeField] private Camera[] camerasObject;
    [SerializeField] private bool isPlacingObj;
    private int placingObjIndex;
    [SerializeField] private GameObject[] placingMenus;

    private void Awake()
    {
        Instance = this;
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetInputs();

        Move();
        
        SelectObject();
        ContainerManagement();
        StorageManagement();
        
        if (isPlacingObj)
        {
            PlaceObject(placingObjIndex);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(objectsInHands[0]);
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
        transform.Translate(Vector3.forward * (move * speed * Time.deltaTime));
        transform.Rotate(Vector3.up * (rotate * rotationSpeed * Time.deltaTime));
    }
    
    private void SelectObject()
    {
        RaycastHit hit;
        Vector2 mousePosition = InputSystem.GetDevice<Mouse>().position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
    
        bool hitGrabbable = Physics.Raycast(ray, out hit, 1.5f, LayerMask.GetMask("Grabbable"));
        bool hitNothing = !hitGrabbable && !Physics.Raycast(ray, out hit, 2f, LayerMask.GetMask("Grabbable"));

        if (hitGrabbable)
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
                objectCanvas.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0.5f));
                objectCanvas.transform.rotation = Quaternion.LookRotation(objectCanvas.transform.position - Camera.main.transform.position);
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
                compMotionHandle.Cancel();
                compMotionHandle.Clear();
                highlightMat.SetFloat("_Thickness", 0);
                isAnimating = false;
            }
            objectCanvas.gameObject.SetActive(false);
        }
    }
    
    private void HandlePickup(GameObject pickedObject, int hand)
    {
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
        Debug.Log($"Picking up with {(hand == 0 ? "left" : "right")} hand");
    }
    
    private void ContainerManagement()
    {
        RaycastHit hit;
        Vector2 mousePosition = InputSystem.GetDevice<Mouse>().position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        bool hitContainer = Physics.Raycast(ray, out hit, 1.5f, LayerMask.GetMask("Container"));
        bool hitNothing = !hitContainer && !Physics.Raycast(ray, out hit, 2f, LayerMask.GetMask("Container"));

        if (hitContainer)
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
                containerCanvas.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0.5f));
                containerCanvas.transform.rotation = Quaternion.LookRotation(containerCanvas.transform.position - Camera.main.transform.position);
            }
        }
        else if (hitNothing && hoveredObjectMats != null)
        {
            Highlight(lastHoveredContainer, false);
        }

        if (Vector3.Distance(transform.position, selectedObjectPos) > 2f)
        {
            if (isAnimating)
            {
                selectedObject = null;
                compMotionHandle.Cancel();
                compMotionHandle.Clear();
                highlightMat.SetFloat("_Thickness", 0);
                isAnimating = false;
            }
            containerCanvas.gameObject.SetActive(false);
        }
    }

    public void PickupHand(int hand)
    {
        if (hand == 0 && objectsInHands[0] == null)
        {
            HandlePickup(selectedObject, 0);
        }
        else if (hand == 1 && objectsInHands[1] == null)
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
            foreach (var child in objectsInHands[objIndex].transform.gameObject.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = LayerMask.NameToLayer("Grabbable");
            }
            objectsInHands[objIndex].GetComponent<Rigidbody>().isKinematic = false;
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
        
        if (Physics.Raycast(ray, out hit, 1.5f, LayerMask.GetMask("Storage")))
        {
            Highlight(hit.transform.gameObject, true);
            
            selectedObjectPos = hit.transform.position;

            if (click > 0)
            {
            }
        }
        
        else if (!Physics.Raycast(ray, out hit, 2f, LayerMask.GetMask("Storage")))
        {
            if (hoveredObjectMats != null)
            {
                Highlight(lastHoveredStorage, false);
            }
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
        foreach (Material mat in obj.GetComponentInChildren<MeshRenderer>().materials)
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
}
