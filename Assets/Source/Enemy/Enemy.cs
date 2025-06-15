using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth;
    [SerializeField] private VisualEffect deathVFX;
    [SerializeField] private GameObject[] destroyOnDeath;
    [SerializeField] private float respawnTime;
    [Header("Guns")]
    [SerializeField] private Transform[] hardpoints;
    [SerializeField] private Gun gunPrefab;
    [Header("Aggro")]
    [SerializeField] private float aggroRange;
    [SerializeField] private float alertRange;
    [Header("Score")]
    [SerializeField] private int scorePointsOnDeath;
    [Header("Misc")]
    [SerializeField] private string enemyName;

    public float HealthFraction { get { return health / maxHealth; } }
    public bool IsDead { get { return isDead; } }
    public bool IsAggroed {  get { return isAggroed; } }
    public string Name { get { return enemyName; } }

    private float health;
    private bool isDead;

    private List<Gun> guns = new ();

    private bool isAggroed = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float deathTime;

    private void Awake()
    {
        health = maxHealth;

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        foreach (Transform hardpoint in hardpoints)
        {
            guns.Add(Instantiate(gunPrefab.gameObject, hardpoint.position,
                Quaternion.LookRotation(hardpoint.forward), hardpoint).
                GetComponent<Gun>());
        }
    }

    private void Update()
    {
        Vector3 dir = Player.Instance.transform.position - transform.position;
        float dist = dir.magnitude;

        if (dist < GetGunRange())
        {
            deathTime = Time.time;
        }

        if (isDead && (respawnTime < Time.time - deathTime))
        {
            OnRespawn();
        }

        if (isDead)
            return;

        if (Player.Instance.IsDead)
        {
            isAggroed = false;

            ControlGuns(guns);

            return;
        }

        if (health <= 0)
        {
            OnDeath();
        }

        if (dist < aggroRange && !isAggroed)
        {
            OnAggro();

            AlertNearbyEnemies();
        }

        if (dist > GetGunRange() && isAggroed)
        {
            isAggroed = false;
        }

        ControlGuns(guns);

        OnUpdate();
    }

    private void AlertNearbyEnemies()
    {
        Collider[] colliders =
                Physics.OverlapSphere(transform.position, alertRange);

        List<Enemy> nearbyEnemies = new();

        foreach (Collider collider in colliders)
        {
            Enemy enemy = collider.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                if (!nearbyEnemies.Contains(enemy))
                {
                    nearbyEnemies.Add(enemy);
                }
            }
        }

        foreach (Enemy enemy in nearbyEnemies)
        {
            enemy.Alert();
        }
    }

    private void Alert()
    {
        OnAggro();
    }

    private void OnAggro()
    {
        isAggroed = true;

        EnemyTrackerManager.Instance.AddTracker(this);
    }

    private void OnRespawn()
    {
        isDead = false;

        health = maxHealth;

        foreach (GameObject obj in destroyOnDeath)
        {
            obj.SetActive(true);
        }
    }

    private void OnDeath()
    {
        isDead = true;
        isAggroed = false;

        deathTime = Time.time;

        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        AudioManager.Instance.PlaySound("Explosion");
        deathVFX.Play();

        Player.Instance.AddToScore(scorePointsOnDeath);

        foreach (GameObject obj in destroyOnDeath)
        {
            obj.SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0f, maxHealth);
        isAggroed = true;

        AlertNearbyEnemies();
    }

    protected float GetGunRange()
    {
        float range = 0f;

        foreach (Gun gun in guns)
        {
            if (gun.GetRange() > range)
                range = gun.GetRange();
        }

        return range;
    }

    protected float GetGunProjectileSpeed()
    {
        float projectileSpeed = 0f;

        foreach (Gun gun in guns)
        {
            if (gun.GetProjectileSpeed() > projectileSpeed)
                projectileSpeed = gun.GetProjectileSpeed();
        }

        return projectileSpeed;
    }

    protected abstract void OnUpdate();
    protected abstract void ControlGuns(List<Gun> guns);
}