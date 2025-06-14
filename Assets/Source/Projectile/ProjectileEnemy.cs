using UnityEngine;

public class ProjectileEnemy : Projectile
{
    protected override void OnHit(RaycastHit hitInfo, float damage)
    {
        if (splashRadius == 0f)
        {
            if (hitInfo.collider.GetComponentInParent<Player>() != null)
            {
                AudioManager.Instance.PlaySound("ProjectileHit");
                Player.Instance.TakeDamage(damage);
            }
        }
        else
        {
            Collider[] colliders =
                Physics.OverlapSphere(transform.position, splashRadius, hitMask);

            foreach (Collider collider in colliders)
            {
                if (collider.GetComponentInParent<Player>() != null)
                {
                    AudioManager.Instance.PlaySound("ProjectileHit");
                    Player.Instance.TakeDamage(damage);
                    break;
                }
            }
        }
    }
}