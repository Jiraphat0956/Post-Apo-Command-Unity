using System.Collections.Generic;
using UnityEngine;

public class ExpeditionManager : Singleton<ExpeditionManager>
{
    public AreaTemplate CurrentArea;
    public ExpeditionResult CurrentResult;

    public delegate void ExpeditionGenericHandler<T>(T data);
    public delegate void ExpeditionHandler();

    public event ExpeditionGenericHandler<AreaTemplate> OnAreaChange;
    public event ExpeditionHandler OnExpeditionComplete;
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
        AreaTemplate area = CurrentArea;
        // 1. รวม Stat ของทุกคนในทีม
        int totalStr = 0;
        int totalPer = 0;
        int totalAgi = 0;

        foreach (var member in party)
        {
            totalStr += member.Stats.Strength;
            totalPer += member.Stats.Perception;
            totalAgi += member.Stats.Agility;
        }

        // 2. คำนวณโอกาสสำเร็จ (Success Rate)
        // เริ่มต้นที่ 100% แล้วหักออกตามส่วนต่างของ Stat ที่ขาดไป
        float successChance = 1.0f;
        List<string> updates = new List<string>();

        // ตรวจสอบ Strength (เช่น การฝ่าสิ่งกีดขวาง)
        if (totalStr < area.Stats.RequiredStrength)
        {
            float penalty = (area.Stats.RequiredStrength - totalStr) * 0.1f;
            successChance -= penalty;
            updates.Add($"The team didn't have enough manpower to clear the way. (Success Chance -{penalty * 100}%)");
        }

        // ตรวจสอบ Perception (เช่น การหาของหรือเลี่ยงอันตราย)
        if (totalPer < area.Stats.RequiredPerception)
        {
            successChance -= 0.15f;
            updates.Add("\r\nThe team couldn't see a shortcut, which wasted time and increased their risks.");
        }

        // 3. ทอยลูกเต๋าตัดสินผล (Final Roll)
        bool isSuccess = Random.value <= successChance;
        float foodFound = 0;

        if (isSuccess)
        {
            foodFound = area.FoodRewardRange * Random.Range(0.8f, 1.2f);
            updates.Add($"\r\nSuccess! {foodFound:F0} supply unit found.");
        }
        else
        {
            updates.Add("\r\nThe mission failed. The team had to retreat before it became dangerous.");
        }

        // 4. จัดการสถานะลูกทีมหลังจบภารกิจ
        foreach (var member in party)
        {
            // ถ้าล้มเหลว มีโอกาสบาดเจ็บ
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
            FoodGained = foodFound,
            StatusUpdates = updates
        };
        OnExpeditionComplete?.Invoke();
    }
    public void SkipExpedition()
    {
        CurrentResult = new ExpeditionResult
        {
            IsSuccess = false,
            Log = "Expedition Skipped",
            FoodGained = 0,
            StatusUpdates = new List<string> { "The team decided to skip this mission." }
        };
        OnExpeditionComplete?.Invoke();
    }
}

public struct ExpeditionResult
{
    public bool IsSuccess;
    public string Log;
    public float FoodGained;
    public List<string> StatusUpdates; // เก็บเหตุการณ์ที่เกิดขึ้นกับแต่ละคน
}