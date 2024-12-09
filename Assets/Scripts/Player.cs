using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference rotation;
    
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    
    private void OnEnable()
    {
        movement.action.Enable();
        rotation.action.Enable();
    }
    
    private void OnDisable()
    {
        movement.action.Disable();
        rotation.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float move = movement.action.ReadValue<float>();
        float rotate = rotation.action.ReadValue<float>();
        
        transform.Translate(Vector3.forward * (move * speed * Time.deltaTime));
        transform.Rotate(Vector3.up * (rotate * rotationSpeed * Time.deltaTime));
        
    }
}
