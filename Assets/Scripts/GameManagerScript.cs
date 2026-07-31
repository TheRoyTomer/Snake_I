using UnityEngine;
using TMPro;
using System.Collections;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI highScoreText;    
    [SerializeField] private TextMeshProUGUI pauseText;
    
    private bool isGameOver = false;
    private int score = 0;
    private int highScore = 0;
    private bool isPaused = false;
    private Coroutine gameOverBlinkCoroutine;

    private void Start()
    {
        UpdateScoreText();
        UpdateHighScoreText();
        gameOverText.gameObject.SetActive(false);
        pauseText.gameObject.SetActive(false);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();

        if (score > highScore)
        {
            highScore = score;
            UpdateHighScoreText();
        }
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverText.gameObject.SetActive(true);

        gameOverBlinkCoroutine = StartCoroutine(BlinkGameOverText());
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseText.gameObject.SetActive(false);
        isGameOver = false;

        if (gameOverBlinkCoroutine != null)
        {
            StopCoroutine(gameOverBlinkCoroutine);
            gameOverBlinkCoroutine = null;
        }

        gameOverText.enabled = true;
        gameOverText.gameObject.SetActive(false);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    private IEnumerator BlinkGameOverText()
    {
        while (isGameOver)
        {
            gameOverText.enabled = !gameOverText.enabled;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Score: {score}";
    }
    
    private void UpdateHighScoreText()
    {
        highScoreText.text = $"High Score: {highScore}";
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseText.gameObject.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    public bool IsPaused()
    {
        return isPaused;
    }
}