using System.Drawing;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileSpread;
    [SerializeField] private float projectileRange;
    [SerializeField] private float projectileDamage;
    [SerializeField] private float projectileCount;
    [SerializeField] private float projectileSplashRadius;
    [SerializeField] private float cooldown;
    [SerializeField] private float angleThreshold;
    [Header("Composition")]
    [SerializeField] private Projectile projectilePrototype;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private VisualEffect muzzleFlashVFX;

    private float lastShotTime;
    private bool canShoot;
    private Vector3 aimPoint;

    void Update()
    {
        if (HUDManager.IsPaused)
            return;

        if (cooldown > Time.time - lastShotTime || !canShoot)
            return;

        lastShotTime = Time.time;

        muzzleFlashVFX.Play();

        AudioManager.Instance.PlaySound("ProjectileShoot");

        Vector3 aimDir = aimPoint - muzzlePoint.position;

        for (int i = 0; i < projectileCount; i++)
        {
            Projectile.Launch(projectilePrototype, muzzlePoint.position,
                              aimDir, projectileSpeed, projectileRange,
                              projectileSpread, projectileDamage,
                              projectileSplashRadius);
        }
    }

    public void SetCanShoot(bool canShoot)
    {
        this.canShoot = canShoot;
    }

    public void SetAimPoint(Transform target, Ray aimRay)
    {
        aimPoint = aimRay.GetPoint((target.position - aimRay.origin).magnitude);
    }

    public void SetAimPoint(Vector3 point, Ray aimRay)
    {
        Vector3 aimDir = point - muzzlePoint.position;

        if (Vector3.Angle(aimDir, muzzlePoint.forward) > angleThreshold)
            aimPoint = aimRay.GetPoint(projectileRange);
        else
            aimPoint = point;
    }

    public void SetAimPoint(Ray aimRay)
    {
        aimPoint = aimRay.GetPoint(projectileRange);
    }

    public void SetAimPoint()
    {
        aimPoint = muzzlePoint.position + muzzlePoint.forward;
    }

    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }

    public float GetRange()
    {
        return projectileRange;
    }

    public bool GetVisibility(Vector3 position, LayerMask obstacleMask)
    {
        Vector3 dir = (position - muzzlePoint.position);
        float dist = dir.magnitude;
        Ray ray = new(muzzlePoint.position, dir);

        return !Physics.Raycast(ray, dist, obstacleMask);
    }
}