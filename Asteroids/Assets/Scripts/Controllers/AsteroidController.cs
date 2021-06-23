using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AsteroidSize
{
    Big = 1,
    Medium = 2,
    Small = 3,
}

public class AsteroidController : MonoBehaviour
{

    [SerializeField] private float speed = 1;

    private AsteroidSize size = AsteroidSize.Big;

    private Renderer render;
    private Rigidbody2D rb;

    private GameController gameController;
    private GenericUtilities genericUtilities;

    private Vector2 moveDirection;
    private float moveAngle;

    private List<int> pointsToAdd = new List<int>(new int[] { 20, 50, 100 });

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        render = GetComponent<Renderer>();
    }

    private void Start()
    {
        gameController = GameController.Instance;
        genericUtilities = GameController.Instance.genericUtilities;        
    }

    private void Update()
    {
        genericUtilities.WrapFromScreenEdge(transform, genericUtilities.GetScreenWrapOffset(render));
    }

    private void FixedUpdate()
    {
        //Clamp Velocity
        float maxVelocity = speed + (float)size;
        if (rb.velocity.magnitude > maxVelocity)
            rb.velocity = moveDirection * maxVelocity;
    }

    public void SetVelocity(float angle)
    {
        if(genericUtilities == null)
        {
            genericUtilities = GameController.Instance.genericUtilities;
        }

        Vector2 direction = genericUtilities.Rotate(Vector2.right, angle);

        moveAngle = angle;

        moveDirection = direction.normalized;
        rb.velocity = moveDirection * (speed + (float)size);
    }

    public void SetSize(int newSize)
    {
        size = (AsteroidSize)newSize;

        transform.localScale = Vector3.one * (1.6f - newSize*0.3f);
    }

    public void OnDestroyAsteroid(bool destoyedByPlayer)
    {
        if (size != AsteroidSize.Small)
        {
            for (int i = 0; i < 2; i++)
            {
                float newAngle = moveAngle + Random.Range(-45, 45);

                Vector2 dir = genericUtilities.Rotate(Vector2.right, newAngle);
                Vector2 pos = (Vector2)transform.position + dir * Random.Range(0.1f, 0.75f);

                AsteroidController asteroid = Instantiate(gameObject, pos, Quaternion.identity).GetComponent<AsteroidController>();

                asteroid.SetVelocity(newAngle);
                asteroid.SetSize((int)size + 1);
            }
        }

        gameController.AsteroidDestoyed(pointsToAdd[(int)size - 1], destoyedByPlayer);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().KillPlayer();
            OnDestroyAsteroid(false);
        }
    }
}
