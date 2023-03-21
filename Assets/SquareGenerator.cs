using UnityEngine;
using TMPro;

public class SquareGenerator : MonoBehaviour
{
    public GameObject squarePrefab;
    public int numSquares = 2;
    public float minX = -2;
    public float maxX = 2f;
    public float minY = -4f;
    public float maxY = 4f;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    private int score;
    private bool gameStarted;
    private float startTime;

    void Start()
    {
        score = 0;
        scoreText.text = "Score: " + score;
        timerText.text = "Time: 10s";

        gameStarted = false;
        startTime = 0f;

        for (int i = 0; i < numSquares; i++)
        {
            // Generate random position for the square
            Vector3 pos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);

            // Check if the square overlaps with any existing squares
            bool overlap = true;
            while (overlap)
            {
                overlap = false;
                foreach (Transform child in transform)
                {
                    if (Vector3.Distance(pos, child.position) < squarePrefab.transform.localScale.x)
                    {
                        overlap = true;
                        pos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
                        break;
                    }
                }
            }

            // Create a new square at the non-overlapping position
            GameObject square = Instantiate(squarePrefab, pos, Quaternion.identity);
            square.transform.parent = transform;

            // Add touch input handler to the square
            TouchInputHandler touchHandler = square.AddComponent<TouchInputHandler>();
            touchHandler.minX = minX;
            touchHandler.maxX = maxX;
            touchHandler.minY = minY;
            touchHandler.maxY = maxY;
            touchHandler.scoreManager = this;
            touchHandler.StartTimer = StartTimer;
        }
    }

    void Update()
    {
        if (gameStarted)
        {
            // Calculate elapsed time
            float elapsedTime = Time.time - startTime;

            // Update timer text
            int remainingTime = Mathf.RoundToInt(10f - elapsedTime);
            if (remainingTime < 0) remainingTime = 0;
            timerText.text = "Time: " + remainingTime + "s";

            // End the game when time is up
            if (remainingTime == 0)
            {
                EndGame();
            }
        }
    }

    public void StartTimer()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            startTime = Time.time;
        }
    }

    public void IncrementScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

    private void EndGame()
    {
        // Disable all squares
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // Display final score
        scoreText.text = "Final Score: " + score;
    }
}

public class TouchInputHandler : MonoBehaviour
{
    public float minX = -2f;
    public float maxX = 2f;
    public float minY = -4f;
    public float maxY = 4f;
    public SquareGenerator scoreManager;
    public System.Action StartTimer;

    void Update()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if touch is inside the square
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchPos.z = 0;
                if (GetComponent<Collider2D>().OverlapPoint(touchPos))
                {
                    // Move the square to a random position
                    Vector3 newPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
                    transform.position = newPos;

                    // Increment the score and update the score text
                    scoreManager.IncrementScore();

                    // Start the timer if it hasn't started yet
                    if (StartTimer != null)
                    {
                        StartTimer();
                    }
                }   
            }
        }   
    }
}