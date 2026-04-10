using System.Collections.Generic;
using UnityEngine;
using static ExpeditionManager;
using static UnityEngine.Rendering.STP;

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
    public float GetSuccessChance(List<ActiveSurvivor> party)
    {
        if (party == null || party.Count == 0) return 0f;

        GameBalanceConfig config = GameManager.Instance.CurrentConfig;
        AreaTemplate area = CurrentArea;

        int totalStr = 0, totalPer = 0, totalAgi = 0;
        float totalFatigue = 0;

        foreach (var member in party)
        {
            totalStr += member.Stats.Strength;
            totalPer += member.Stats.Perception;
            totalAgi += member.Stats.Agility;
            totalFatigue += member.Fatigue;
        }

        float successChance = 0;
        float totalRequirement = area.Stats.RequiredStrength + area.Stats.RequiredPerception + area.Stats.RequiredAgility;

        // --- คำนวณความสำเร็จพื้นฐานจาก Stat ---
        // ใช้ Mathf.Min เพื่อให้แต่ละ Stat ส่งคะแนนให้ได้ไม่เกินที่โควต้ากำหนด
        successChance += Mathf.Min(totalStr, area.Stats.RequiredStrength);
        successChance += Mathf.Min(totalPer, area.Stats.RequiredPerception);
        successChance += Mathf.Min(totalAgi, area.Stats.RequiredAgility);

        // ทำเป็น Ratio (0.0 - 1.0)
        successChance = (successChance / totalRequirement);

        // --- คำนวณ Fatigue Penalty ---
        // สมมติค่าเฉลี่ยความเหนื่อย 20 จะได้ค่า 0.2f
        float avgFatigue = totalFatigue / party.Count;
        float fatiguePenaltyPercent = (avgFatigue / 100f);

        // --- หักลบแบบ % โดยเอา successChance ตั้ง ---
        // สูตร: โอกาสเดิม * (1 - %ที่จะลด)
        // เช่น ถ้า successChance คือ 0.8 (80%) และเหนื่อย 0.2 (20%) 
        // ผลคือ 0.8 * (1 - 0.2) = 0.64 (เหลือ 64%)
        successChance = successChance * (1f - fatiguePenaltyPercent);

        return Mathf.Clamp01(successChance);
    }
    public void ExecutePartyExpedition(List<ActiveSurvivor> party)
    {
        GameBalanceConfig config = GameManager.Instance.CurrentConfig;
        AreaTemplate area = CurrentArea;
        List<string> updates = new List<string>();

        // --- ใช้ฟังก์ชัน GetSuccessChance มาคำนวณเพื่อให้ค่าตรงกัน 100% ---
        float successChance = GetSuccessChance(party);

        // คำนวณความเหนื่อยเฉลี่ยเพื่อแสดง Log เท่านั้น
        float avgFatigue = 0;
        foreach (var m in party) avgFatigue += m.Fatigue;
        avgFatigue /= party.Count;

        if (avgFatigue > 50f)
            updates.Add($"The team is exhausted, heavily affecting performance.");

        // (สร้าง Log แจ้งเตือนเรื่อง Stat ที่ไม่พอ - สามารถใส่ Logic เช็คสั้นๆ ตรงนี้เพื่อเพิ่มข้อความใน Log ได้)
        CheckStatDeficiency(party, area, updates);

        Debug.Log($"Calculated Success Chance: {successChance * 100:F2}%");
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
            SupplyUsed = GameManager.Instance.notSelectedSurvivor.Count * config.SupplyConsumptionPerPerson,
            StatusUpdates = updates,
            FoundSurvivor = foundSurvivor
        };
        OnExpeditionComplete?.Invoke(CurrentResult);
    }
    // ฟังก์ชันเสริมสำหรับเขียน Log ในหน้า Result ให้ละเอียดขึ้น
    void CheckStatDeficiency(List<ActiveSurvivor> party, AreaTemplate area, List<string> updates)
    {
        int s = 0, p = 0, a = 0;
        foreach (var m in party) { s += m.Stats.Strength; p += m.Stats.Perception; a += m.Stats.Agility; }

        if (s < area.Stats.RequiredStrength) updates.Add("- Low Manpower (Strength deficiency)");
        if (p < area.Stats.RequiredPerception) updates.Add("- Poor Visibility (Perception deficiency)");
        if (a < area.Stats.RequiredAgility) updates.Add("- Difficult Terrain (Agility deficiency)");
    }
    public void SkipExpedition()
    {
        GameBalanceConfig config = GameManager.Instance.CurrentConfig;

        CurrentResult = new ExpeditionResult
        {
            IsSuccess = false,
            Log = "Expedition Skipped",
            SupplyGained = 0,
            SupplyUsed = GameManager.Instance.notSelectedSurvivor.Count * config.SupplyConsumptionPerPerson * config.SkipExpeditionPenaltyMultiplier,
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
    public float SupplyUsed;
    public List<string> StatusUpdates; // เก็บเหตุการณ์ที่เกิดขึ้นกับแต่ละคน
    public SurvivorTemplate FoundSurvivor;
}