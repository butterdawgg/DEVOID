using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassSelectionMenuManager : MonoBehaviour
{
    [SerializeField] private Button[] classSelectionButtons;
    [SerializeField] private GameObject howToPlayMenu;
    [SerializeField] private GameObject introMenu;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject defaultDescriptionText;
    [SerializeField] private GameObject firstPlayDescriptionText;

    private void Awake()
    {
        foreach (var button in classSelectionButtons)
        {
            button.onClick.AddListener(OnClassSelectionButtonClick);
        }

        nextButton.onClick.AddListener(OnNextButtonClick);

        bool isFirstPlay = SerializeManager.GetIsFirstPlay();

        defaultDescriptionText.SetActive(!isFirstPlay);
        firstPlayDescriptionText.SetActive(isFirstPlay);

        howToPlayMenu.SetActive(false);
        introMenu.SetActive(false);
    }

    private void OnClassSelectionButtonClick()
    {
        if (SerializeManager.GetIsFirstPlay())
        {
            howToPlayMenu.SetActive(true);
            introMenu.SetActive(false);

            return;
        }

        howToPlayMenu.SetActive(false);
        introMenu.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnNextButtonClick()
    {
        howToPlayMenu.SetActive(false);
        introMenu.SetActive(true);
    }
}