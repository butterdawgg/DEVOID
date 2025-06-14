using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationLerpK;
    [SerializeField] private float minFOV;
    [SerializeField] private float maxFOV;
    [SerializeField] private float FOVLeprK;

    public static CameraController Instance { get; private set; }
    public Camera Camera { get; private set; }


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Camera = GetComponentInChildren<Camera>();

        if (Camera == GetComponent<Camera>())
            Camera = null;

        if (Camera == null)
            Debug.Log("CameraController doesn't have a camera as a child object!");
    }

    void FixedUpdate()
    {
        if (HUDManager.IsPaused)
            return;

        transform.position = Player.Instance.transform.position;

        transform.rotation = Quaternion.Lerp(transform.rotation,
                                             Player.Instance.transform.rotation,
                                             rotationLerpK * Time.fixedDeltaTime);
    }

    private void Update()
    {
        if (HUDManager.IsPaused)
            return;

        if (Camera == null)
            return;

        Camera.fieldOfView =
            Mathf.Lerp(Camera.fieldOfView,
            Mathf.Lerp(minFOV, maxFOV, Player.Instance.VelocityFraction),
            FOVLeprK * Time.deltaTime);
    }
}