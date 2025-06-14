using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float size;
    [SerializeField] private LayerMask obstacleMask;

    Rigidbody rb;
    MeshRenderer mr;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (Player.Instance.IsDead)
            return;

        if (HUDManager.IsPaused)
            return;

        float distance = (rb.position - Player.Instance.transform.position).magnitude;
        float travelTime = distance / Player.Instance.GetGunProjectileSpeed();

        transform.position = rb.position + rb.linearVelocity * travelTime;

        Vector3 camPos = CameraController.Instance.Camera.transform.position;

        Vector3 toCam = camPos - transform.position;

        float distToCam = toCam.magnitude;

        transform.rotation = Quaternion.LookRotation(toCam);

        float diameter = 2f * Mathf.Tan(size * Mathf.Deg2Rad * 0.5f) * distToCam;

        transform.localScale = new Vector3(diameter, diameter, 1f);

        Vector3 rayDir = (transform.position - camPos).normalized;
        Ray ray = new(camPos, rayDir);

        bool occluded = true;

        if (distance < Player.Instance.GetGunRange())
        {
            if (Player.Instance.GetGunVisibility(transform.position, obstacleMask))
            {
                occluded = false;
            }
        }

        mr.forceRenderingOff = occluded;
    }
}