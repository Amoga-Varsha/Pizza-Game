using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AiContextVolume : MonoBehaviour
{
    [SerializeField] private VolumeModifiers modifiers = new VolumeModifiers
    {
        AggressionMult = 1f,
        FlankBiasMult = 1f,
        PushBiasMult = 1f,
        RetreatBiasMult = 1f
    };

    public VolumeModifiers Modifiers => modifiers;

    private void OnTriggerEnter(Collider other)
    {
        var context = other.GetComponentInParent<AiContext>();
        if (context != null)
        {
            context.AddVolume(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var context = other.GetComponentInParent<AiContext>();
        if (context != null)
        {
            context.RemoveVolume(this);
        }
    }
}