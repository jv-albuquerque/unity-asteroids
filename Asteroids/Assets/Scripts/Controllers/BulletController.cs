using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Renderer render;

    private bool shootByPlayer = true;

    [SerializeField] private float timeToDie = 1.5f;
    [SerializeField] private float speed = 10;

    public GameObject GetGameObject { get => gameObject; }

    private Rigidbody2D rb;

    private GenericUtilities genericUtilities;

    private void Awake()
    {
        render = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        genericUtilities = GameController.Instance.genericUtilities;
        Invoke("DestroyOnTime", timeToDie);
    }

    //TODO: Otimaization to create: preloaded gameObjects, and don't destroy, put in a stack able to be used (reuse gameObjects)
    private void DestroyOnTime()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        genericUtilities.WrapFromScreenEdge(transform, genericUtilities.GetScreenWrapOffset(render));
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
            collision.GetComponent<AsteroidController>().OnDestroyAsteroid(shootByPlayer);
        }

        Destroy(gameObject);
    }
}
