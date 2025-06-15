using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct IntroLine
{
    [TextArea]
    public string text;
    public float pauseTime;
}

public class IntroWindowManager : MonoBehaviour
{
    [SerializeField] private Button skipButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private IntroLine[] introLines;

    [SerializeField] private TextMeshProUGUI introText;

    private bool isIntroOver = false;

    private void Awake()
    {
        skipButton.onClick.AddListener(OnSkipButtonClick);
        startGameButton.onClick.AddListener(OnStartGameButtonClick);
    }

    private void OnEnable()
    {
        introText.text = "";

        skipButton.gameObject.SetActive(true);
        startGameButton.gameObject.SetActive(false);

        StartCoroutine(IntroCoroutine());
    }

    private void OnSkipButtonClick()
    {
        isIntroOver = true;
        string text = "";

        foreach (var line in introLines)
        {
            text += line.text;
        }

        introText.text = text;

        StopCoroutine(IntroCoroutine());
        skipButton.gameObject.SetActive(false);
        startGameButton.gameObject.SetActive(true);
    }

    private void OnStartGameButtonClick()
    {
        SerializeManager.SetIsFirstPlay(false);
        SceneManager.LoadScene(1);
    }

    private IEnumerator IntroCoroutine()
    {
        if (isIntroOver)
            yield return null;

        foreach (var line in introLines)
        {
            char[] characters = line.text.ToCharArray();

            for (int i = 0; i < characters.Length; i++)
            {
                if (isIntroOver)
                    break;

                introText.text += characters[i];

                AudioManager.Instance.PlaySound("Text1");

                yield return new WaitForSeconds(0.05f);
            }

            if (!isIntroOver)
                yield return new WaitForSeconds(line.pauseTime);
        }

        isIntroOver = true;

        skipButton.gameObject.SetActive(false);
        startGameButton.gameObject.SetActive(true);
    }
}