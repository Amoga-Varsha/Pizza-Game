using UnityEngine;

[CreateAssetMenu(menuName = "AI/Enemy Role Profile", fileName = "EnemyRoleProfile")]
public class EnemyRoleProfile : ScriptableObject
{
    [Header("Planner Weights")]
    [SerializeField] private float attackRoleWeight = 1f;
    [SerializeField] private float flankRoleWeight = 1f;
    [SerializeField] private float suppressRoleWeight = 1f;
    [SerializeField] private float retreatRoleWeight = 1f;

    [Header("Planner Tuning")]
    [SerializeField] private float goalSwitchMultiplier = 1.2f;
    [SerializeField] private float minGoalDuration = 0f;

    [Header("Movement Speed")]
    [SerializeField] private float minMoveSpeed = 2f;
    [SerializeField] private float maxMoveSpeed = 4.5f;

    [Header("Nav Ranges")]
    [SerializeField] private float desiredAttackRange = 6f;
    [SerializeField] private float suppressRange = 10f;
    [SerializeField] private float flankDistance = 6f;
    [SerializeField] private float flankLateralOffset = 4f;
    [SerializeField] private float retreatDistance = 8f;
    [SerializeField] private bool stopWhenInRange = true;

    [Header("Nav Tuning")]
    [SerializeField] private float repathInterval = 0.5f;
    [SerializeField] private float targetMoveThreshold = 1f;
    [SerializeField] private float sampleRadius = 2f;

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

    public void ApplyTo(GoapPlanner planner, HfsmController hfsm, AiNavAgentController nav, AiAttackController attack)
    {
        if (planner != null)
        {
            planner.ApplyRoleWeights(attackRoleWeight, flankRoleWeight, suppressRoleWeight, retreatRoleWeight);
            planner.ApplyPlannerTuning(goalSwitchMultiplier, minGoalDuration);
        }

        if (hfsm != null)
        {
            hfsm.ApplyMoveSpeed(minMoveSpeed, maxMoveSpeed);
        }

        if (nav != null)
        {
            nav.ApplyRoleNavigation(
                desiredAttackRange,
                suppressRange,
                flankDistance,
                flankLateralOffset,
                retreatDistance,
                stopWhenInRange,
                repathInterval,
                targetMoveThreshold,
                sampleRadius);
        }

        if (attack != null)
        {
            attack.ApplyAttackTuning(
                rangedRange,
                meleeRange,
                meleeDamage,
                meleeCooldown,
                shotsPerBurst,
                timeBetweenShots,
                burstCooldown,
                projectileSpeed,
                projectileDamage);
        }
    }
}
