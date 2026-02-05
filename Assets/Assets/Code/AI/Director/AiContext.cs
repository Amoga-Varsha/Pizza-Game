using System;
using System.Collections.Generic;
using UnityEngine;

public class AiContext : MonoBehaviour
{
    [SerializeField] private AiDirector director;
    [SerializeField] private WorldFacts worldFacts;

    public DirectorModifiers DirectorModifiers { get; private set; }
    public WorldFacts WorldFacts => worldFacts;
    public VolumeModifiers VolumeModifiers { get; private set; } = VolumeModifiers.Identity();

    private readonly HashSet<AiContextVolume> activeVolumes = new HashSet<AiContextVolume>();

    private void OnEnable()
    {
        if (director != null)
        {
            director.ModifiersUpdated += OnDirectorUpdated;
            DirectorModifiers = director.CurrentModifiers;
        }
    }

    private void OnDisable()
    {
        if (director != null)
        {
            director.ModifiersUpdated -= OnDirectorUpdated;
        }
    }

    public void SetWorldFacts(bool hasLineOfSight, float distanceToTarget, float coverScore)
    {
        worldFacts.HasLineOfSight = hasLineOfSight;
        worldFacts.DistanceToTarget = distanceToTarget;
        worldFacts.CoverScore = Mathf.Clamp01(coverScore);
    }

    public void AddVolume(AiContextVolume volume)
    {
        if (volume == null || !activeVolumes.Add(volume))
        {
            return;
        }

        RebuildVolumeModifiers();
    }

    public void RemoveVolume(AiContextVolume volume)
    {
        if (volume == null || !activeVolumes.Remove(volume))
        {
            return;
        }

        RebuildVolumeModifiers();
    }

    public float GetContextModifier(GoalType goalType)
    {
        var modifier = VolumeModifiers.GetBias(goalType);

        if (!worldFacts.HasLineOfSight && goalType == GoalType.Attack)
        {
            modifier *= 0.6f;
        }

        if (!worldFacts.HasLineOfSight && goalType == GoalType.Suppress)
        {
            modifier *= 1.2f;
        }

        if (worldFacts.DistanceToTarget > 12f && goalType == GoalType.Flank)
        {
            modifier *= 1.15f;
        }

        if (worldFacts.CoverScore > 0.7f && goalType == GoalType.Retreat)
        {
            modifier *= 0.8f;
        }

        return modifier;
    }

    private void OnDirectorUpdated(DirectorModifiers modifiers)
    {
        DirectorModifiers = modifiers;
    }

    private void RebuildVolumeModifiers()
    {
        var combined = VolumeModifiers.Identity();
        foreach (var volume in activeVolumes)
        {
            var mods = volume.Modifiers;
            combined.AggressionMult *= mods.AggressionMult;
            combined.FlankBiasMult *= mods.FlankBiasMult;
            combined.PushBiasMult *= mods.PushBiasMult;
            combined.RetreatBiasMult *= mods.RetreatBiasMult;
        }

        VolumeModifiers = combined;
    }
}