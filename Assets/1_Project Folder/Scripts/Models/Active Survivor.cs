[System.Serializable]
public class ActiveSurvivor
{
    public string Name;
    public SurvivorStats Stats;
    public float CurrentHealth = 100f;
    public bool IsResting = false; // สถานะพักฟื้นหลังภารกิจ

    // Constructor: สร้างตัวละครจริงจาก Template
    public ActiveSurvivor(SurvivorTemplate template)
    {
        this.Name = template.DefaultName;
        this.Stats = template.BaseStats;
    }
}