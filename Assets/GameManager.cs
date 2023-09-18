using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject leftWarpNode;
    public GameObject pacman;
    public GameObject rightWarpNode;

    public AudioSource munch1;
    public AudioSource munch2;
    public AudioSource powerPelletAudio;
    public AudioSource respawningAudio;
    public AudioSource ghostEatenAudio;

    public int currentMunch = 0;

    public int score;
    public Text scoreText;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;
    public GameObject redGhost;
    public GameObject pinkGhost;
    public GameObject blueGhost;
    public GameObject orangeGhost;

    public PlayerController pacmanController;
    public EnemyController redGhostController;
    public EnemyController pinkGhostController;
    public EnemyController blueGhostController;
    public EnemyController orangeGhostController;

    public int totalPellets;
    public int pelletsLeft;
    public int pelletsCollectedOnThisLife;

    public bool hadDeathOnThisLevel = false;

    public bool gameIsRunning;

    public List<NodeController> nodeControllers = new List<NodeController>();

    public bool newGame;
    public bool clearedLevel;

    public AudioSource startGameAudio;
    public AudioSource death;

    public int lives;
    public int currentLevel;

    public Image blackBackground;

    public Text gameOverText;
    public Text livesText;
    public bool isPowerPelletRunning = false;
    public float currentPowerPelletTime = 0;
    public float powerPelletTimer = 8f;
    public int powerPelletMultiplyer = 1;

    public enum GhostMode
    {
        chase, scatter
    }

    public GhostMode currentGhostMode;

    public int[] ghostModeTimers = new int[] { 7, 20, 7, 20, 5, 20, 5 };

    public int ghostModeTimerIndex;
    public float ghostModeTimer = 0;
    public bool runningTimer;
    public bool completedTimer;

    public List<GameObject> maps = new List<GameObject>();

    public int currentMap = 0;

    // Start is called before the first frame update
    void Awake()
    {
        newGame = true;
        clearedLevel = false;
        blackBackground.enabled = false;

        maps[0].SetActive(false);
        maps[1].SetActive(true);
        maps[2].SetActive(false);

        pacmanController = pacman.GetComponent<PlayerController>();
        redGhostController = redGhost.GetComponent<EnemyController>();
        pinkGhostController = pinkGhost.GetComponent<EnemyController>();
        blueGhostController = blueGhost.GetComponent<EnemyController>();
        orangeGhostController = orangeGhost.GetComponent<EnemyController>();

        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;

        pacman = GameObject.Find("Player");

        StartCoroutine(Setup());
    }

    public IEnumerator Setup()
    {
        ghostModeTimerIndex = 0;
        ghostModeTimer = 0;
        completedTimer = false;
        runningTimer = true;
        gameOverText.enabled = false;
        if (clearedLevel)
        {
            blackBackground.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        blackBackground.enabled = false;

        pelletsCollectedOnThisLife = 0;
        currentGhostMode = GhostMode.scatter;
        gameIsRunning = false;
        currentMunch = 0;

        float waitTimer = 1f;

        if (clearedLevel || newGame)
        {
            pelletsLeft = totalPellets;
            waitTimer = 4f;
        }

        if (newGame)
        {
            startGameAudio.Play();
            score = 0;
            scoreText.text = "Score: " + score.ToString();
            SetLives(3);
            currentLevel = 1;
        }

        pacmanController.Setup();
        redGhostController.Setup();
        pinkGhostController.Setup();
        blueGhostController.Setup();
        orangeGhostController.Setup();

        clearedLevel = false;
        yield return new WaitForSeconds(waitTimer);

        if(currentMap == 1){
            maps[0].SetActive(false);
            maps[1].SetActive(true);
            maps[2].SetActive(false);
        }
        else if(currentMap == 2){
            maps[0].SetActive(false);
            maps[1].SetActive(false);
            maps[2].SetActive(true);
        }
        
        if(newGame){
            for (int i = 0; i < nodeControllers.Count; i++)
            {
                nodeControllers[i].RespawnPellet();
            }
        }
        newGame = false;
        
        StartGame();
    }

    void SetLives(int newLives)
    {
        lives = newLives;
        livesText.text = "Lives: " + lives;
    }

    void StartGame()
    {
        gameIsRunning = true;
        // siren.Play();
    }

    void StopGame()
    {
        gameIsRunning = false;
        // siren.Stop();
        powerPelletAudio.Stop();
        pacman.GetComponent<PlayerController>().Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameIsRunning)
        {
            return;
        }

        if (redGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning
        || pinkGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning
        || blueGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning
        || orangeGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning)
        {
            if (!respawningAudio.isPlaying)
            {
                respawningAudio.Play();
            }
        }
        else
        {
            if (respawningAudio.isPlaying)
            {
                respawningAudio.Stop();
            }
        }

        if (!completedTimer && runningTimer)
        {
            ghostModeTimer += Time.deltaTime;
            if (ghostModeTimer >= ghostModeTimers[ghostModeTimerIndex])
            {
                ghostModeTimer = 0;
                ghostModeTimerIndex++;
                if (currentGhostMode == GhostMode.chase)
                {
                    currentGhostMode = GhostMode.scatter;
                }
                else
                {
                    currentGhostMode = GhostMode.chase;
                }

                if (ghostModeTimerIndex == ghostModeTimers.Length)
                {
                    completedTimer = true;
                    runningTimer = false;
                    currentGhostMode = GhostMode.chase;
                }
            }
        }

        if (isPowerPelletRunning)
        {
            currentPowerPelletTime += Time.deltaTime;
            if (currentPowerPelletTime >= powerPelletTimer)
            {
                isPowerPelletRunning = false;
                currentPowerPelletTime = 0;
                powerPelletAudio.Stop();
                powerPelletMultiplyer = 1;
            }
        }
    }

    public void GotPelletFromNodeController(NodeController nodeController)
    {
        nodeControllers.Add(nodeController);
        totalPellets++;
        pelletsLeft++;
    }

    public void AddToScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score.ToString();
    }

    public IEnumerator CollectedPellet(NodeController nodeController)
    {
        if (currentMunch == 0)
        {
            munch1.Play();
            currentMunch = 1;
        }
        else if (currentMunch == 1)
        {
            munch2.Play();
            currentMunch = 0;
        }

        pelletsLeft--;
        pelletsCollectedOnThisLife++;

        int requiredBluePellets = 0;
        int requiredOrangePellets = 0;

        if (hadDeathOnThisLevel)
        {
            requiredBluePellets = 12;
            requiredOrangePellets = 32;
        }
        else
        {
            requiredBluePellets = 30;
            requiredOrangePellets = 60;
        }

        if (pelletsCollectedOnThisLife >= requiredBluePellets && !blueGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            blueGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }

        if (pelletsCollectedOnThisLife >= requiredOrangePellets && !orangeGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            orangeGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }


        AddToScore(10);

        if (pelletsLeft == 0)
        {
            newGame = true;
            currentMap++;
            currentLevel++;
            clearedLevel = true;
            StopGame();
            yield return new WaitForSeconds(1);
            StartCoroutine(Setup());
        }


        if (nodeController.isPowerPellet)
        {
            //siren.Stop();
            powerPelletAudio.Play();
            isPowerPelletRunning = true;
            currentPowerPelletTime = 0;

            redGhostController.SetFrightened(true);
            pinkGhostController.SetFrightened(true);
            blueGhostController.SetFrightened(true);
            orangeGhostController.SetFrightened(true);
        }
    }

    public IEnumerator PauseGame(float timeToPause)
    {
        gameIsRunning = false;
        yield return new WaitForSeconds(timeToPause);
        gameIsRunning = true;
    }
    public void GhostEaten()
    {
        ghostEatenAudio.Play();
        AddToScore(400 * powerPelletMultiplyer);
        powerPelletMultiplyer++;
        StartCoroutine(PauseGame(1));
    }

    public IEnumerator PlayerEaten()
    {
        hadDeathOnThisLevel = true;
        StopGame();
        yield return new WaitForSeconds(1);

        redGhostController.SetVisible(false);
        pinkGhostController.SetVisible(false);
        blueGhostController.SetVisible(false);
        orangeGhostController.SetVisible(false);

        pacman.GetComponent<PlayerController>().Death();
        death.Play();
        yield return new WaitForSeconds(3);
        SetLives(lives - 1);
        if (lives <= 0)
        {
            newGame = true;
            gameOverText.enabled = true;
            yield return new WaitForSeconds(3);
        }
        StartCoroutine(Setup());
    }
}
