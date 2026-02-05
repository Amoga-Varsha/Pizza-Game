using System.Collections.Generic;
using UnityEngine;

public class CombatRoleCoordinator : MonoBehaviour
{
    [SerializeField] private AiDirector director;

    [Header("Attack Slots")]
    [SerializeField] private int minAttackSlots = 1;
    [SerializeField] private int maxAttackSlots = 6;

    [Header("Suppress Slots")]
    [SerializeField] private int minSuppressSlots = 0;
    [SerializeField] private int maxSuppressSlots = 4;

    [Header("Flank Slots")]
    [SerializeField] private int minFlankSlots = 1;
    [SerializeField] private int maxFlankSlots = 3;
    [Range(0f, 1f)] [SerializeField] private float flankOverCapMultiplier = 0.35f;

    private readonly Dictionary<GoapPlanner, GoalType> assignments = new Dictionary<GoapPlanner, GoalType>();
    private readonly Dictionary<GoalType, int> counts = new Dictionary<GoalType, int>();

    public void RegisterPlanner(GoapPlanner planner)
    {
        if (planner == null || assignments.ContainsKey(planner))
        {
            return;
        }

        var initialGoal = planner.CurrentIntent.GoalType;
        assignments.Add(planner, initialGoal);
        Increment(initialGoal);
    }

    public void UnregisterPlanner(GoapPlanner planner)
    {
        if (planner == null)
        {
            return;
        }

        if (assignments.TryGetValue(planner, out var current))
        {
            Decrement(current);
            assignments.Remove(planner);
        }
    }

    public void NotifyIntentChanged(GoapPlanner planner, GoalType previousGoal, GoalType nextGoal)
    {
        if (planner == null)
        {
            return;
        }

        Decrement(previousGoal);
        Increment(nextGoal);
        assignments[planner] = nextGoal;
    }

    public float GetPriorityMultiplier(GoapPlanner planner, GoalType goalType)
    {
        if (goalType == GoalType.None || goalType == GoalType.Retreat)
        {
            return 1f;
        }

        var currentGoal = GoalType.None;
        if (planner != null)
        {
            assignments.TryGetValue(planner, out currentGoal);
        }

        if (currentGoal == goalType)
        {
            return 1f;
        }

        var caps = GetCaps();
        var count = GetCount(goalType);
        switch (goalType)
        {
            case GoalType.Attack:
                return count < caps.Attack ? 1f : 0f;
            case GoalType.Suppress:
                return count < caps.Suppress ? 1f : 0f;
            case GoalType.Flank:
                return count < caps.Flank ? 1f : flankOverCapMultiplier;
            default:
                return 1f;
        }
    }

    private (int Attack, int Suppress, int Flank) GetCaps()
    {
        var mods = director != null ? director.CurrentModifiers : default;
        var attackT = Mathf.Clamp01(mods.Aggression * mods.PushBias);
        var suppressT = Mathf.Clamp01(mods.Aggression * (1f - mods.PushBias));
        var flankT = Mathf.Clamp01(mods.FlankBias);

        var attack = Mathf.RoundToInt(Mathf.Lerp(minAttackSlots, maxAttackSlots, attackT));
        var suppress = Mathf.RoundToInt(Mathf.Lerp(minSuppressSlots, maxSuppressSlots, suppressT));
        var flank = Mathf.RoundToInt(Mathf.Lerp(minFlankSlots, maxFlankSlots, flankT));

        return (attack, suppress, flank);
    }

    private int GetCount(GoalType goalType)
    {
        return counts.TryGetValue(goalType, out var count) ? count : 0;
    }

    private void Increment(GoalType goalType)
    {
        if (goalType == GoalType.None)
        {
            return;
        }

        counts[goalType] = GetCount(goalType) + 1;
    }

    private void Decrement(GoalType goalType)
    {
        if (goalType == GoalType.None)
        {
            return;
        }

        var next = GetCount(goalType) - 1;
        if (next <= 0)
        {
            counts.Remove(goalType);
        }
        else
        {
            counts[goalType] = next;
        }
    }
}
