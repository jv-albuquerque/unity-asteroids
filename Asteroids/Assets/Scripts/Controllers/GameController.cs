using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Listener();

public class GameController : MonoBehaviour
{
    static public GameController Instance;

    public Listener DestroyGameObjects = delegate { };

    [SerializeField] private PlayerController player;
    private UIController ui;

    [SerializeField] private GameObject asteroidObject;
    [SerializeField] private GameObject ufoObject;

    private int level = 1;

    private int destroyedAsteroids = 0;
    private int points = 0;
    private int lifes = 5;

    private GameData data;
    public GameData Data { get => data; }

    private bool onTheGame = true;
    public bool OnTheGame { get => onTheGame; set => onTheGame = value; }

    private SoundController soundController;
    public SoundController GetSoundController { get => soundController; }
    public PlayerController Player { get => player; }
    public UIController Ui { get => ui; }

    private float ufoSpawnCooldown = 8;
    private float ufoSpawnLastTick;

    private UnityEngine.EventSystems.EventSystem eventSystem;



    private void Awake()
    {
        Singleton();

        ui = GetComponent<UIController>();
        soundController = GetComponent<SoundController>();

        data = SaveSystem.LoadData();
        if(data == null)
        {
            data = new GameData();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        eventSystem = myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>();

        OnMainMenu();
    }

    private void Update()
    {
        if ((Time.time >= ufoSpawnLastTick + ufoSpawnCooldown) && onTheGame)
        {
            SpawnUfo();
        }
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

    public void StartGame()
    {
        if(onTheGame)
        { 
            return;
        }

        onTheGame = true;

        level = 1;        
        ui.StartGame();
        player.ResetShip();
        soundController.StartGame();
        ResetGame();
        SpawnAsteroids();
        ufoSpawnLastTick = Time.time;
    }

    public void ResetGame()
    {
        level = 1;
        ui.UpdateLevel(level);
        destroyedAsteroids = 0;

        points = 0;
        ui.UpdatePoints(points);

        lifes = 5;
        ui.UpdateLifes(lifes);
    }

    public void OnMainMenu()
    {
        if(!onTheGame)
        {
            return;
        }

        player.DisableShip();
        ui.OnMainMenu();

        OnTheGame = false;
    }

    private void SpawnAsteroids()
    {
        int numAsteroids = level + 3;

        Vector2 bounds = GenericUtilities.MainCameraBounds().extents;

        for (int i = 0; i < numAsteroids; i++)
        {
            Vector2 InstancePos = new Vector2(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));
            while (Vector2.Distance(player.transform.position, InstancePos) < 2)
            {
                InstancePos = new Vector2(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));
            }

            AsteroidController asteroid = Instantiate(asteroidObject, InstancePos, Quaternion.identity).GetComponent<AsteroidController>();
            asteroid.Init();
        }

        destroyedAsteroids = 0;
    }

    private void SpawnUfo()
    {
        UfoController ufo = Instantiate(ufoObject, Vector2.zero, Quaternion.identity).GetComponent<UfoController>();

        bool goingRight = Random.Range(0, 2) == 0 ? false : true;
        ufo.Init(goingRight);

        ufoSpawnLastTick = Time.time;
        ufoSpawnCooldown = Random.Range(8.0f, 12.0f);
    }

    public void ObjectDestoyed(int pointsToAdd, bool destroyedByPlayer, bool isAsteroid = true)
    {
        if(isAsteroid)
        {
            destroyedAsteroids++;
        }

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
        ui.UpdateLevel(level);
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
        ui.UpdateLifes(lifes);

        if (lifes >= 0)
        {
            player.ResetShip();
        }
        else
        {
            bool isNewHighScore = false;
            if(data.highScore < points)
            {
                isNewHighScore = true;
                data.highScore = points;
            }

            ui.OnEndGame(isNewHighScore, points);
            SaveSystem.SaveData(data);
        }
    }

    public void BackToMainMenu()
    {
        eventSystem.SetSelectedGameObject(null);

        DestroyGameObjects();
        OnMainMenu();
    }

    public void RestartGame()
    {
        eventSystem.SetSelectedGameObject(null);

        DestroyGameObjects();
        onTheGame = false;
        StartGame();
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SaveData(data);
    }
}
