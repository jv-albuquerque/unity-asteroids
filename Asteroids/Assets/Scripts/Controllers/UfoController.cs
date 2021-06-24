using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoController : GenericSpaceObject
{
    [SerializeField] private GameObject bulletObject;
    [SerializeField] private AudioClip shootAudio;
    [SerializeField] private float maxVelocity = 2;
    [SerializeField] private List<AudioClip> explosions;

    private GameController gameController;
    private Rigidbody2D rb;

    private int points = 1000;

    private bool goingRight = false;

    private float xLimit;

    private bool movingOnHorizontal = true;
    private float delayChangeDirection = 1;

    private float delayToShoot = 0.5f;

    protected override void AwakenCall()
    {
        base.AwakenCall();
        rb = GetComponent<Rigidbody2D>();
        WrapFromScreenOnX = false;
    }

    private void Start()
    {
        gameController = GameController.Instance;
        gameController.Player.OnDeath += DestroyUfo;
    }

    protected override void UpdateCall()
    {
        base.UpdateCall();

        if(Mathf.Abs(xLimit - transform.position.x) <= 0.1f)
        {
            DestroyUfo();
        }
    }

    public void Init(bool _goingRight)
    {
        goingRight = _goingRight;

        int xMultiplier = goingRight ? -1 : 1;

        float size = render.bounds.size.x;
        size /= 2; //this is made, because the pivot is in the center of the ship

        Vector2 bounds = GenericUtilities.MainCameraBounds().extents;
        Vector2 InstancePos = new Vector2((bounds.x + size) * xMultiplier, Random.Range(-bounds.y, bounds.y));

        xLimit = (bounds.x + size) * xMultiplier * -1;

        transform.position = InstancePos;
        
        rb.velocity = Vector2.left * maxVelocity* xMultiplier;

        Invoke("ChangeDirection", delayChangeDirection);

        Invoke("Shoot", 0.1f);
    }

    private void ChangeDirection()
    {
        int xMultiplier = goingRight ? -1 : 1;
        Vector2 velocity = Vector2.left * maxVelocity * xMultiplier;

        if (movingOnHorizontal)
        {
            int clockwise = Random.Range(0, 2);
            clockwise = (clockwise * 2) - 1; // -1 or 1

            velocity = GenericUtilities.Rotate(velocity, 45 * clockwise);
        }

        movingOnHorizontal = !movingOnHorizontal;

        rb.velocity = velocity;

        delayChangeDirection = Random.Range(1.0f, 3.0f);

        delayChangeDirection /= movingOnHorizontal ? 1 : 2;

        Invoke("ChangeDirection", delayChangeDirection);
    }

    private void Shoot()
    {
        float angle = Random.Range(0, 360);

        Vector2 velocity = GenericUtilities.Rotate(Vector2.right, angle);
        Vector2 shootSpot = (velocity * 1.1f) + (Vector2)transform.position;
        
        var bullet = Instantiate(bulletObject, shootSpot, Quaternion.Euler(0, 0, angle)).GetComponent<BulletController>();

        bullet.Shoot(velocity);
        bullet.ShootByPlayer = false;

        SoundController.PlayOneShot(shootAudio);

        Invoke("Shoot", delayToShoot);
        delayToShoot = Random.Range(0.8f, 1.2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().KillPlayer();
            KillUfo(false);
        }
        else if (collision.tag == "Asteroid")
        {
            collision.GetComponent<AsteroidController>().OnDestroyAsteroid(false);
            KillUfo(false);
        }
    }

    public void KillUfo(bool shootByPlayer = false)
    {
        SoundController.PlayOneShot(explosions[Random.Range(0, explosions.Count)]);

        gameController.AsteroidDestoyed(points, shootByPlayer);

        DestroyUfo();
    }

    public void DestroyUfo()
    {
        gameController.Player.OnDeath -= DestroyUfo;
        Destroy(gameObject);
    }
}
