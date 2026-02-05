using System;
using System.Collections.Generic;
using UnityEngine;

public class AiDirector : MonoBehaviour
{
    [Header("Inputs")]
    [Range(0f, 1f)] public float PlayerHealth01 = 1f;
    [Range(0f, 1f)] public float EncounterPressure01;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Update")]
    [Range(0.1f, 2f)] public float UpdateIntervalMin = 0.5f;
    [Range(0.1f, 2f)] public float UpdateIntervalMax = 1f;
    [Range(0f, 1f)] public float Smoothing = 0.5f;

    public DirectorModifiers CurrentModifiers { get; private set; }

    public event Action<DirectorModifiers> ModifiersUpdated;

    private void OnEnable()
    {
        StartCoroutine(UpdateLoop());
    }

    private System.Collections.IEnumerator UpdateLoop()
    {
        while (true)
        {
            var next = ComputeTargetModifiers();
            CurrentModifiers = DirectorModifiers.Lerp(CurrentModifiers, next, Smoothing);
            ModifiersUpdated?.Invoke(CurrentModifiers);

            var delay = UnityEngine.Random.Range(UpdateIntervalMin, UpdateIntervalMax);
            yield return new WaitForSeconds(delay);
        }
    }

    private DirectorModifiers ComputeTargetModifiers()
    {
        var health01 = playerHealth != null ? playerHealth.Health01 : PlayerHealth01;
        health01 = Mathf.Clamp01(health01);
        var pressure = Mathf.Clamp01(EncounterPressure01);

        return new DirectorModifiers
        {
            Aggression = Mathf.Clamp01(pressure * (1f + (1f - health01) * 0.5f)),
            FlankBias = Mathf.Clamp01(0.5f + pressure * 0.5f),
            PushBias = Mathf.Clamp01(pressure),
            RetreatBias = Mathf.Clamp01((1f - pressure) * (1f - health01) + 0.15f)
        };
    }
}
