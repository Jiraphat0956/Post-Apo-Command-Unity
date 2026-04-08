using UnityEngine;

[CreateAssetMenu(fileName = "GameBalanceConfig", menuName = "ScriptableObjects/GameBalanceConfig")]
public class GameBalanceConfig : ScriptableObject
{
    [Header("Starting Values")]
    public float StartingSupply = 100f; // เสบียงเริ่มต้น
    public int StartingSurvivorCount = 3; // จำนวนลูกทีมเริ่มต้น
    public int TargetDayToWin = 20; // จำนวนเทิร์นเป้าหมายเพื่อชนะ

    [Header("Fatigue & Health")]
    public float BaseFatigueGain = 20f; // ค่าเหนื่อยฐานเมื่อออกสำรวจ
    public float FatigueMultiplier = 1.5f; // ตัวคูณความเหนื่อยสะสม
    public float RestRecoveryRate = 25f; // อัตราการลดความเหนื่อยเมื่อพัก
    public float HealthRecoveryRate = 10f; // อัตราการเพิ่มเลือดเมื่อพัก

    [Header("Resources")]
    public float FoodConsumptionPerPerson = 5f; // อาหารที่กินต่อคนต่อเทิร์น
    public float SkipExpeditionPenaltyMultiplier = 2f; // ตัวคูณบทลงโทษเมื่อ Skip

    [Header("Recruitment")]
    [Range(0, 1)] public float NewSurvivorChance = 0.25f; // โอกาสเจอคนใหม่ (0-1)

    [Header("Success Probability")]
    [Range(0, 1)] public float MaxFatiguePenalty = 0.4f; // โดนหักโอกาสสำเร็จสูงสุดจากความเหนื่อย
}