using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public abstract class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth;
    [SerializeField] private VisualEffect deathVFX;
    [SerializeField] private GameObject[] destroyOnDeath;
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
    protected bool IsAggroed {  get { return isAggroed; } }
    public string Name { get { return enemyName; } }

    private float health;
    private bool isDead;

    private List<Gun> guns = new ();

    private bool isAggroed = false;

    private void Awake()
    {
        health = maxHealth;

        foreach (Transform hardpoint in hardpoints)
        {
            guns.Add(Instantiate(gunPrefab.gameObject, hardpoint.position,
                Quaternion.LookRotation(hardpoint.forward), hardpoint).
                GetComponent<Gun>());
        }
    }

    private void Update()
    {
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

            isDead = true;
        }

        Vector3 dir = Player.Instance.transform.position - transform.position;
        float dist = dir.magnitude;

        if (dist < aggroRange && !isAggroed)
        {
            isAggroed = true;

            AlertNearbyEnemies();
        }

        if (dist > Player.Instance.GetGunRange() && isAggroed)
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
        isAggroed = true;
    }

    private void OnDeath()
    {
        AudioManager.Instance.PlaySound("Explosion");
        deathVFX.Play();

        Player.Instance.AddToScore(scorePointsOnDeath);

        foreach (GameObject obj in destroyOnDeath)
        {
            Destroy(obj);
        }

        Destroy(gameObject, 1f);
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0f, maxHealth);
        isAggroed = true;

        AlertNearbyEnemies();
    }

    protected abstract void OnUpdate();
    protected abstract void ControlGuns(List<Gun> guns);
}