using System.Collections.Generic;
using UnityEngine;
using static ExpeditionManager;

public class ExpeditionManager : Singleton<ExpeditionManager>
{
    public AreaTemplate CurrentArea;
    public ExpeditionResult CurrentResult;

    public delegate void ExpeditionGenericHandler<T>(T data);
    public delegate void ExpeditionHandler();

    public event ExpeditionGenericHandler<AreaTemplate> OnAreaChange;
    public event ExpeditionGenericHandler<ExpeditionResult> OnExpeditionComplete;
    public void SetArea()
    {
        List<AreaTemplate>areas = GameManager.Instance.allAreas;
        if (areas.Count > 0)
        {
            CurrentArea = areas[Random.Range(0, areas.Count)];
            OnAreaChange?.Invoke(CurrentArea);
            Debug.Log($"Current Area: {CurrentArea.AreaName}");
        }
        else
        {
            Debug.LogError("No areas available in GameManager!");
        }
    }
    public void ExecutePartyExpedition(List<ActiveSurvivor> party)
    {
        GameBalanceConfig config = GameManager.Instance.CurrentConfig;
        AreaTemplate area = CurrentArea;
        // 1. รวม Stat ของทุกคนในทีม
        int totalStr = 0;
        int totalPer = 0;
        int totalAgi = 0;
        float totalFatigue = 0;

        foreach (var member in party)
        {
            totalStr += member.Stats.Strength;
            totalPer += member.Stats.Perception;
            totalAgi += member.Stats.Agility;
            totalFatigue += member.Fatigue;
        }
        // คำนวณความเหนื่อยเฉลี่ยของทีม
        float avgFatigue = totalFatigue / party.Count;

        // 2. คำนวณโอกาสสำเร็จพื้นฐาน
        float successChance = 1.0f;
        List<string> updates = new List<string>();

        // หักลบโอกาสสำเร็จจากความเหนื่อย (ยิ่งเหนื่อยมาก โอกาสล้มเหลวยิ่งสูง)
        float fatiguePenalty = (avgFatigue / 100f) * config.MaxFatiguePenalty; // สูงสุดหัก 99%
        successChance -= fatiguePenalty;

        if (avgFatigue > 50f)
            updates.Add($"The team is exhausted, decreasing success chance by {fatiguePenalty * 100:F0}%.");

        // ตรวจสอบ Stat ตามเดิม
        if (totalStr < area.Stats.RequiredStrength)
        {
            float penalty = (area.Stats.RequiredStrength - totalStr) * 0.1f;
            successChance -= penalty;
            updates.Add($"Manpower insufficient. (Success Chance -{penalty * 100}%)");
        }


        // 3. ทอยลูกเต๋าตัดสินผล (Final Roll)
        bool isSuccess = Random.value <= successChance;
        float supplyFound = 0;
        SurvivorTemplate foundSurvivor = null; // ตัวแปรชั่วคราวเก็บคนใหม่

        if (isSuccess)
        {
            supplyFound = area.FoodRewardRange * Random.Range(0.8f, 1.2f);
            updates.Add($"\r\nSuccess! {supplyFound:F0} supply unit found.");
            // เพิ่มโอกาสพบผู้รอดชีวิตใหม่ (เช่น 25%)
            if (Random.value <= config.NewSurvivorChance)
            {
                var templates = GameManager.Instance.allSurvivors;
                if (templates.Count > 0)
                {
                    foundSurvivor = templates[Random.Range(0, templates.Count)];
                    updates.Add($"<color=green>New Survivor Found: {foundSurvivor.DefaultName}!</color>");
                }
            }
        }
        else
        {
            updates.Add("\r\nThe mission failed. The team had to retreat before it became dangerous.");
        }

        // 4. จัดการสถานะลูกทีมหลังจบภารกิจ
        foreach (var member in party)
        {
            // ถ้าล้มเหลว มีโอกาสบาดเจ็บ
            IncreaseFatigue(member);
            if (!isSuccess)
            {
                float damage = Random.Range(10f, 30f);
                member.CurrentHealth -= damage;
                if (member.CurrentHealth <= 0)
                {
                    GameManager.Instance.RemoveActiveSurvivor(member); // Kill if health drops to 0 or below
                    updates.Add($"{member.Name} died in an accident during an expedition.");

                }
                else
                    updates.Add($"{member.Name} was injured during the retreat. (-{damage:F0} HP)");
            }
        }

        CurrentResult = new ExpeditionResult
        {
            IsSuccess = isSuccess,
            Log = isSuccess ? "Mission Accomplished" : "Mission Failed",
            SupplyGained = supplyFound,
            StatusUpdates = updates,
            FoundSurvivor = foundSurvivor
        };
        OnExpeditionComplete?.Invoke(CurrentResult);
    }
    public void SkipExpedition()
    {
        CurrentResult = new ExpeditionResult
        {
            IsSuccess = false,
            Log = "Expedition Skipped",
            SupplyGained = 0,
            StatusUpdates = new List<string> { "The team decided to skip this mission." }
        };
        OnExpeditionComplete?.Invoke(CurrentResult);
    }
    void IncreaseFatigue(ActiveSurvivor survivor)
    {
        GameBalanceConfig config = GameManager.Instance.CurrentConfig;
        // ยิ่งเหนื่อยมาก ครั้งต่อไปจะยิ่งเหนื่อยเพิ่มขึ้นทวีคูณ
        // สูตร: ค่าความเหนื่อยฐาน + (ค่าความเหนื่อยปัจจุบัน * ตัวคูณ)
        float baseFatigueGain = config.BaseFatigueGain;
        float multiplier = config.FatigueMultiplier;

        float additionalFatigue = baseFatigueGain + (survivor.Fatigue * multiplier);
        survivor.Fatigue = Mathf.Min(100f, survivor.Fatigue + additionalFatigue);
    }
}

public struct ExpeditionResult
{
    public bool IsSuccess;
    public string Log;
    public float SupplyGained;
    public List<string> StatusUpdates; // เก็บเหตุการณ์ที่เกิดขึ้นกับแต่ละคน
    public SurvivorTemplate FoundSurvivor;
}