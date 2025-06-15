using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTracker
{
    public Enemy enemy;
    public Image tracker;
}

public class EnemyTrackerManager : MonoBehaviour
{
    [SerializeField] private RectTransform trackerPivot;
    [SerializeField] private float circleRadius;
    [SerializeField] private float verticalOffset;
    [SerializeField] private Image trackerPrefab;

    public static EnemyTrackerManager Instance { get; private set; }

    private List<EnemyTracker> enemyTrackers = new ();

    private Canvas canvas;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        List<EnemyTracker> toRemove = new List<EnemyTracker>();

        foreach (var enemyTracker in enemyTrackers)
        {
            if (enemyTracker.enemy.IsDead || !enemyTracker.enemy.IsAggroed)
            {
                toRemove.Add(enemyTracker);
            }
        }

        foreach (var enemyTracker in toRemove)
        {
            Destroy(enemyTracker.tracker);

            enemyTrackers.Remove(enemyTracker);
        }

        float radius =
            (Screen.width >= Screen.height ? Screen.height : Screen.width) *
            circleRadius * 0.5f;

        foreach (var enemyTracker in enemyTrackers)
        {
            bool behindCamera =
                Vector3.Dot(
                    enemyTracker.enemy.transform.position -
                    CameraController.Instance.Camera.transform.position,
                    CameraController.Instance.Camera.transform.forward) < 0f;

            Vector3 pos =
                CameraController.Instance.Camera.WorldToScreenPoint(
                    enemyTracker.enemy.transform.position) -
                    new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

            pos.z = 0f;

            Image tracker = enemyTracker.tracker;

            Vector2 canvasSize =
                canvas.GetComponent<RectTransform>().sizeDelta;

            if (behindCamera)
            {
                pos.x = -pos.x;
                pos.y = -pos.y;

                pos = pos.normalized * radius;
            }
            else
            {
                pos = Vector3.ClampMagnitude(pos, radius);
            }

            pos.x += Screen.width * 0.5f;
            pos.y += Screen.height * 0.5f;

            pos.x /= Screen.width;
            pos.y /= Screen.height;

            tracker.rectTransform.anchoredPosition =
                new Vector2(pos.x * canvasSize.x,
                pos.y * canvasSize.y + verticalOffset * canvasSize.y);
        }
    }

    public void AddTracker(Enemy enemy)
    {
        foreach (var t in enemyTrackers)
        {
            if (t.enemy == enemy)
                return;
        }

        EnemyTracker enemyTracker = new EnemyTracker();

        enemyTracker.enemy = enemy;
        enemyTracker.tracker = Instantiate(trackerPrefab.gameObject,
            trackerPivot).GetComponent<Image>();

        enemyTrackers.Add(enemyTracker);
    }
}