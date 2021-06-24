using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : GenericSpaceObject
{
    private bool shootByPlayer = true;

    [SerializeField] private float timeToDie = 1.5f;
    [SerializeField] private float speed = 10;

    public GameObject GetGameObject { get => gameObject; }
    public bool ShootByPlayer { get => shootByPlayer; set => shootByPlayer = value; }

    private Rigidbody2D rb;

    protected override void AwakenCall()
    {
        base.AwakenCall();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        GameController.Instance.DestroyGameObjects += DestroyObject;
        Invoke("DestroyObject", timeToDie);
    }

    private void DestroyObject()
    {
        GameController.Instance.DestroyGameObjects -= DestroyObject;
        Destroy(gameObject);
    }

    public void Shoot(Vector2 direction)
    {
        rb.velocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().KillPlayer();
        }
        else if (collision.tag == "Asteroid")
        {
            collision.GetComponent<AsteroidController>().OnDestroyAsteroid(ShootByPlayer);
        }
        else if(collision.tag == "Ufo")
        {
            collision.GetComponent<UfoController>().KillUfo(ShootByPlayer);
        }

        DestroyObject();
    }
}
