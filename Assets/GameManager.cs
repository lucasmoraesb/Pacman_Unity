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

    public int lives;
    public int currentLevel;

    public Image blackBackground;

    public enum GhostMode
    {
        chase, scatter
    }

    public GhostMode currentGhostMode;

    // Start is called before the first frame update
    void Awake()
    {
        newGame = true;
        clearedLevel = false;
        blackBackground.enabled = false;

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
            for (int i = 0; i < nodeControllers.Count; i++)
            {
                nodeControllers[i].RespawnPellet();
            }
        }

        if (newGame)
        {
            startGameAudio.Play();
            score = 0;
            scoreText.text = "Score: " + score.ToString();
            lives = 3;
            currentLevel = 1;
        }

        pacmanController.Setup();
        redGhostController.Setup();
        pinkGhostController.Setup();
        blueGhostController.Setup();
        orangeGhostController.Setup();

        newGame = false;
        clearedLevel = false;
        yield return new WaitForSeconds(waitTimer);

        StartGame();
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
        pacman.GetComponent<PlayerController>().Stop();
    }

    // Update is called once per frame
    void Update()
    {

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
            currentLevel++;
            clearedLevel = true;
            StopGame();
            yield return new WaitForSeconds(1);
            StartCoroutine(Setup());
        }
    }
}
