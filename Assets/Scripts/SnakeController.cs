using UnityEngine;
using System.Collections.Generic;
public class SnakeController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float timeToMove = 0.5f;
    [SerializeField] private float speedIncrease = 0.03f;
    [SerializeField] private float minimumTimeToMove = 0.15f;
    [SerializeField] private GameObject fruit;

    [SerializeField] private int width = 20;
    [SerializeField] private int height = 15;
    
    [SerializeField] private GameManagerScript  gameManager;
    
    [SerializeField] private int startingBodyParts = 2;

    
    private List<Transform> bodyParts = new List<Transform>();
    
    private float startingTimeToMove;
    private Vector3 startPosition =  Vector3.zero;
    private Vector3 currentDirection = Vector3.zero;
    private Vector3 nextDirection = Vector3.zero;
    private Vector3 lastTailPosition = Vector3.zero;
    private float moveTimerCounter = 0f;
    
    [SerializeField] private GameObject bodyPartPrefab;
    

    private void Awake()
    {
        startingTimeToMove = timeToMove;
        GameReset();
        fruit.GetComponent<SpriteRenderer>().enabled = true;
    }
    
    private void Update()
    {
        if (gameManager.IsGameOver())
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameReset();
                gameManager.RestartGame();
            }

            return;
        }
        
        ReadInput();

        moveTimerCounter += Time.deltaTime;

        if (moveTimerCounter >= timeToMove)
        {
            ValidateDirection();

            if (currentDirection != Vector3.zero)
            {
                Move();
            }

            moveTimerCounter -= timeToMove;
        }
    }

    private void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            nextDirection = Vector3.up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            nextDirection = Vector3.down; 
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            nextDirection = Vector3.left; 
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            nextDirection = Vector3.right; 
        }
    }
    
    private void ValidateDirection()
    {
        if (nextDirection == Vector3.zero)
        {
            return;
        }
        
        if (currentDirection == Vector3.zero &&
            nextDirection == Vector3.up)
        {
            return;
        }

        if (nextDirection == Vector3.up &&
            currentDirection != Vector3.down)
        {
            currentDirection = nextDirection;
        }
        else if (nextDirection == Vector3.down &&
                 currentDirection != Vector3.up)
        {
            currentDirection = nextDirection;
        }
        else if (nextDirection == Vector3.left &&
                 currentDirection != Vector3.right)
        {
            currentDirection = nextDirection;
        }
        else if (nextDirection == Vector3.right &&
                 currentDirection != Vector3.left)
        {
            currentDirection = nextDirection;
        }
    }
    
    private void Move()
    {
        Vector3 nextPosition = transform.position + currentDirection;

        if (!IsInsideBoard(nextPosition) || IsPositionOnBody(nextPosition))
        {
            gameManager.GameOver();
            return;
        }

        Vector3 previousPosition = transform.position;

        transform.position = nextPosition;

        MoveBody(previousPosition);

        if (IsSameGridPosition(transform.position, fruit.transform.position))
        {
            EatFruit();
        }
    }
    
    private void EatFruit()
    {
        gameManager.AddScore(100);

        AddBodyPart();

        timeToMove = Mathf.Max(
            minimumTimeToMove,
            timeToMove - speedIncrease
        );

        fruit.transform.position = GetRandomFruitPosition();
    }
    
    private void GameReset()
    {
        transform.position = startPosition;

        currentDirection = Vector3.zero;
        nextDirection = Vector3.zero;
        moveTimerCounter = 0f;
        lastTailPosition = startPosition;
        timeToMove = startingTimeToMove;
        
        foreach (Transform bodyPart in bodyParts)
        {
            Destroy(bodyPart.gameObject);
        }

        bodyParts.Clear();

        CreateStartingBody();
        
        gameManager.ResetScore();
        
        fruit.transform.position = GetRandomFruitPosition();
    }
    
    private bool IsInsideBoard(Vector3 position)
    {
        int minX = -width / 2;
        int maxX = width / 2 - 1;

        int minY = -height / 2;
        int maxY = height / 2;

        return position.x >= minX &&
               position.x <= maxX &&
               position.y >= minY &&
               position.y <= maxY;
    }
    
    private void AddBodyPart()
    {
        GameObject newBodyPart = Instantiate(bodyPartPrefab);
        newBodyPart.transform.position = lastTailPosition;
        bodyParts.Add(newBodyPart.transform);
    }
    
    private void MoveBody(Vector3 previousHeadPosition)
    {
        Vector3 positionToPass = previousHeadPosition;

        for (int i = 0; i < bodyParts.Count; i++)
        {
            Vector3 previousBodyPosition = bodyParts[i].position;

            bodyParts[i].position = positionToPass;

            positionToPass = previousBodyPosition;
        }
        lastTailPosition = positionToPass;
    }
    
    private bool IsPositionOnBody(Vector3 position)
    {
        foreach (Transform bodyPart in bodyParts)
        {
            if (IsSameGridPosition(position, bodyPart.position))
            {
                return true;
            }
        }

        return false;
    }
    
    private bool IsSameGridPosition(Vector3 firstPosition, Vector3 secondPosition)
    {
        return Mathf.Approximately(firstPosition.x, secondPosition.x) &&
               Mathf.Approximately(firstPosition.y, secondPosition.y);
    }
    
    private bool IsTooCloseToHead(Vector3 position)
    {
        Vector3 headPosition = transform.position;

        Vector3 frontPosition = headPosition + currentDirection;

        Vector3 leftDirection = new Vector3(
            -currentDirection.y,
            currentDirection.x,
            0
        );

        Vector3 rightDirection = new Vector3(
            currentDirection.y,
            -currentDirection.x,
            0
        );

        Vector3 leftPosition = headPosition + leftDirection;
        Vector3 rightPosition = headPosition + rightDirection;

        return IsSameGridPosition(position, frontPosition) ||
               IsSameGridPosition(position, leftPosition) ||
               IsSameGridPosition(position, rightPosition);
    }
    
    private Vector3 GetRandomFruitPosition()
    {
        Vector3 randomPosition;

        do
        {
            randomPosition = new Vector3(
                Random.Range(-width / 2 + 1, width / 2 - 1),
                Random.Range(-height / 2 + 1, height / 2),
                1
            );
        }
        while (IsSameGridPosition(randomPosition, transform.position) ||
               IsPositionOnBody(randomPosition) ||
               IsTooCloseToHead(randomPosition));

        return randomPosition;
    }
    
    private void CreateStartingBody()
    {
        for (int i = 1; i <= startingBodyParts; i++)
        {
            GameObject newBodyPart = Instantiate(bodyPartPrefab);

            newBodyPart.transform.position =
                startPosition + Vector3.up * i;

            bodyParts.Add(newBodyPart.transform);
        }

        lastTailPosition =
            startPosition + Vector3.up * startingBodyParts;
    }
    
}