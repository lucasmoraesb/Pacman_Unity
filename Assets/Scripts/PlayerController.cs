using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MovementController movementController;
    public SpriteRenderer sprite;
    public Animator animator;
    public GameObject startNode;
    public GameManager gameManager;
    public bool isDead = false;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        movementController = GetComponent<MovementController>();
        startNode = movementController.currentNode;
    }

    public void Setup()
    {
        if(gameManager.currentMap == 0){
            startNode = GameObject.Find("NodeStartMap0");
        }
        else if(gameManager.currentMap == 1){
            startNode = GameObject.Find("NodeStartMap1");
        }
        else{
            startNode = GameObject.Find("NodeStartMap2");
        }

        animator.SetBool("dead", false);
        animator.SetBool("moving", false);
        movementController.currentNode = startNode;
        movementController.direction = "left";
        movementController.lastMovingDirection = "left";
        sprite.flipX = false;
        transform.position = new Vector2(startNode.transform.position.x + 0.2f, startNode.transform.position.y);
        animator.speed = 1;
    }

    public void Stop()
    {
        isDead = false;
        animator.speed = 0;
    }

    void Update()
    {
        if (!gameManager.gameIsRunning)
        {
            if (!isDead)
            {
                animator.speed = 0;
            }
            return;
        }

        animator.speed = 1;

        animator.SetBool("moving", true);
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movementController.SetDirection("up");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movementController.SetDirection("down");
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movementController.SetDirection("left");
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movementController.SetDirection("right");
        }


        bool flipX = false;
        bool flipY = false;
        if (movementController.lastMovingDirection == "left")
        {
            animator.SetInteger("direction", 0);
        }
        else if (movementController.lastMovingDirection == "right")
        {
            animator.SetInteger("direction", 0);
            flipX = true;
        }
        else if (movementController.lastMovingDirection == "up")
        {
            animator.SetInteger("direction", 1);
        }
        else if (movementController.lastMovingDirection == "down")
        {
            animator.SetInteger("direction", 1);
            flipY = true;
        }

        sprite.flipY = flipY;
        sprite.flipX = flipX;
    }

    public void Death()
    {
        isDead = true;
        animator.SetBool("moving", false);
        animator.speed = 1;
        animator.SetBool("dead", true);
    }
}
