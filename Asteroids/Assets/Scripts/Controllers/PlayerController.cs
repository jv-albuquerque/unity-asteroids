using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;

    [SerializeField] private float maxVelocity = 3;
    [SerializeField] private float moveForce = 10;
    [SerializeField] private float rotationForce = 100;

    //Verify if the ship is to accelerate
    private bool accelerating = false;
    //If the rotation is clockwise (1) or not (-1)
    private float clockwise = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(accelerating)
        {
            MoveShip();
        }

        if(clockwise != 0)
        {
            RotateShip();
        }
    }

    private void MoveShip()
    {
        Vector2 force = transform.up * moveForce;

        rb.AddForce(force);

        ClampVelocity();
    }

    private void ClampVelocity()
    {
        float x = Mathf.Clamp(rb.velocity.x, -maxVelocity, maxVelocity);
        float y = Mathf.Clamp(rb.velocity.y, -maxVelocity, maxVelocity);

        rb.velocity = new Vector2(x, y);
    }

    private void RotateShip()
    {
        transform.RotateAround(transform.position, Vector3.forward, clockwise* rotationForce * Time.deltaTime);
    }

    public void OnRotate(InputAction.CallbackContext value)
    {
        clockwise = value.ReadValue<float>();

        //only to make the right button go clockwise
        clockwise *= -1;
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        if(value.started)
        {
            accelerating = true;
        }
        if (value.canceled)
        {
            accelerating = false;
        }
    }
}
