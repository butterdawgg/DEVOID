using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Windows")]
    [SerializeField] private GameObject classSelectionWindow;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject exitWindow;
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button classSelectionButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI mainMenuText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Awake()
    {
        classSelectionWindow.SetActive(false);
        settingsWindow.SetActive(false);
        exitWindow.SetActive(false);

        playButton.onClick.AddListener(OnPlayButtonClick);
        classSelectionButton.onClick.AddListener(OnClassSelectionButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);

        if (SerializeManager.GetIsFirstPlay())
        {
            classSelectionButton.GetComponent<MenuButton>().IsSelectable = false;
            classSelectionButton.enabled = false;
        }
        else
        {
            classSelectionButton.GetComponent<MenuButton>().IsSelectable = true;
            classSelectionButton.enabled = true;
        }

        Time.timeScale = 1.0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        highScoreText.text = "high score: " + SerializeManager.GetHighScore();
    }

    private void Update()
    {
        mainMenuText.rectTransform.localScale = Vector3.one +
            (new Vector3(Mathf.Sin(Time.time * 1.5f), Mathf.Sin(Time.time * 1.5f),
            Mathf.Sin(Time.time * 1.5f)) * 0.05f);
    }

    private void OnPlayButtonClick()
    {
        if (SerializeManager.GetIsFirstPlay())
        {
            OnClassSelectionButtonClick();
        }
        else
        {
            SerializeManager.SetIsFirstPlay(false);
            SceneManager.LoadScene(1);
        }
    }

    private void OnClassSelectionButtonClick()
    {
        classSelectionWindow.SetActive(true);
    }

    private void OnSettingsButtonClick()
    {
        settingsWindow.SetActive(true);
    }

    private void OnExitButtonClick()
    {
        exitWindow.SetActive(true);
    }
}