using System.Collections.Generic;
using UnityEngine;

public class ExpeditionManager : Singleton<ExpeditionManager>
{
    public AreaTemplate CurrentArea;

    public delegate void ExpeditionGenericHandler<T>(T data);

    public event ExpeditionGenericHandler<AreaTemplate> OnAreaChange;
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
    public ExpeditionResult ExecutePartyExpedition(List<ActiveSurvivor> party, AreaTemplate area)
    {
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
            updates.Add($"ทีมแรงไม่พอที่จะเคลียร์ทางสะดวก (Success Chance -{penalty * 100}%)");
        }

        // ตรวจสอบ Perception (เช่น การหาของหรือเลี่ยงอันตราย)
        if (totalPer < area.Stats.RequiredPerception)
        {
            successChance -= 0.15f;
            updates.Add("ทีมมองไม่เห็นเส้นทางลัด ทำให้เสียเวลาและเสี่ยงมากขึ้น");
        }

        // 3. ทอยลูกเต๋าตัดสินผล (Final Roll)
        bool isSuccess = Random.value <= successChance;
        float foodFound = 0;

        if (isSuccess)
        {
            foodFound = area.FoodRewardRange * Random.Range(0.8f, 1.2f);
            updates.Add($"สำเร็จ! พบเสบียง {foodFound:F0} หน่วย");
        }
        else
        {
            updates.Add("ภารกิจล้มเหลว ทีมต้องถอยกลับมาก่อนจะเกิดอันตราย");
        }

        // 4. จัดการสถานะลูกทีมหลังจบภารกิจ
        foreach (var member in party)
        {
            member.IsResting = true; // ทุกคนที่ไปต้องพักในเทิร์นหน้า (Mechanic: Fatigue Management)

            // ถ้าล้มเหลว มีโอกาสบาดเจ็บ
            if (!isSuccess)
            {
                float damage = Random.Range(10f, 30f);
                member.CurrentHealth -= damage;
                updates.Add($"{member.Name} บาดเจ็บจากการถอยทัพ (-{damage:F0} HP)");
            }
        }

        return new ExpeditionResult
        {
            IsSuccess = isSuccess,
            Log = isSuccess ? "Mission Accomplished" : "Mission Failed",
            FoodGained = foodFound,
            StatusUpdates = updates
        };
    }
}

public struct ExpeditionResult
{
    public bool IsSuccess;
    public string Log;
    public float FoodGained;
    public List<string> StatusUpdates; // เก็บเหตุการณ์ที่เกิดขึ้นกับแต่ละคน
}