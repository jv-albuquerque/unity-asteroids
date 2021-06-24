using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public delegate void Listener();

public class PlayerController : MonoBehaviour
{
    public Listener OnDeath = delegate { };

    [SerializeField] private List<AudioClip> explosions;

    private Rigidbody2D rb;

    private ShipInfo ship;

    //Verify if the ship is to accelerate
    private bool accelerating = false;
    //If the rotation is clockwise (1) or not (-1)
    private float clockwise = 0;

    private GameController gameController;
    private Collider2D coll;

    private bool dead = false;
    
    private bool canPlayMotorAudio = true;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ship = GetComponentInChildren<ShipInfo>();
        coll = GetComponent<Collider2D>();
    }

    private void Start()
    {
        gameController = GameController.Instance;
    }

    private void Update()
    {
        //Verify if need to wrap because the ship passed the screen edge
        if (rb.velocity.magnitude > 0)
        {
            GenericUtilities.WrapFromScreenEdge(transform, GenericUtilities.GetScreenWrapOffset(ship.Renderer));
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
        
        if(canPlayMotorAudio)
        {
            SoundController.PlayOneShot(ship.MotorAudio);
            canPlayMotorAudio = false;
            Invoke("DelayMotorAudio", ship.MotorAudio.length);
        }
    }

    private void DelayMotorAudio()
    {
        canPlayMotorAudio = true;
    }

    private void RotateShip()
    {
        transform.RotateAround(transform.position, Vector3.forward, clockwise* ship.RotationForce * Time.deltaTime);
    }

    public void OnRotate(InputAction.CallbackContext value)
    {
        if (dead)
        {
            gameController.StartGame();
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
            gameController.StartGame();
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
            gameController.StartGame();
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

            SoundController.PlayOneShot(ship.ShootAudio);
        }
    }

    public void KillPlayer()
    {
        coll.enabled = false;
        ship.Renderer.enabled = false;
        accelerating = false;
        dead = true;
        SoundController.PlayOneShot(explosions[Random.Range(0, explosions.Count)]);

        OnDeath();
        Invoke("TryResetPlayer", 1.0f);
    }

    private void TryResetPlayer()
    {
        gameController.PlayerDied();
    }

    public void ResetShip()
    {
        transform.position = Vector2.zero;

        clockwise = 0;
        transform.rotation = Quaternion.identity;

        accelerating = false;
        rb.velocity = Vector2.zero;

        dead = false;
        coll.enabled = true;
        ship.Renderer.enabled = true;
    }

    public void DisableShip()
    {
        coll.enabled = false;
        ship.Renderer.enabled = false;
        dead = true;
    }
}
