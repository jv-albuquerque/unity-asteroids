using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    private ShipInfo ship;

    //Verify if the ship is to accelerate
    private bool accelerating = false;
    //If the rotation is clockwise (1) or not (-1)
    private float clockwise = 0;

    private GameController gameController;
    private GenericUtilities genericUtilities;
    private Collider2D collider;

    private bool dead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ship = GetComponentInChildren<ShipInfo>();
        collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        gameController = GameController.Instance;
        genericUtilities = GameController.Instance.genericUtilities;
    }

    private void Update()
    {
        //Verify if need to wrap because the ship passed the screen edge
        if (rb.velocity.magnitude > 0)
        {
            genericUtilities.WrapFromScreenEdge(transform, genericUtilities.GetScreenWrapOffset(ship.Renderer));
        }
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
        Vector2 force = transform.up * ship.MoveForce;

        rb.AddForce(force);

        //Clamp Velocity
        if (rb.velocity.magnitude > ship.MaxVelocity)
            rb.velocity = rb.velocity.normalized * ship.MaxVelocity;        
    }

    private void RotateShip()
    {
        transform.RotateAround(transform.position, Vector3.forward, clockwise* ship.RotationForce * Time.deltaTime);
    }

    public void OnRotate(InputAction.CallbackContext value)
    {
        if (dead)
        {
            return;
        }

        clockwise = value.ReadValue<float>();

        //only to make the right button go clockwise
        clockwise *= -1;
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        if (dead)
        {
            return;
        }

        if (value.started)
        {
            accelerating = true;
        }
        if (value.canceled)
        {
            accelerating = false;
        }
    }

    public void OnShoot(InputAction.CallbackContext value)
    {
        if (dead)
        {
            return;
        }

        if (value.started)
        {
            List<Transform> shootPositions = ship.ShootPos;

            foreach (var shootPosition in shootPositions)
            {
                var bullet = Instantiate(ship.Bullet, shootPosition.position, transform.rotation).GetComponent<BulletController>();

                bullet.Shoot(transform.up);
            }

            Vector2 force = -transform.up * ship.Recoil;
            rb.AddForce(force);
        }
    }

    public void KillPlayer()
    {
        collider.enabled = false;
        ship.Renderer.enabled = false;
        dead = true;
        Invoke("TryResetPlayer", 1.0f);
    }

    private void TryResetPlayer()
    {
        gameController.PlayerDied();
    }

    public void ResetPlayer()
    {
        collider.enabled = true;
        transform.position = Vector2.zero;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        accelerating = false;
        dead = false;
        ship.Renderer.enabled = true;
    }
}
