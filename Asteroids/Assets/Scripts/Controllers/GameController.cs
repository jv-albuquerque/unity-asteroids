using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static public GameController Instance;

    [SerializeField] private GameObject player;
    public GenericUtilities genericUtilities = new GenericUtilities();
    private UIController ui;

    [SerializeField] private GameObject asteroidObject;

    private int level = 1;

    private int destroyedAsteroids = 0;
    private int points = 0;
    private int lifes = 5;


    private void Awake()
    {
        Singleton();

        ui = GetComponent<UIController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnAsteroids();
        ResetGame();
    }

    private void Singleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void ResetGame()
    {
        level = 1;
        destroyedAsteroids = 0;

        points = 0;
        ui.UpdatePoints(points);

        lifes = 5;
    }

    private void SpawnAsteroids()
    {
        int numAsteroids = level + 3;

        Vector2 bounds = genericUtilities.MainCameraBounds().extents;

        for (int i = 0; i < numAsteroids; i++)
        {
            Vector2 InstancePos = new Vector2(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));
            while (Vector2.Distance(player.transform.position, InstancePos) < 2)
            {
                InstancePos = new Vector2(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));
            }

            AsteroidController asteroid = Instantiate(asteroidObject, InstancePos, Quaternion.identity).GetComponent<AsteroidController>();
            asteroid.SetVelocity(Random.Range(0, 360));
        }

        destroyedAsteroids = 0;
    }

    public void AsteroidDestoyed(int pointsToAdd, bool destroyedByPlayer)
    {
        destroyedAsteroids++;

        if(destroyedByPlayer)
        {            
            AddPoints(pointsToAdd);
        }

        if(destroyedAsteroids >= (level + 3)*7)
        {
            NextLevel();
        }
    }

    private void NextLevel()
    {
        level++;
        SpawnAsteroids();
    }

    private void AddPoints(int pointsToAdd)
    {
        points += pointsToAdd;
        ui.UpdatePoints(points);
    }

    public void PlayerDied()
    {
        lifes--;

        if(lifes >= 0)
        {
            player.GetComponent<PlayerController>().ResetPlayer();
        }
        else
        {
            Debug.Log("End Game");
        }
    }
}
