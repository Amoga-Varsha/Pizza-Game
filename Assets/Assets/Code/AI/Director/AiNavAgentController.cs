using UnityEngine;
using UnityEngine.AI;

public class AiNavAgentController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private HfsmController hfsm;
    [SerializeField] private Transform target;
    [SerializeField] private float repathInterval = 0.5f;
    [SerializeField] private float targetMoveThreshold = 1f;
    [SerializeField] private float desiredAttackRange = 6f;
    [SerializeField] private float suppressRange = 10f;
    [SerializeField] private float flankDistance = 6f;
    [SerializeField] private float flankLateralOffset = 4f;
    [SerializeField] private float retreatDistance = 8f;
    [SerializeField] private float sampleRadius = 2f;
    [SerializeField] private bool stopWhenInRange = true;

    private float timer;
    private Vector3 lastTargetPos;
    private HfsmState lastState;

    public void ApplyRoleNavigation(
        float desiredAttackRangeValue,
        float suppressRangeValue,
        float flankDistanceValue,
        float flankLateralOffsetValue,
        float retreatDistanceValue,
        bool stopWhenInRangeValue,
        float repathIntervalValue,
        float targetMoveThresholdValue,
        float sampleRadiusValue)
    {
        desiredAttackRange = desiredAttackRangeValue;
        suppressRange = suppressRangeValue;
        flankDistance = flankDistanceValue;
        flankLateralOffset = flankLateralOffsetValue;
        retreatDistance = retreatDistanceValue;
        stopWhenInRange = stopWhenInRangeValue;
        repathInterval = repathIntervalValue;
        targetMoveThreshold = targetMoveThresholdValue;
        sampleRadius = sampleRadiusValue;
    }

    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        hfsm = GetComponent<HfsmController>();
    }

    private void Update()
    {
        if (agent == null || hfsm == null || target == null)
        {
            return;
        }

        agent.speed = hfsm.CurrentMoveSpeed;

        timer += Time.deltaTime;
        var stateChanged = hfsm.CurrentState != lastState;
        var targetMoved = (target.position - lastTargetPos).sqrMagnitude > targetMoveThreshold * targetMoveThreshold;

        if (stateChanged || timer >= repathInterval || targetMoved)
        {
            timer = 0f;
            lastState = hfsm.CurrentState;
            lastTargetPos = target.position;
            UpdateDestination();
        }
    }

    private void UpdateDestination()
    {
        if (hfsm.CurrentState == HfsmState.Idle)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        var origin = transform.position;
        var targetPos = target.position;
        var toTarget = (targetPos - origin);
        var distance = toTarget.magnitude;
        var dir = distance > 0.01f ? toTarget / distance : transform.forward;

        Vector3 desired;
        switch (hfsm.CurrentState)
        {
            case HfsmState.Advance:
                if (stopWhenInRange && distance <= desiredAttackRange)
                {
                    agent.isStopped = true;
                    return;
                }
                desired = targetPos - dir * desiredAttackRange;
                break;
            case HfsmState.Fire:
                if (stopWhenInRange && distance <= suppressRange)
                {
                    agent.isStopped = true;
                    return;
                }
                desired = targetPos - dir * suppressRange;
                break;
            case HfsmState.Flank:
                var side = (GetInstanceID() & 1) == 0 ? 1f : -1f;
                var right = Vector3.Cross(Vector3.up, dir) * side;
                desired = targetPos + right * flankLateralOffset - dir * flankDistance;
                break;
            case HfsmState.Retreat:
                desired = origin - dir * retreatDistance;
                break;
            default:
                desired = targetPos;
                break;
        }

        if (NavMesh.SamplePosition(desired, out var hit, sampleRadius, NavMesh.AllAreas))
        {
            desired = hit.position;
        }

        agent.SetDestination(desired);
    }
}
