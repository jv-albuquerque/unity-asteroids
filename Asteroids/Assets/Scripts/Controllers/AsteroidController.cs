using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AsteroidSize
{
    Big = 1,
    Medium = 2,
    Small = 3,
}

public class AsteroidController : GenericSpaceObject
{
    [SerializeField] private float speed = 0.4f;
    [SerializeField] private List<AudioClip> explosions;

    private AsteroidSize size = AsteroidSize.Big;

    private Rigidbody2D rb;

    private GameController gameController;

    float maxVelocity;
    private Vector2 moveDirection;
    private float moveAngle;

    private List<int> pointsToAdd = new List<int>(new int[] { 20, 50, 100 });

    protected override void AwakenCall()
    {
        base.AwakenCall();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        gameController = GameController.Instance;       
    }

    private void FixedUpdate()
    {
        //Clamp Velocity
        if (rb.velocity.magnitude > maxVelocity)
            rb.velocity = moveDirection * maxVelocity;
    }

    public void Init()
    {
        SetVelocity(Random.Range(0, 360));
    }

    public void SetVelocity(float angle)
    {
        Vector2 direction = GenericUtilities.Rotate(Vector2.right, angle);

        moveAngle = angle;

        moveDirection = direction.normalized;

        maxVelocity = speed * (float)size;
        rb.velocity = moveDirection * maxVelocity;
    }

    public void SetSize(int newSize)
    {
        size = (AsteroidSize)newSize;

        transform.localScale = Vector3.one * (1.6f - newSize*0.3f);
    }

    public void OnDestroyAsteroid(bool shootByPlayer)
    {
        if (size != AsteroidSize.Small)
        {
            for (int i = 0; i < 2; i++)
            {
                float newAngle = moveAngle + Random.Range(-45, 45);

                Vector2 dir = GenericUtilities.Rotate(Vector2.right, newAngle);
                Vector2 pos = (Vector2)transform.position + dir * Random.Range(0.1f, 0.75f);

                AsteroidController asteroid = Instantiate(gameObject, pos, Quaternion.identity).GetComponent<AsteroidController>();

                asteroid.SetSize((int)size + 1);
                asteroid.SetVelocity(newAngle);
            }
        }

        gameController.AsteroidDestoyed(pointsToAdd[(int)size - 1], shootByPlayer);

        SoundController.PlayOneShot(explosions[(int)size - 1]);

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
