using UnityEngine;
using UnityEngine.VFX;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] private VisualEffect hitVFX;
    [SerializeField] private GameObject[] destroyOnDeath;
    [SerializeField] protected LayerMask hitMask;

    private float speed;
    private float damage;
    private float lifetime;
    protected float splashRadius;

    private float createdTime;
    private bool isDead;

    public static Projectile Launch(Projectile prototype,
        Vector3 position, Vector3 direction, float speed,
        float range, float spread, float damage, float splashRadius)
    {
        Projectile proj =
            Instantiate(prototype, position, Quaternion.LookRotation(direction));

        proj.transform.localEulerAngles +=
            new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized *
            Random.Range(-spread, spread);

        proj.speed = speed;
        proj.damage = damage;
        proj.lifetime = range / speed;
        proj.createdTime = Time.time;
        proj.isDead = false;
        proj.splashRadius = splashRadius;

        return proj;
    }

    protected abstract void OnHit(RaycastHit hitInfo, float damage);

    private void OnDeath()
    {
        isDead = true;

        hitVFX.Play();

        foreach (GameObject obj in destroyOnDeath)
        {
            Destroy(obj);
        }

        Destroy(gameObject, 1f);
    }

    private void Update()
    {
        if (HUDManager.IsPaused)
            return;

        if (isDead)
            return;

        if (lifetime < Time.time - createdTime)
            OnDeath();

        float distance = speed * Time.deltaTime;

        Ray ray = new(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, hitMask))
        {
            transform.position = hitInfo.point;
            transform.parent = hitInfo.transform;

            OnHit(hitInfo, damage);
            OnDeath();
        }
        else
        {
            transform.position += transform.forward * distance;
        }
    }
}