using UnityEngine;

public class AiDebugHud : MonoBehaviour
{
    [SerializeField] private AiContext context;
    [SerializeField] private GoapPlanner planner;
    [SerializeField] private HfsmController hfsm;
    [SerializeField] private Vector2 screenOffset = new Vector2(12f, 12f);
    [SerializeField] private int fontSize = 14;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private bool showHud = true;

    private GUIStyle style;

    private void OnGUI()
    {
        if (!showHud)
        {
            return;
        }

        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                normal = { textColor = textColor }
            };
        }

        var lines = BuildLines();
        var y = screenOffset.y;
        foreach (var line in lines)
        {
            GUI.Label(new Rect(screenOffset.x, y, 500f, 20f), line, style);
            y += fontSize + 4f;
        }
    }

    private string[] BuildLines()
    {
        if (context == null || planner == null || hfsm == null)
        {
            return new[]
            {
                "AI HUD: missing references",
                "Assign AiContext, GoapPlanner, HfsmController"
            };
        }

        var mods = context.DirectorModifiers;
        var volume = context.VolumeModifiers;
        var intent = planner.CurrentIntent;

        return new[]
        {
            $"Goal: {intent.GoalType} ({intent.Priority:0.00})",
            $"State: {hfsm.CurrentState}  Speed: {hfsm.CurrentMoveSpeed:0.00}",
            $"Aggression: {mods.Aggression:0.00}  Flank: {mods.FlankBias:0.00}",
            $"Push: {mods.PushBias:0.00}  Retreat: {mods.RetreatBias:0.00}",
            $"Vol Agg: {volume.AggressionMult:0.00}  Vol Flank: {volume.FlankBiasMult:0.00}",
            $"Vol Push: {volume.PushBiasMult:0.00}  Vol Retreat: {volume.RetreatBiasMult:0.00}"
        };
    }
}
