using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using LitMotion;

public class Player : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference rotation;
    [SerializeField] private InputActionReference mouseClick;
    private float move;
    private float rotate;
    private float click;
    
    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    
    [Header("References")]
    [SerializeField] private Canvas objectCanvas;
    [SerializeField] private Material highlightMat;
    private bool isAnimating;
    private Vector3 selectedObjectPos;
    private CompositeMotionHandle compMotionHandle;
    
    private void OnEnable()
    {
        movement.action.Enable();
        rotation.action.Enable();
        mouseClick.action.Enable();
    }
    
    private void OnDisable()
    {
        movement.action.Disable();
        rotation.action.Disable();
        mouseClick.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        isAnimating = false;
        selectedObjectPos = new Vector3(9999999, 9999999, 9999999);
        compMotionHandle = new CompositeMotionHandle();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetInputs();

        Move();
        
        HighlightObject();
        
    }

    private void GetInputs()
    {
        move = movement.action.ReadValue<float>();
        rotate = rotation.action.ReadValue<float>();
        click = mouseClick.action.ReadValue<float>();
    }
    
    private void Move()
    {
        transform.Translate(Vector3.forward * (move * speed * Time.deltaTime));
        transform.Rotate(Vector3.up * (rotate * rotationSpeed * Time.deltaTime));
    }
    
    private void HighlightObject()
    {
        RaycastHit hit;
        Vector2 mousePosition = InputSystem.GetDevice<Mouse>().position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        
        if (Physics.Raycast(ray, out hit, 1f, LayerMask.GetMask("Grabable")))
        {
            objectCanvas.gameObject.SetActive(true);
            highlightMat = hit.transform.GetComponent<MeshRenderer>().materials[1];
            selectedObjectPos = hit.transform.position;
            
            if (!isAnimating)
            {
                LMotion.Create(0f, 0.03f, 1f).WithLoops(-1, LoopType.Yoyo).Bind(x => highlightMat.SetFloat("_Thickness", x)).AddTo(compMotionHandle);
                isAnimating = true;
                selectedObjectPos = hit.transform.position;
                Debug.Log("Animating");
            }
        }

        else if (Vector3.Distance(transform.position, selectedObjectPos) > 2f && isAnimating)
        {
            compMotionHandle.Cancel();
            compMotionHandle.Clear();
            highlightMat.SetFloat("_Thickness", 0);
            isAnimating = false;
            Debug.Log("Canceling");
        }
    }
}
