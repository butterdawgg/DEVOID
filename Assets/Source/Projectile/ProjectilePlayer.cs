using System.Collections.Generic;
using UnityEngine;

public class ProjectilePlayer : Projectile
{
    protected override void OnHit(RaycastHit hitInfo, float damage)
    {
        if (splashRadius == 0)
        {
            Enemy enemy = hitInfo.collider.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                AudioManager.Instance.PlaySound("ProjectileHit");
                enemy.TakeDamage(damage);
            }
        }
        else
        {
            Collider[] colliders =
                Physics.OverlapSphere(transform.position, splashRadius, hitMask);

            List<Enemy> hitEnemies = new ();

            foreach (Collider collider in colliders)
            {
                Enemy enemy = collider.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    if (!hitEnemies.Contains(enemy))
                    {
                        AudioManager.Instance.PlaySound("ProjectileHit");
                        enemy.TakeDamage(damage);
                        hitEnemies.Add(enemy);
                    }
                }
            }
        }
    }
}