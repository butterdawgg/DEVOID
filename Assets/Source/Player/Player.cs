using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Maximum health of the player")]
    [SerializeField] private float maxHealth;
    [Header("Movement")]
    [Tooltip("Measured in meters per second")]
    [SerializeField] private float minLinearVelocity;
    [Tooltip("Measured in meters per second")]
    [SerializeField] private float maxLinearVelocity;
    [Tooltip("Measured in meters per second squared")]
    [SerializeField] private float linearAcceleration;
    [Tooltip("Rate of change for the thrust parameter")]
    [SerializeField] private float thrustChangeRate;
    [Header("Orientation")]
    [Tooltip("Measured in revolutions per second")]
    [SerializeField] private Vector3 angularVelocity;
    [Tooltip("Measured in revolutions per second squared")]
    [SerializeField] private float angularAcceleration;
    [Header("Guns")]
    [SerializeField] private Transform[] hardpoints;
    [SerializeField] private Gun gunPrefab;
    [Header("Targeting")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask aimMask;
    [Header("Death")]
    [SerializeField] private GameObject[] destroyOnDeath;
    [SerializeField] private VisualEffect deathVFX;

    public static Player Instance { get; private set; }
    public float VelocityFraction { get; private set; }
    public Vector3 Velocity { get; private set; }
    public float HealthFraction { get; private set; }
    public float ThrustFraction { get; private set; }
    public Enemy Target { get; private set; }
    public bool IsDead { get { return isDead; } }
    public int Score { get { return score; } }

    private Rigidbody rb;
    private float thrust;

    private float health;
    private bool isDead;

    private List<Gun> guns = new ();

    private float lastTargetAcquiredTime;
    private float targetResetCooldown = 0.1f;

    private int score;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        rb = GetComponent<Rigidbody>();

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
        if (Target != null)
        {
            if (Target.IsDead)
                Target = null;
        }

        int highScore = SerializeManager.GetHighScore();
        if (highScore < score)
            SerializeManager.SetHighScore(score);

        if (isDead)
            return;

        if (health <= 0)
        {
            isDead = true;

            OnDeath();
        }

        if (HUDManager.IsPaused)
            return;

        ControlParameters();
        ControlGuns();
        ControlTarget();
        ControlMovement();
    }

    private void ControlParameters()
    {
        VelocityFraction = Mathf.InverseLerp(minLinearVelocity,
                                             maxLinearVelocity,
                                             rb.linearVelocity.magnitude);

        Velocity = rb.linearVelocity;

        HealthFraction = health / maxHealth;

        ThrustFraction = thrust;
    }

    private void ControlGuns()
    {
        foreach (Gun gun in guns)
        {
            gun.SetCanShoot(InputManager.GetKey(Action.Shoot));
        }

        Ray ray = CameraController.Instance.Camera.ScreenPointToRay(
            InputManager.GetCrosshairPosBottomLeft());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimMask))
        {
            foreach (Gun gun in guns)
            {
                gun.SetAimPoint(hit.point, ray);
            }
        }
        else
        {
            foreach (Gun gun in guns)
            {
                gun.SetAimPoint(ray);
            }
        }
    }

    private void ControlTarget()
    {
        Ray ray = CameraController.Instance.Camera.ScreenPointToRay(
            InputManager.GetCrosshairPosBottomLeft());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, targetMask))
        {
            Enemy enemy = hit.transform.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                if (Vector3.Distance(enemy.transform.position, transform.position) <
                    GetGunRange())
                {
                    Target = enemy;
                    lastTargetAcquiredTime = Time.time;
                }
                else
                {
                    if (targetResetCooldown < Time.time - lastTargetAcquiredTime)
                        Target = null;
                }
            }
            else
            {
                if (targetResetCooldown < Time.time - lastTargetAcquiredTime)
                    Target = null;
            }
        }
        else
        {
            if (targetResetCooldown < Time.time - lastTargetAcquiredTime)
                Target = null;
        }

        Debug.Log(Target == null ? "no target" : "yes target");
    }

    private void ControlMovement()
    {
        // Input:
        bool accInput = InputManager.GetKey(Action.Accelerate);
        bool decInput = InputManager.GetKey(Action.Decelerate);

        float thrustInput = 0f;

        if (accInput && !decInput)
            thrustInput = 1f;
        else if (!accInput && decInput)
            thrustInput = -1f;

        Vector2 input = InputManager.GetCrosshairPosCenterNormalized();
        Vector2 inputNormalized = input.normalized;
        float inputMagnitude = input.magnitude;
        float modulatedMagnitude = Mathf.Pow(Mathf.Abs(inputMagnitude), 1.2f);
        Vector2 turnDirection = inputNormalized * modulatedMagnitude;

        float rollInput = 0f;

        bool rollLeftInput = InputManager.GetKey(Action.RollLeft);
        bool rollRightInput = InputManager.GetKey(Action.RollRight);

        if (rollLeftInput && !rollRightInput)
            rollInput = 1f;
        else if (!rollLeftInput && rollRightInput)
            rollInput = -1f;

        // Movement:
        thrust += thrustInput * thrustChangeRate * Time.deltaTime;
        thrust = Mathf.Clamp01(thrust);

        Vector3 targetLinearVelocity =
            Mathf.Lerp(minLinearVelocity, maxLinearVelocity, thrust) *
            transform.forward;

        rb.linearVelocity +=
            Vector3.ClampMagnitude(targetLinearVelocity - rb.linearVelocity, 1f) *
            linearAcceleration *
            Time.deltaTime;

        // Orientation:
        float pitchVelocity = angularVelocity.x * (2f * Mathf.PI);
        float yawVelocity = angularVelocity.y * (2f * Mathf.PI);
        float rollVelocity = angularVelocity.z * (2f * Mathf.PI);

        Vector3 targetAngularVelocity =
            (transform.right * -turnDirection.y * pitchVelocity) +
            (transform.up * turnDirection.x * yawVelocity) +
            (transform.forward * rollInput * rollVelocity);

        rb.angularVelocity +=
            Vector3.ClampMagnitude(targetAngularVelocity - rb.angularVelocity, 1f) *
            (angularAcceleration * (2f * Mathf.PI)) *
            Time.deltaTime;
    }

    private void OnDeath()
    {
        int highScore = SerializeManager.GetHighScore();
        if (highScore < score)
            SerializeManager.SetHighScore(score);

        Target = null;

        StartCoroutine(DeathCouroutine());
    }

    private IEnumerator DeathCouroutine()
    {
        AudioManager.Instance.PlaySound("Explosion");

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        foreach (GameObject obj in destroyOnDeath)
        {
            Destroy(obj);
        }

        deathVFX.Play();

        yield return new WaitForSeconds(2f);

        HUDManager.Instance.OnPlayerDeath();
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0f, maxHealth);
    }

    public float GetGunProjectileSpeed()
    {
        return gunPrefab.GetProjectileSpeed();
    }

    public float GetGunRange()
    {
        return gunPrefab.GetRange();
    }

    public bool GetGunVisibility(Vector3 position, LayerMask obstacleMask)
    {
        if (isDead)
            return false;

        foreach (Gun gun in guns)
        {
            if (!gun.GetVisibility(position, obstacleMask))
            {
                return false;
            }
        }

        return true;
    }

    public void AddToScore(int value)
    {
        score += value;
    }
}