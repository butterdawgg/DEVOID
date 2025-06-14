using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseWindowManager : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Awake()
    {
        restartButton.onClick.AddListener(OnRestartButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void Update()
    {
        scoreText.text = "points earned: " + Player.Instance.Score;
        highScoreText.text = "high score: " + SerializeManager.GetHighScore();
    }

    private void OnRestartButtonClick()
    {
        SceneManager.LoadScene(1);
    }

    private void OnExitButtonClick()
    {
        SceneManager.LoadScene(0);
    }
}