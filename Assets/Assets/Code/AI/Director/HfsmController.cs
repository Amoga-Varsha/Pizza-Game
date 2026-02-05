using System;
using System.Collections.Generic;
using UnityEngine;

public class HfsmController : MonoBehaviour
{
    [SerializeField] private AiContext context;
    [SerializeField] private GoapPlanner planner;
    [SerializeField] private float minMoveSpeed = 2f;
    [SerializeField] private float maxMoveSpeed = 4.5f;

    public HfsmState CurrentState { get; private set; } = HfsmState.Idle;
    public float CurrentMoveSpeed { get; private set; }

    public void ApplyMoveSpeed(float minSpeed, float maxSpeed)
    {
        minMoveSpeed = minSpeed;
        maxMoveSpeed = maxSpeed;
    }

    private void OnEnable()
    {
        if (planner != null)
        {
            planner.IntentChanged += OnIntentChanged;
        }
    }

    private void OnDisable()
    {
        if (planner != null)
        {
            planner.IntentChanged -= OnIntentChanged;
        }
    }

    private void Update()
    {
        if (context == null)
        {
            return;
        }

        var aggression = context.DirectorModifiers.Aggression;
        CurrentMoveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, aggression);
    }

    private void OnIntentChanged(IntentData intent)
    {
        CurrentState = MapIntentToState(intent.GoalType);
    }

    private static HfsmState MapIntentToState(GoalType goalType)
    {
        switch (goalType)
        {
            case GoalType.Attack:
                return HfsmState.Advance;
            case GoalType.Flank:
                return HfsmState.Flank;
            case GoalType.Retreat:
                return HfsmState.Retreat;
            case GoalType.Suppress:
                return HfsmState.Fire;
            default:
                return HfsmState.Idle;
        }
    }
}
