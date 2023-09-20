using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum GhostNodeStatesEnum
    {
        respawning,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        movingInNodes
    }

    public GhostNodeStatesEnum ghostNodeState;
    public GhostNodeStatesEnum startGhostNodeState;
    public GhostNodeStatesEnum respawnState;

    public enum GhostType
    {
        red,
        pink,
        blue,
        orange
    }

    public GhostType ghostType;
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public MovementController movementController;

    public GameObject startingNode;

    public bool readyToLeaveHome = false;

    public GameManager gameManager;

    public bool testRespawn = false;
    public List<string> respawnPath = new();
    public bool isFrightened = false;

    public GameObject[] scatterNodes;
    public int scatterNodeIndex;

    public bool leftHomeBefore = false;

    public bool isVisible = true;

    public SpriteRenderer ghostSprite;
    public SpriteRenderer eyesSprite;

    public Animator animator;

    public Color color;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        ghostSprite = GetComponent<SpriteRenderer>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();
        if (ghostType == GhostType.red)
        {
            startGhostNodeState = GhostNodeStatesEnum.startNode;
            respawnState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeStart;
            ghostNodeState = startGhostNodeState;
        }
        else if (ghostType == GhostType.pink)
        {
            startGhostNodeState = GhostNodeStatesEnum.centerNode;
            respawnState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
            ghostNodeState = startGhostNodeState;
        }
        else if (ghostType == GhostType.blue)
        {
            startGhostNodeState = GhostNodeStatesEnum.leftNode;
            respawnState = GhostNodeStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
            ghostNodeState = startGhostNodeState;
        }
        else if (ghostType == GhostType.orange)
        {
            startGhostNodeState = GhostNodeStatesEnum.rightNode;
            respawnState = GhostNodeStatesEnum.rightNode;
            startingNode = ghostNodeRight;
            ghostNodeState = startGhostNodeState;
        }
    }

    public void Setup()
    {
        if(gameManager.currentMap == 0){
            if (ghostType == GhostType.red)
            {
                startingNode = GameObject.Find("Start");
            }
            else if (ghostType == GhostType.pink)
            {
                startingNode = GameObject.Find("Center");
            }
            else if (ghostType == GhostType.blue)
            {
                startingNode = GameObject.Find("Left");
            }
            else if (ghostType == GhostType.orange)
            {
                startingNode = GameObject.Find("Right");
            }
            ghostNodeStart = GameObject.Find("Start");
            ghostNodeCenter = GameObject.Find("Center");
            ghostNodeLeft = GameObject.Find("Left");
            ghostNodeRight = GameObject.Find("Right");
        }
        else if(gameManager.currentMap == 1){
            if (ghostType == GhostType.red)
            {
                startingNode = GameObject.Find("Start1");
            }
            else if (ghostType == GhostType.pink)
            {
                startingNode = GameObject.Find("Center1");
            }
            else if (ghostType == GhostType.blue)
            {
                startingNode = GameObject.Find("Left1");
            }
            else if (ghostType == GhostType.orange)
            {
                startingNode = GameObject.Find("Right1");
            }
            ghostNodeStart = GameObject.Find("Start1");
            ghostNodeCenter = GameObject.Find("Center1");
            ghostNodeLeft = GameObject.Find("Left1");
            ghostNodeRight = GameObject.Find("Right1");
        }
        else{
            if (ghostType == GhostType.red)
            {
                startingNode = GameObject.Find("Start2");
            }
            else if (ghostType == GhostType.pink)
            {
                startingNode = GameObject.Find("Center2");
            }
            else if (ghostType == GhostType.blue)
            {
                startingNode = GameObject.Find("Left2");
            }
            else if (ghostType == GhostType.orange)
            {
                startingNode = GameObject.Find("Right2");
            }
            ghostNodeStart = GameObject.Find("Start2");
            ghostNodeCenter = GameObject.Find("Center2");
            ghostNodeLeft = GameObject.Find("Left2");
            ghostNodeRight = GameObject.Find("Right2");
        }
        animator.SetBool("moving", false);

        ghostNodeState = startGhostNodeState;
        readyToLeaveHome = false;
        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;

        movementController.direction = "";
        movementController.lastMovingDirection = "";

        scatterNodeIndex = 0;

        isFrightened = false;

        leftHomeBefore = false;

        if (ghostType == GhostType.red)
        {
            readyToLeaveHome = true;
            leftHomeBefore = true;
        }
        else if (ghostType == GhostType.pink)
        {
            readyToLeaveHome = true;
        }
        SetVisible(true);
    }

    void Update()
    {
        if (ghostNodeState != GhostNodeStatesEnum.movingInNodes || !gameManager.isPowerPelletRunning)
        {
            isFrightened = false;
        }

        if (isVisible)
        {
            if (ghostNodeState != GhostNodeStatesEnum.respawning)
            {
                ghostSprite.enabled = true;
            }
            else
            {
                ghostSprite.enabled = false;
            }

            eyesSprite.enabled = true;
        }
        else
        {
            ghostSprite.enabled = false;
            eyesSprite.enabled = false;
        }

        if (isFrightened)
        {
            animator.SetBool("frightened", true);
            eyesSprite.enabled = false;
            ghostSprite.color = new Color(255, 255, 255, 255);
        }
        else
        {
            animator.SetBool("frightened", false);
            ghostSprite.color = color;
        }

        if (!gameManager.gameIsRunning)
        {
            return;
        }


        animator.SetBool("moving", true);

        if (testRespawn == true)
        {
            readyToLeaveHome = false;
            ghostNodeState = GhostNodeStatesEnum.respawning;
            testRespawn = false;
        }

        if (movementController.currentNode.GetComponent<NodeController>().isSideNode)
        {
            movementController.SetSpeed(1);
        }
        else
        {
            if (isFrightened)
            {
                movementController.SetSpeed(1);
            }
            else if (ghostNodeState == GhostNodeStatesEnum.respawning)
            {
                movementController.SetSpeed(7);
            }
            else
            {
                movementController.SetSpeed(2);
            }
        }
    }

    public void SetFrightened(bool newIsFrightened)
    {
        isFrightened = newIsFrightened;
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodeStatesEnum.movingInNodes)
        {
            leftHomeBefore = true;
            if (gameManager.currentGhostMode == GameManager.GhostMode.scatter)
            {
                DetermineGhostScatterModeDirection();
            }
            else if (isFrightened)
            {
                string direction = GetDistantDirection(gameManager.pacman.transform.position);
                movementController.SetDirection(direction);
            }
            else
            {
                // Determine next game node to go to
                if (GhostType.red == ghostType)
                {
                    DetermineRedGhostDirection();
                }
                else if (GhostType.pink == ghostType)
                {
                    DeterminePinkGhostDirection();
                }
                else if (GhostType.blue == ghostType)
                {
                    DetermineBlueGhostDirection();
                }
                else if (GhostType.orange == ghostType)
                {
                    DetermineOrangeGhostDirection();
                }
            }

        }
        else if (ghostNodeState == GhostNodeStatesEnum.respawning)
        {
            if (transform.position.x == ghostNodeStart.transform.position.x && transform.position.y == ghostNodeStart.transform.position.y)
            {
                movementController.SetDirection("down");
                respawnPath.Clear();
            }
            else if (transform.position.x == ghostNodeCenter.transform.position.x && transform.position.y == ghostNodeCenter.transform.position.y)
            {
                if (respawnState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = respawnState;
                }
                else if (respawnState == GhostNodeStatesEnum.leftNode)
                {
                    movementController.SetDirection("left");
                }
                else if (respawnState == GhostNodeStatesEnum.rightNode)
                {
                    movementController.SetDirection("right");
                }
            }
            else if (
                (transform.position.x == ghostNodeLeft.transform.position.x && transform.position.y == ghostNodeLeft.transform.position.y) ||
                (transform.position.x == ghostNodeRight.transform.position.x && transform.position.y == ghostNodeRight.transform.position.y)
                )
            {
                ghostNodeState = respawnState;
            }
            else
            {
                if(respawnPath.Count == 0)
                    AStarPath(ghostNodeStart.transform.position);
                movementController.SetDirection(respawnPath[0]);
                respawnPath.RemoveAt(0);
            }
        }
        else
        {
            // If we are ready to leave our home
            if (readyToLeaveHome)
            {
                //If we are in the left or right home node, move to the center
                if (ghostNodeState == GhostNodeStatesEnum.leftNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.SetDirection("right");
                }
                else if (ghostNodeState == GhostNodeStatesEnum.rightNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.SetDirection("left");
                }
                else if (ghostNodeState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.startNode;
                    movementController.SetDirection("up");
                }
                else if (ghostNodeState == GhostNodeStatesEnum.startNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.movingInNodes;
                    movementController.SetDirection("left");
                }
            }
        }
    }

    string GetDistantDirection(Vector2 target)
    {
        float distantDistance = 0;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";

        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        if (nodeController.canMoveUp && lastMovingDirection != "down")
        {
            GameObject nodeUp = nodeController.nodeUp;

            float distance = Vector2.Distance(nodeUp.transform.position, target);

            if (distance > distantDistance || distantDistance == 0)
            {
                distantDistance = distance;
                newDirection = "up";
            }
        }

        if (nodeController.canMoveDown && lastMovingDirection != "up")
        {
            GameObject nodeDown = nodeController.nodeDown;

            float distance = Vector2.Distance(nodeDown.transform.position, target);

            if (distance > distantDistance || distantDistance == 0)
            {
                distantDistance = distance;
                newDirection = "down";
            }
        }

        if (nodeController.canMoveLeft && lastMovingDirection != "right")
        {
            GameObject nodeLeft = nodeController.nodeLeft;

            float distance = Vector2.Distance(nodeLeft.transform.position, target);

            if (distance > distantDistance || distantDistance == 0)
            {
                distantDistance = distance;
                newDirection = "left";
            }
        }

        if (nodeController.canMoveRight && lastMovingDirection != "left")
        {
            GameObject nodeRight = nodeController.nodeRight;

            float distance = Vector2.Distance(nodeRight.transform.position, target);

            if (distance > distantDistance || distantDistance == 0)
            {
                distantDistance = distance;
                newDirection = "right";
            }
        }

        return newDirection;
    }

    void DetermineGhostScatterModeDirection()
    {
        if (transform.position.x == scatterNodes[scatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[scatterNodeIndex].transform.position.y)
        {
            scatterNodeIndex++;

            if (scatterNodeIndex == scatterNodes.Length - 1)
            {
                scatterNodeIndex = 0;
            }
        }

        string direction = GetClosestDirection(scatterNodes[scatterNodeIndex].transform.position);

        movementController.SetDirection(direction);
    }

    void DetermineRedGhostDirection()
    {
        string direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.SetDirection(direction);
    }


    void DeterminePinkGhostDirection()
    {
        string pacmanDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = 0.35f;

        Vector2 target = gameManager.pacman.transform.position;
        if (pacmanDirection == "left")
        {
            target.x -= distanceBetweenNodes * 2;
        }
        else if (pacmanDirection == "right")
        {
            target.x += distanceBetweenNodes * 2;
        }
        else if (pacmanDirection == "up")
        {
            target.y += distanceBetweenNodes * 2;
        }
        else if (pacmanDirection == "down")
        {
            target.y -= distanceBetweenNodes * 2;
        }
        string direction = GetClosestDirection(target);
        movementController.SetDirection(direction);
    }


    void DetermineBlueGhostDirection()
    {
        string pacmanDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = 0.35f;

        Vector2 target = gameManager.pacman.transform.position;
        if (pacmanDirection == "left")
        {
            target.x -= distanceBetweenNodes * 2;
        }
        else if (pacmanDirection == "right")
        {
            target.x += distanceBetweenNodes * 2;
        }
        else if (pacmanDirection == "up")
        {
            target.y += distanceBetweenNodes * 2;
        }
        else if (pacmanDirection == "down")
        {
            target.y -= distanceBetweenNodes * 2;
        }

        GameObject redGhost = gameManager.redGhost;
        float xDistance = target.x - redGhost.transform.position.x;
        float yDistance = target.y - redGhost.transform.position.y;

        Vector2 blueTarget = new Vector2(target.x + xDistance, target.y + yDistance);
        string direction = GetClosestDirection(blueTarget);
        movementController.SetDirection(direction);
    }

    void DetermineOrangeGhostDirection()
    {
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);
        float distanceBetweenNodes = 0.35f;

        if (distance < 0)
        {
            distance *= -1;
        }

        if (distance <= distanceBetweenNodes * 8)
        {
            DetermineRedGhostDirection();
        }
        else
        {
            DetermineGhostScatterModeDirection();
        }
    }

    string GetClosestDirection(Vector2 target)
    {
        float shortestDistance = 0;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";

        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        if (nodeController.canMoveUp && lastMovingDirection != "down")
        {
            GameObject nodeUp = nodeController.nodeUp;

            float distance = Vector2.Distance(nodeUp.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "up";
            }
        }

        if (nodeController.canMoveDown && lastMovingDirection != "up")
        {
            GameObject nodeDown = nodeController.nodeDown;

            float distance = Vector2.Distance(nodeDown.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "down";
            }
        }

        if (nodeController.canMoveLeft && lastMovingDirection != "right")
        {
            GameObject nodeLeft = nodeController.nodeLeft;

            float distance = Vector2.Distance(nodeLeft.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "left";
            }
        }

        if (nodeController.canMoveRight && lastMovingDirection != "left")
        {
            GameObject nodeRight = nodeController.nodeRight;

            float distance = Vector2.Distance(nodeRight.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "right";
            }
        }

        return newDirection;
    }

    string GetFurthestDirection(Vector2 target)
    {
        float shortestDistance = 0;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";

        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        if (nodeController.canMoveUp && lastMovingDirection != "down")
        {
            GameObject nodeUp = nodeController.nodeUp;

            float distance = Vector2.Distance(nodeUp.transform.position, target);

            if (distance > shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "up";
            }
        }

        if (nodeController.canMoveDown && lastMovingDirection != "up")
        {
            GameObject nodeDown = nodeController.nodeDown;

            float distance = Vector2.Distance(nodeDown.transform.position, target);

            if (distance > shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "down";
            }
        }

        if (nodeController.canMoveLeft && lastMovingDirection != "right")
        {
            GameObject nodeLeft = nodeController.nodeLeft;

            float distance = Vector2.Distance(nodeLeft.transform.position, target);

            if (distance > shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "left";
            }
        }

        if (nodeController.canMoveRight && lastMovingDirection != "left")
        {
            GameObject nodeRight = nodeController.nodeRight;

            float distance = Vector2.Distance(nodeRight.transform.position, target);

            if (distance > shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "right";
            }
        }

        return newDirection;
    }

    public void SetVisible(bool newIsVisible)
    {
        isVisible = newIsVisible;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && ghostNodeState != GhostNodeStatesEnum.respawning)
        {
            if (isFrightened)
            {
                gameManager.GhostEaten();
                ghostNodeState = GhostNodeStatesEnum.respawning;
            }
            else
            {
                StartCoroutine(gameManager.PlayerEaten());
            }
        }
    }

    public class Node
    {
        public GameObject node;
        public float hFunction;
        public float gFunction;
        public string direction;
        public Node father;
        public List<Node> neighbors;
    }

    public Node CreateNode(GameObject node, Node father, string direction)
    {
        Node newNode = new()
        {
            node = node,
            direction = direction,
            father = father,
            neighbors = new List<Node>()
        };
        return newNode;
    }

    public float CalculateHFunction(Node node, Vector2 target)
    {
        float hFunction = Vector2.Distance(node.node.transform.position, target);
        return hFunction;
    }

    public float CalculateGFunction(Node currentNode, Node node)
    {
        float gFunction = Vector2.Distance(currentNode.node.transform.position, node.node.transform.position);
        return gFunction + currentNode.gFunction;
    }

    public List<Node> FindLeafs(Node node)
    {
        List<Node> leafs = new();
        if (node.neighbors.Count == 0)
        {
            leafs.Add(node);
        }
        else
        {
            foreach (Node neighbor in node.neighbors)
            {
                leafs.AddRange(FindLeafs(neighbor));
            }
        }
        return leafs;
    }

    public Node ShortestLeaf(List<Node> leafs)
    {
        Node shortestLeaf = leafs[0];
        foreach (Node leaf in leafs)
        {
            if ((leaf.hFunction + leaf.gFunction) < (shortestLeaf.hFunction + shortestLeaf.gFunction))
            {
                shortestLeaf = leaf;
            }
        }
        return shortestLeaf;
    }

    public void AStarPath(Vector2 target)
    {
        List <string> path = new();

        Node startNode = CreateNode(movementController.currentNode, null, movementController.lastMovingDirection);
        startNode.hFunction = CalculateHFunction(startNode, target);
        startNode.gFunction = 0;

        Node currentNode = startNode;

        while(currentNode.hFunction != 0)
        {
            if(currentNode.node.GetComponent<NodeController>().canMoveUp && currentNode.direction != "down")
            {
                GameObject nodeUp = currentNode.node.GetComponent<NodeController>().nodeUp;
                Node newNode = CreateNode(nodeUp, currentNode, "up");
                newNode.hFunction = CalculateHFunction(newNode, target);
                newNode.gFunction = CalculateGFunction(currentNode, newNode);
                currentNode.neighbors.Add(newNode);
            }
            if (currentNode.node.GetComponent<NodeController>().canMoveDown && currentNode.direction != "up")
            {
                GameObject nodeDown = currentNode.node.GetComponent<NodeController>().nodeDown;
                Node newNode = CreateNode(nodeDown, currentNode, "down");
                newNode.hFunction = CalculateHFunction(newNode, target);
                newNode.gFunction = CalculateGFunction(currentNode, newNode);
                currentNode.neighbors.Add(newNode);
            }
            if (currentNode.node.GetComponent<NodeController>().canMoveLeft && currentNode.direction != "right")
            {
                GameObject nodeLeft = currentNode.node.GetComponent<NodeController>().nodeLeft;
                Node newNode = CreateNode(nodeLeft, currentNode, "left");
                newNode.hFunction = CalculateHFunction(newNode, target);
                newNode.gFunction = CalculateGFunction(currentNode, newNode);
                currentNode.neighbors.Add(newNode);
            }
            if (currentNode.node.GetComponent<NodeController>().canMoveRight && currentNode.direction != "left")
            {
                GameObject nodeRight = currentNode.node.GetComponent<NodeController>().nodeRight;
                Node newNode = CreateNode(nodeRight, currentNode, "right");
                newNode.hFunction = CalculateHFunction(newNode, target);
                newNode.gFunction = CalculateGFunction(currentNode, newNode);
                currentNode.neighbors.Add(newNode);
            }

            List<Node> leafs = FindLeafs(startNode);
            currentNode = ShortestLeaf(leafs);
        }

        while(currentNode.father != null)
        {
            path.Add(currentNode.direction);
            currentNode = currentNode.father;
        }

        path.Reverse();

        Debug.Log("Path encontrado");
        
        respawnPath = path;
    }    
}


