using UnityEngine;

public class EnemyRole : MonoBehaviour
{
    [SerializeField] private EnemyRoleProfile roleProfile;
    [SerializeField] private GoapPlanner planner;
    [SerializeField] private HfsmController hfsm;
    [SerializeField] private AiNavAgentController navController;
    [SerializeField] private AiAttackController attackController;
    [SerializeField] private bool applyOnEnable = true;

    private void Reset()
    {
        planner = GetComponent<GoapPlanner>();
        hfsm = GetComponent<HfsmController>();
        navController = GetComponent<AiNavAgentController>();
        attackController = GetComponent<AiAttackController>();
    }

    private void OnEnable()
    {
        if (applyOnEnable)
        {
            ApplyRole();
        }
    }

    private void OnValidate()
    {
        if (!applyOnEnable || Application.isPlaying)
        {
            return;
        }

        ApplyRole();
    }

    public void ApplyRole()
    {
        if (roleProfile == null)
        {
            return;
        }

        roleProfile.ApplyTo(planner, hfsm, navController, attackController);
    }
}
