using UnityEngine;

[System.Serializable]
public class AreaStats
{
    public int RequiredStrength;
    public int RequiredPerception;
    public int RequiredAgility;
    [Range(0, 1)] public float BaseRisk; // โอกาสเกิดเหตุการณ์เลวร้ายพื้นฐาน
}