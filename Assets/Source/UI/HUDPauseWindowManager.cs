using UnityEngine;
using UnityEngine.UI;

public class HUDPauseWindowManager : MonoBehaviour
{
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject controlsWindow;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject exitWindow;

    private void Awake()
    {
        controlsWindow.SetActive(false);
        settingsWindow.SetActive(false);
        exitWindow.SetActive(false);

        controlsButton.onClick.AddListener(OnControlsButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDisable()
    {
        controlsWindow.SetActive(false);
        settingsWindow.SetActive(false);
        exitWindow.SetActive(false);
    }

    private void OnControlsButtonClick()
    {
        controlsWindow.SetActive(true);
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