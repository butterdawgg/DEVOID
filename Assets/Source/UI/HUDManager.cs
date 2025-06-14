using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class HUDManager : MonoBehaviour
{
    [SerializeField] private GameObject mainWindow;
    [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject loseWindow;

    public static HUDManager Instance { get; private set; }
    public static bool IsPaused { get { return Instance.isPaused; } }
    private bool isPaused;

    private bool isPlayerDead;

    private Canvas canvas;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        isPaused = false;
        isPlayerDead = false;

        loseWindow.SetActive(false);
    }

    void Start()
    {
        canvas = GetComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraController.Instance.Camera;
        canvas.planeDistance = 1f;
    }

    void Update()
    {
        if (isPlayerDead)
        {
            isPaused = true;
            Time.timeScale = 0f;
            Cursor.visible = true;
            mainWindow.SetActive(false);
            pauseWindow.SetActive(false);
            return;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
            isPaused = !isPaused;

        Time.timeScale = IsPaused ? 0f : 1f;
        Cursor.visible = IsPaused;
        mainWindow.SetActive(!IsPaused);
        pauseWindow.SetActive(IsPaused);
    }

    public void OnPlayerDeath()
    {
        isPlayerDead = true;

        loseWindow.SetActive(true);
    }
}