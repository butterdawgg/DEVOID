using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDMainWindowManager : MonoBehaviour
{
    [SerializeField] private Image crosshair;
    [SerializeField] private Image cursorDot;
    [SerializeField] private Image[] decorativeDots;
    [SerializeField] private Slider thrustMeter;
    [SerializeField] private Slider healthMeter;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image targetDescription;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Image targetHealthBar;

    private Canvas canvas;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (HUDManager.IsPaused)
            return;

        Vector3 crossPosNDC = InputManager.GetCrosshairPosBottomLeftNDC();
        Vector3 cursorPosNDC = InputManager.GetCursorPosBottomLeftNDC();

        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;

        Vector3 crossPos = new Vector3(crossPosNDC.x * canvasSize.x,
            crossPosNDC.y * canvasSize.y, 0f);

        Vector3 cursorPos = new Vector3(cursorPosNDC.x * canvasSize.x,
            cursorPosNDC.y * canvasSize.y, 0f);

        Vector3 direction = cursorPos - crossPos;
        float interval = direction.magnitude / (decorativeDots.Length + 1);
        direction.Normalize();

        crosshair.rectTransform.anchoredPosition = crossPos;
        cursorDot.rectTransform.anchoredPosition = cursorPos;

        for (int i = 0; i < decorativeDots.Length; i++)
        {
            Vector3 dotPos = crossPos + direction * (interval * (i + 1));

            decorativeDots[i].rectTransform.anchoredPosition = dotPos;
        }

        thrustMeter.value = Player.Instance.ThrustFraction;
        healthMeter.value = Player.Instance.HealthFraction;

        scoreText.text = Player.Instance.Score.ToString();

        if (Player.Instance.Target == null)
        {
            targetText.gameObject.SetActive(false);
            targetHealthBar.gameObject.SetActive(false);
            targetDescription.gameObject.SetActive(false);
        }
        else
        {
            targetText.gameObject.SetActive(true);
            targetHealthBar.gameObject.SetActive(true);
            targetDescription.gameObject.SetActive(true);

            targetText.text = Player.Instance.Target.Name;
            targetHealthBar.rectTransform.localScale =
                new Vector3(Player.Instance.Target.HealthFraction, 1f, 1f);
        }
    }
}