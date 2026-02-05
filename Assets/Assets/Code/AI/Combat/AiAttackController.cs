using UnityEngine;

public class AiAttackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private AiContext context;
    [SerializeField] private HfsmController hfsm;
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform muzzle;

    [Header("Behavior")]
    [SerializeField] private bool requireLineOfSight = true;
    [SerializeField] private bool gateByHfsmState = true;

    [Header("Attack Tuning")]
    [SerializeField] private float rangedRange = 12f;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private int meleeDamage = 12;
    [SerializeField] private float meleeCooldown = 1.2f;
    [SerializeField] private int shotsPerBurst = 3;
    [SerializeField] private float timeBetweenShots = 0.2f;
    [SerializeField] private float burstCooldown = 1.5f;
    [SerializeField] private float projectileSpeed = 18f;
    [SerializeField] private int projectileDamage = 8;
    [SerializeField] private float targetAimHeight = 1.4f;

    private float meleeTimer;
    private float burstTimer;
    private float shotTimer;
    private int shotsRemaining;

    private void Reset()
    {
        hfsm = GetComponent<HfsmController>();
        context = GetComponent<AiContext>();
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        if (gateByHfsmState && hfsm != null && !IsAllowedState(hfsm.CurrentState))
        {
            return;
        }

        var distance = Vector3.Distance(transform.position, target.position);
        TickTimers(Time.deltaTime);

        if (distance <= meleeRange)
        {
            TryMelee();
            return;
        }

        if (distance <= rangedRange && HasLineOfSight())
        {
            TryRanged();
        }
    }

    public void ApplyAttackTuning(
        float rangedRangeValue,
        float meleeRangeValue,
        int meleeDamageValue,
        float meleeCooldownValue,
        int shotsPerBurstValue,
        float timeBetweenShotsValue,
        float burstCooldownValue,
        float projectileSpeedValue,
        int projectileDamageValue)
    {
        rangedRange = rangedRangeValue;
        meleeRange = meleeRangeValue;
        meleeDamage = meleeDamageValue;
        meleeCooldown = meleeCooldownValue;
        shotsPerBurst = shotsPerBurstValue;
        timeBetweenShots = timeBetweenShotsValue;
        burstCooldown = burstCooldownValue;
        projectileSpeed = projectileSpeedValue;
        projectileDamage = projectileDamageValue;
    }

    private void TickTimers(float deltaTime)
    {
        if (meleeTimer > 0f)
        {
            meleeTimer -= deltaTime;
        }

        if (burstTimer > 0f)
        {
            burstTimer -= deltaTime;
        }

        if (shotTimer > 0f)
        {
            shotTimer -= deltaTime;
        }
    }

    private void TryMelee()
    {
        if (meleeTimer > 0f)
        {
            return;
        }

        var health = target.GetComponentInParent<PlayerHealth>();
        if (health != null)
        {
            health.ApplyDamage(meleeDamage);
        }

        meleeTimer = meleeCooldown;
        shotsRemaining = 0;
    }

    private void TryRanged()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        if (shotsRemaining <= 0)
        {
            if (burstTimer > 0f)
            {
                return;
            }

            shotsRemaining = Mathf.Max(1, shotsPerBurst);
            shotTimer = 0f;
        }

        if (shotTimer > 0f)
        {
            return;
        }

        FireProjectile();
        shotsRemaining--;
        shotTimer = Mathf.Max(0.01f, timeBetweenShots);

        if (shotsRemaining <= 0)
        {
            burstTimer = Mathf.Max(0f, burstCooldown);
        }
    }

    private void FireProjectile()
    {
        var spawn = muzzle != null ? muzzle.position : transform.position;
        var targetPos = target.position + Vector3.up * targetAimHeight;
        var direction = (targetPos - spawn).normalized;
        var rotation = direction.sqrMagnitude > 0.001f ? Quaternion.LookRotation(direction) : transform.rotation;

        var projectile = Instantiate(projectilePrefab, spawn, rotation);
        projectile.Initialize(direction, projectileSpeed, projectileDamage, gameObject);
    }

    private bool HasLineOfSight()
    {
        if (!requireLineOfSight)
        {
            return true;
        }

        if (context != null)
        {
            return context.WorldFacts.HasLineOfSight;
        }

        return true;
    }

    private static bool IsAllowedState(HfsmState state)
    {
        return state == HfsmState.Advance || state == HfsmState.Fire || state == HfsmState.Flank;
    }
}
