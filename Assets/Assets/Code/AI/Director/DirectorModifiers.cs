using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DirectorModifiers
{
    [Range(0f, 1f)] public float Aggression;
    [Range(0f, 2f)] public float FlankBias;
    [Range(0f, 2f)] public float PushBias;
    [Range(0f, 2f)] public float RetreatBias;

    public static DirectorModifiers Lerp(DirectorModifiers a, DirectorModifiers b, float t)
    {
        return new DirectorModifiers
        {
            Aggression = Mathf.Lerp(a.Aggression, b.Aggression, t),
            FlankBias = Mathf.Lerp(a.FlankBias, b.FlankBias, t),
            PushBias = Mathf.Lerp(a.PushBias, b.PushBias, t),
            RetreatBias = Mathf.Lerp(a.RetreatBias, b.RetreatBias, t)
        };
    }
}

[Serializable]
public struct WorldFacts
{
    public bool HasLineOfSight;
    public float DistanceToTarget;
    [Range(0f, 1f)] public float CoverScore;
}

[Serializable]
public struct VolumeModifiers
{
    public float AggressionMult;
    public float FlankBiasMult;
    public float PushBiasMult;
    public float RetreatBiasMult;

    public static VolumeModifiers Identity()
    {
        return new VolumeModifiers
        {
            AggressionMult = 1f,
            FlankBiasMult = 1f,
            PushBiasMult = 1f,
            RetreatBiasMult = 1f
        };
    }

    public float GetBias(GoalType goalType)
    {
        switch (goalType)
        {
            case GoalType.Attack:
                return AggressionMult * PushBiasMult;
            case GoalType.Flank:
                return FlankBiasMult;
            case GoalType.Retreat:
                return RetreatBiasMult;
            case GoalType.Suppress:
                return AggressionMult;
            default:
                return 1f;
        }
    }
}

public enum GoalType
{
    None,
    Attack,
    Flank,
    Retreat,
    Suppress
}

public enum HfsmState
{
    Idle,
    Advance,
    Flank,
    TakeCover,
    Fire,
    Retreat
}

public struct IntentData
{
    public GoalType GoalType;
    public float Priority;
}
