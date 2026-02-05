using System;
using System.Collections.Generic;
using UnityEngine;

public class AiAnimationBridge : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private HfsmController hfsmController;
    [SerializeField] private AiContext context;

    private static readonly int AggressionParam = Animator.StringToHash("Aggression");
    private static readonly int StateParam = Animator.StringToHash("State");

    private void Update()
    {
        if (animator == null || hfsmController == null)
        {
            return;
        }

        if (context != null)
        {
            animator.SetFloat(AggressionParam, context.DirectorModifiers.Aggression);
        }

        animator.SetInteger(StateParam, (int)hfsmController.CurrentState);
    }
}
