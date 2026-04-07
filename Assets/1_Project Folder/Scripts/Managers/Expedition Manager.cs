using UnityEngine;

public class ExpeditionManager : Singleton<ExpeditionManager>
{

    public ExpeditionResult CalculateResult(ActiveSurvivor explorer, AreaTemplate area)
    {
        float successChance = 1.0f;
        string report = "";

        // 1. Check Strength Gap
        int strGap = area.Stats.RequiredStrength - explorer.Stats.Strength;
        if (strGap > 0)
        {
            successChance -= (strGap * 0.15f); // เสียโอกาสสำเร็จ 15% ต่อแต้มที่ขาด
            report += "ขาดแรงในการเคลียร์ซากปรักหักพัง. ";
        }

        // 2. Check Perception Gap (ส่งผลต่อความเสียหายที่ได้รับ)
        int perGap = area.Stats.RequiredPerception - explorer.Stats.Perception;
        float damageTaken = 0f;
        if (perGap > 0)
        {
            damageTaken = perGap * 20f; // ได้รับบาดเจ็บตามค่า Perception ที่ขาด
            report += "ตรวจไม่พบกับดัก/กัมมันตภาพรังสีในพื้นที่. ";
        }

        // 3. Final Roll
        float roll = Random.value; // สุ่มค่า 0.0 - 1.0
        bool isSuccess = roll <= successChance;

        // Apply Damage
        explorer.CurrentHealth -= damageTaken;
        if (!isSuccess) explorer.CurrentHealth -= 30f; // ถ้าพลาดภารกิจ จะเจ็บตัวเพิ่ม

        return new ExpeditionResult
        {
            IsSuccess = isSuccess,
            DamageTaken = damageTaken,
            Log = isSuccess ? "ภารกิจสำเร็จ! " + report : "ภารกิจล้มเหลว... " + report,
            SurvivorStillAlive = explorer.CurrentHealth > 0
        };
    }
}

public struct ExpeditionResult
{
    public bool IsSuccess;
    public float DamageTaken;
    public string Log;
    public bool SurvivorStillAlive;
}