using UnityEngine;

[System.Serializable]
public class ActiveSurvivor
{
    public string Name;
    public string Description;
    public SurvivorStats Stats;
    public float CurrentHealth = 100f;
    public bool IsResting = false; // สถานะพักฟื้นหลังภารกิจ
    [Range(0, 100)] public float Fatigue = 0; // 0 = สดชื่น, 100 = หมดสภาพ
                                              // Constructor: สร้างตัวละครจริงจาก Template
    public ActiveSurvivor(SurvivorTemplate template)
    {
        this.Name = template.DefaultName;
        this.Description = template.Description;
        this.Stats = template.BaseStats;
        this.CurrentHealth = template.BaseStats.MaxHealth;
    }

    // ฟังก์ชันคำนวณการฟื้นฟูเมื่อไม่ได้ออกสำรวจ
    public void RestAndRecover()
    {
        GameBalanceConfig config = GameManager.Instance.CurrentConfig;
        // ลดความเหนื่อยลงเหลือ 0 (ฟื้นฟูเต็มที่)
        Fatigue = Mathf.Max(0, Fatigue - config.RestRecoveryRate);

        // ฟื้นฟูเลือด (ถ้ามีอาหารพอกิน)
        CurrentHealth = Mathf.Min(Stats.MaxHealth, CurrentHealth + config.HealthRecoveryRate);
    }


}