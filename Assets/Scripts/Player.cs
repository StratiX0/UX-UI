using System.Collections;
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
    private Vector3 selectedObject;
    
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
        selectedObject = new Vector3(9999999, 9999999, 9999999);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetInputs();

        Move();
        
        if (click > 0)
        {
            GetObject();
        }
        
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
    
    private void GetObject()
    {
        RaycastHit hit;
        
        Vector2 mousePosition = InputSystem.GetDevice<Mouse>().position.ReadValue();
        
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (Physics.Raycast(ray, out hit, 1f, LayerMask.GetMask("Grabable")))
        {
            Debug.Log(hit.transform.name);
            
            objectCanvas.gameObject.SetActive(true);
            objectCanvas.transform.position = Camera.main.ScreenToWorldPoint(hit.transform.position);
            
            // hit.transform.SetParent(transform);
        }
    }
    
    private void HighlightObject()
    {
        RaycastHit hit;
        Vector2 mousePosition = InputSystem.GetDevice<Mouse>().position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out hit, 1f, LayerMask.GetMask("Grabable")))
        {
            objectCanvas.gameObject.SetActive(true);

            if (!isAnimating)
            {
                LMotion.Create(0f, 0.03f, 1f).WithLoops(-1, LoopType.Yoyo).Bind(x => highlightMat.SetFloat("_Thickness", x));
                isAnimating = true;
                Debug.Log("Animating");
                selectedObject = hit.transform.position;
            }
        }
    }
}
