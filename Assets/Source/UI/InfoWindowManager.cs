using UnityEngine;
using UnityEngine.UI;

public class InfoWindowManager : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
    }

    private void OnBackButtonClick()
    {
        gameObject.SetActive(false);
    }
}