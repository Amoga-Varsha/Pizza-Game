using System;
using System.Collections.Generic;
using UnityEngine;

public class GoapPlanner : MonoBehaviour
{
    [SerializeField] private AiContext context;
    [SerializeField] private CombatRoleCoordinator roleCoordinator;
    [SerializeField] private List<GoapGoalDefinition> goals = new List<GoapGoalDefinition>();
    [Range(0.05f, 1f)] [SerializeField] private float updateInterval = 0.2f;
    [SerializeField] private float goalSwitchMultiplier = 1.2f;
    [SerializeField] private float minGoalDuration = 0f;

    [Header("Role Weights")]
    [SerializeField] private float attackRoleWeight = 1f;
    [SerializeField] private float flankRoleWeight = 1f;
    [SerializeField] private float suppressRoleWeight = 1f;
    [SerializeField] private float retreatRoleWeight = 1f;

    public IntentData CurrentIntent { get; private set; }

    public event Action<IntentData> IntentChanged;

    private float timer;
    private float currentGoalTimer;

    private void OnEnable()
    {
        if (roleCoordinator != null)
        {
            roleCoordinator.RegisterPlanner(this);
        }
    }

    private void OnDisable()
    {
        if (roleCoordinator != null)
        {
            roleCoordinator.UnregisterPlanner(this);
        }
    }

    public void ApplyRoleWeights(float attack, float flank, float suppress, float retreat)
    {
        attackRoleWeight = attack;
        flankRoleWeight = flank;
        suppressRoleWeight = suppress;
        retreatRoleWeight = retreat;
    }

    public void ApplyPlannerTuning(float goalSwitchMultiplierValue, float minGoalDurationValue)
    {
        goalSwitchMultiplier = goalSwitchMultiplierValue;
        minGoalDuration = minGoalDurationValue;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        currentGoalTimer += Time.deltaTime;
        if (timer < updateInterval)
        {
            return;
        }

        timer = 0f;
        var nextIntent = EvaluateIntent(out var currentScore);

        if (nextIntent.GoalType != CurrentIntent.GoalType)
        {
            var previousGoal = CurrentIntent.GoalType;
            if (CurrentIntent.GoalType != GoalType.None)
            {
                if (currentGoalTimer < minGoalDuration)
                {
                    return;
                }

                if (nextIntent.Priority < currentScore * goalSwitchMultiplier)
                {
                    return;
                }
            }

            CurrentIntent = nextIntent;
            currentGoalTimer = 0f;
            if (roleCoordinator != null)
            {
                roleCoordinator.NotifyIntentChanged(this, previousGoal, CurrentIntent.GoalType);
            }
            IntentChanged?.Invoke(CurrentIntent);
        }
    }

    private IntentData EvaluateIntent(out float currentScore)
    {
        var best = new IntentData { GoalType = GoalType.None, Priority = 0f };
        currentScore = 0f;
        if (context == null)
        {
            return best;
        }

        foreach (var goal in goals)
        {
            if (goal == null)
            {
                continue;
            }

            var priority = goal.BasePriority;
            priority *= GetDirectorBias(goal.GoalType, context.DirectorModifiers);
            priority *= context.GetContextModifier(goal.GoalType);
            priority *= GetRoleWeight(goal.GoalType);
            if (roleCoordinator != null)
            {
                var multiplier = roleCoordinator.GetPriorityMultiplier(this, goal.GoalType);
                if (multiplier <= 0f)
                {
                    continue;
                }

                priority *= multiplier;
            }

            if (goal.GoalType == CurrentIntent.GoalType)
            {
                currentScore = priority;
            }

            if (priority > best.Priority)
            {
                best = new IntentData { GoalType = goal.GoalType, Priority = priority };
            }
        }

        return best;
    }

    private static float GetDirectorBias(GoalType goalType, DirectorModifiers modifiers)
    {
        switch (goalType)
        {
            case GoalType.Attack:
                return Mathf.Max(0.01f, modifiers.Aggression * Mathf.Max(0.5f, modifiers.PushBias));
            case GoalType.Flank:
                return Mathf.Max(0.01f, modifiers.FlankBias);
            case GoalType.Retreat:
                return Mathf.Max(0.01f, modifiers.RetreatBias);
            case GoalType.Suppress:
                return Mathf.Max(0.01f, modifiers.Aggression);
            default:
                return 0.01f;
        }
    }

    private float GetRoleWeight(GoalType goalType)
    {
        switch (goalType)
        {
            case GoalType.Attack:
                return Mathf.Max(0.01f, attackRoleWeight);
            case GoalType.Flank:
                return Mathf.Max(0.01f, flankRoleWeight);
            case GoalType.Retreat:
                return Mathf.Max(0.01f, retreatRoleWeight);
            case GoalType.Suppress:
                return Mathf.Max(0.01f, suppressRoleWeight);
            default:
                return 1f;
        }
    }
}
