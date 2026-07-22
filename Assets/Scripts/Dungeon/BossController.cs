using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class BossController : MonoBehaviour
{
    private Health health;
    private List<float> thresholds = new List<float>();
    private int phaseIndex = -1;
    private bool killed;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    public void ConfigurePhases(float[] phaseFractions, int roomIndex)
    {
        thresholds.Clear();
        phaseIndex = -1;
        killed = false;

        if (phaseFractions == null) return;

        for (int i = 0; i < phaseFractions.Length; i++)
        {
            float v = Mathf.Clamp01(phaseFractions[i]);
            thresholds.Add(v);
        }
    }

    void Update()
    {
        if (health == null) return;
        if (killed) return;
        if (thresholds.Count == 0) return;

        if (!health.IsAlive)
        {
            killed = true;
            try { DungeonEvents.RaiseBossKilled(); }
            catch (System.Exception e) { Debug.LogError($"RaiseBossKilled threw: {e.Message}"); }
            return;
        }

        float frac = health.MaxHealth > 0f ? health.CurrentHealth / health.MaxHealth : 0f;

        for (int i = 0; i < thresholds.Count; i++)
        {
            if (frac <= thresholds[i])
            {
                if (i > phaseIndex)
                {
                    phaseIndex = i;
                    try { DungeonEvents.RaiseBossPhaseChanged(phaseIndex); }
                    catch (System.Exception e) { Debug.LogError($"RaiseBossPhaseChanged threw: {e.Message}"); }
                }
                break;
            }
        }
    }
}
