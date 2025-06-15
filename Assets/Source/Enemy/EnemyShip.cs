using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : Enemy
{
    [Header("Specific")]
    [SerializeField] private float linearVelocity;
    [SerializeField] private float turnLinearVelocity;
    [SerializeField] private float linearAcceleration;
    [SerializeField] private float angularVelocity;
    [SerializeField] private float turnAngularVelocity;
    [SerializeField] private float angularAcceleration;
    [SerializeField] private float turnRange;
    [SerializeField] private float returnRange;
    [SerializeField] private Transform gunPivot;
    [SerializeField] private LayerMask gunObstacleMask;

    private Rigidbody rb;

    private bool canShoot = false;
    private bool isTurning = false;

    protected override void OnUpdate()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (Player.Instance.IsDead)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            canShoot = false;

            return;
        }

        if (IsDead)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            canShoot = false;

            return;
        }

        Vector3 dir = Player.Instance.transform.position - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        if (isTurning)
        {
            if (dist > returnRange)
                isTurning = false;
        }
        else
        {
            if (dist < turnRange)
                isTurning = true;
        }

        Vector3 targetLinearVelocity = Vector3.zero;
        Vector3 targetAngularVelocity = Vector3.zero;

        if (IsAggroed)
        {
            Vector3 targetDirection = Vector3.zero;

            float linearSpeed = 0f;
            float angularSpeed = 0f;

            if (isTurning)
            {
                targetDirection = Vector3.ProjectOnPlane(transform.forward,
                    dir).normalized;

                linearSpeed = turnLinearVelocity;
                angularSpeed = turnAngularVelocity;
            }
            else
            {
                targetDirection = dir;

                linearSpeed = linearVelocity;
                angularSpeed = angularVelocity;
            }

            targetLinearVelocity = transform.forward * linearSpeed;

            Vector3 forward = transform.forward;
            Vector3 desired = targetDirection;

            float dot = Vector3.Dot(forward, desired);
            Vector3 axis = Vector3.Cross(forward, desired);

            if (axis.sqrMagnitude < 1e-6f)
            {
                if (dot < 0f)
                {
                    Vector3 random = Vector3.ProjectOnPlane(transform.forward,
                        Random.onUnitSphere);

                    axis = Vector3.Cross(forward, random).normalized;

                    if (axis.sqrMagnitude < 1e-6f)
                        axis = Vector3.Cross(forward, Vector3.up).normalized;
                }
                else
                {
                    axis = Vector3.zero;
                }
            }
            else
            {
                axis.Normalize();
            }

            targetAngularVelocity = axis * angularSpeed * (2f * Mathf.PI);
        }

        rb.linearVelocity +=
            Vector3.ClampMagnitude(targetLinearVelocity - rb.linearVelocity, 1f) *
            linearAcceleration *
            Time.deltaTime;

        rb.angularVelocity +=
            Vector3.ClampMagnitude(targetAngularVelocity - rb.angularVelocity, 1f) *
            (angularAcceleration * (2f * Mathf.PI)) *
            Time.deltaTime;

        canShoot = false;

        if (!IsAggroed)
            return;

        float gunDist = (Player.Instance.transform.position -
            gunPivot.position).magnitude;

        float travelTime = gunDist / GetGunProjectileSpeed();

        Vector3 gunDir = ((Player.Instance.transform.position +
            Player.Instance.Velocity * travelTime) -
            gunPivot.position).normalized;

        if (Vector3.Angle(transform.forward, gunDir) < 45.0f)
        {
            if (!Physics.Raycast(transform.position, gunDir, dist,
                gunObstacleMask))
            {
                gunPivot.rotation = Quaternion.LookRotation(gunDir);

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