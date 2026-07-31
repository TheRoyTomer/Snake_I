using UnityEngine;
using TMPro;
using System.Collections;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private bool isGameOver = false;
    private int score = 0;

    private Coroutine gameOverBlinkCoroutine;

    private void Start()
    {
        UpdateScoreText();
        gameOverText.gameObject.SetActive(false);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
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
}