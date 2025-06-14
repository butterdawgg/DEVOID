using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeSliderValueChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderValueChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeSliderValueChanged);
        backButton.onClick.AddListener(OnBackButtonClick);

        masterVolumeSlider.value = SerializeManager.GetMasterVolume();
        sfxVolumeSlider.value = SerializeManager.GetSFXVolume();
        musicVolumeSlider.value = SerializeManager.GetMusicVolume();
    }

    private void OnMasterVolumeSliderValueChanged(float value)
    {
        SerializeManager.SetMasterVolume(value);
    }

    private void OnSfxVolumeSliderValueChanged(float value)
    {
        SerializeManager.SetSFXVolume(value);
    }

    private void OnMusicVolumeSliderValueChanged(float value)
    {
        SerializeManager.SetMusicVolume(value);
    }

    private void OnBackButtonClick()
    {
        gameObject.SetActive(false);
    }
}