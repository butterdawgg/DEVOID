using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitMenuManager : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backButton;
    [SerializeField] private int sceneLoadId;

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
    }

    private void OnConfirmButtonClick()
    {
        if (sceneLoadId == -1)
        {
            Application.Quit();
            return;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(sceneLoadId);
    }

    private void OnBackButtonClick()
    {
        gameObject.SetActive(false);
    }
}