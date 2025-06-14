using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassSelectionMenuManager : MonoBehaviour
{
    [SerializeField] private Button[] classSelectionButtons;
    [SerializeField] private GameObject howToPlayMenu;
    [SerializeField] private Button playConfirmButton;
    [SerializeField] private GameObject defaultDescriptionText;
    [SerializeField] private GameObject firstPlayDescriptionText;

    private void Awake()
    {
        foreach (var button in classSelectionButtons)
        {
            button.onClick.AddListener(OnClassSelectionButtonClick);
        }

        playConfirmButton.onClick.AddListener(OnPlayConfirmButtonClick);

        bool isFirstPlay = SerializeManager.GetIsFirstPlay();

        defaultDescriptionText.SetActive(!isFirstPlay);
        firstPlayDescriptionText.SetActive(isFirstPlay);

        howToPlayMenu.SetActive(false);
    }

    private void OnClassSelectionButtonClick()
    {
        if (SerializeManager.GetIsFirstPlay())
        {
            howToPlayMenu.SetActive(true);
            return;
        }

        howToPlayMenu.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnPlayConfirmButtonClick()
    {
        SerializeManager.SetIsFirstPlay(false);
        SceneManager.LoadScene(1);
    }
}