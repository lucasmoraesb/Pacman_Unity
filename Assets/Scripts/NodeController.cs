using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public int map;
    public bool canMoveLeft = false;
    public bool canMoveRight = false;
    public bool canMoveUp = false;
    public bool canMoveDown = false;

    public GameObject nodeLeft;
    public GameObject nodeRight;
    public GameObject nodeUp;
    public GameObject nodeDown;

    public bool isWarpRightNode = false;
    public bool isWarpLeftNode = false;
    public bool isPelletNode = false;
    public bool hasPellet = false;
    public bool isGhostStartingNode = false;

    public SpriteRenderer pelletSprite;
    public GameManager gameManager;

    public bool isSideNode = false;

    public bool isPowerPellet = false;

    public float powerPelletBlinkingTimer = 0;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (transform.childCount > 0)
        {
            gameManager.GotPelletFromNodeController(this);
            hasPellet = false;
            isPelletNode = true;
            pelletSprite = GetComponentInChildren<SpriteRenderer>();
            pelletSprite.enabled = false;
        }

        string tag = this.tag;

        RaycastHit2D[] hitsDown;
        hitsDown = Physics2D.RaycastAll(transform.position, -Vector2.up);

        for (int i = 0; i < hitsDown.Length; i++)
        {
            float distance = Mathf.Abs(hitsDown[i].point.y - transform.position.y);
            if (distance < 0.4f && hitsDown[i].collider.CompareTag(tag))
            {
                canMoveDown = true;
                nodeDown = hitsDown[i].collider.gameObject;
            }
        }


        RaycastHit2D[] hitsUp;
        hitsUp = Physics2D.RaycastAll(transform.position, Vector2.up);

        for (int i = 0; i < hitsUp.Length; i++)
        {
            float distance = Mathf.Abs(hitsUp[i].point.y - transform.position.y);
            if (distance < 0.4f && hitsUp[i].collider.CompareTag(tag))
            {
                canMoveUp = true;
                nodeUp = hitsUp[i].collider.gameObject;
            }
        }


        RaycastHit2D[] hitsRight;
        hitsRight = Physics2D.RaycastAll(transform.position, Vector2.right);

        for (int i = 0; i < hitsRight.Length; i++)
        {
            float distance = Mathf.Abs(hitsRight[i].point.x - transform.position.x);
            if (distance < 0.4f && hitsRight[i].collider.CompareTag(tag))
            {
                canMoveRight = true;
                nodeRight = hitsRight[i].collider.gameObject;
            }
        }


        RaycastHit2D[] hitsLeft;
        hitsLeft = Physics2D.RaycastAll(transform.position, -Vector2.right);

        for (int i = 0; i < hitsLeft.Length; i++)
        {
            float distance = Mathf.Abs(hitsLeft[i].point.x - transform.position.x);
            if (distance < 0.4f && hitsLeft[i].collider.CompareTag(tag))
            {
                canMoveLeft = true;
                nodeLeft = hitsLeft[i].collider.gameObject;
            }
        }

        if (isGhostStartingNode)
        {
            canMoveDown = true;
            if(this.CompareTag("Node"))
            {
                nodeDown = GameObject.Find("Center");
            }
            else if(this.CompareTag("Node2"))
            {
                nodeDown = GameObject.Find("Center1");
            }
            else if(this.CompareTag("Node3"))
            {
                nodeDown = GameObject.Find("Center2");
            }
        }
    }

    void Update()
    {
        if (!gameManager.gameIsRunning)
        {
            return;
        }

        if (isPowerPellet && hasPellet)
        {
            powerPelletBlinkingTimer += Time.deltaTime;
            if (powerPelletBlinkingTimer >= 0.1f)
            {
                powerPelletBlinkingTimer = 0;
                pelletSprite.enabled = !pelletSprite.enabled;
            }
        }
    }

    public GameObject GetNodeFromDirection(string direction)
    {
        if (direction == "left" && canMoveLeft)
        {
            return nodeLeft;
        }
        else if (direction == "right" && canMoveRight)
        {
            return nodeRight;
        }
        else if (direction == "up" && canMoveUp)
        {
            return nodeUp;
        }
        else if (direction == "down" && canMoveDown)
        {
            return nodeDown;
        }
        else
        {
            return null;
        }
    }

    public void RespawnPellet()
    {
        if (isPelletNode)
        {
            hasPellet = true;
            pelletSprite.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && hasPellet)
        {
            hasPellet = false;
            pelletSprite.enabled = false;
            StartCoroutine(gameManager.CollectedPellet(this));
        }
    }
}
