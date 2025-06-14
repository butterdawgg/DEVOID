using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : Enemy
{
    [Header("Specific")]
    [SerializeField] private Transform head;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float angleThreshold;
    [SerializeField] private LayerMask gunObstacleMask;

    private bool canShoot;

    protected override void OnUpdate()
    {
        Vector3 dir = Player.Instance.transform.position - head.position;
        float dist = dir.magnitude;

        if (!IsAggroed)
        {
            canShoot = false;
            return;
        }

        Quaternion targetHeadRotation =
            Quaternion.LookRotation(dir.normalized, transform.up);

        head.rotation =
            Quaternion.RotateTowards(head.rotation,
            targetHeadRotation, rotationSpeed * Time.deltaTime);

        float angleX = head.localEulerAngles.x;
        if (angleX > 180f)
            angleX -= 360f;

        if (angleX > angleThreshold)
        {
            head.localEulerAngles = new Vector3(angleThreshold,
                head.localEulerAngles.y,
                head.localEulerAngles.z);
        }

        canShoot = false;

        if (Vector3.Angle(head.forward, dir) < 5.0f)
        {
            if (!Physics.Raycast(head.position, dir.normalized, dist, gunObstacleMask))
            {
                canShoot = true;
            }
        }
    }

    protected override void ControlGuns(List<Gun> guns)
    {
        foreach (Gun gun in guns)
        {
            gun.SetCanShoot(!IsDead && canShoot && !Player.Instance.IsDead);
            gun.SetAimPoint();
        }
    }
}