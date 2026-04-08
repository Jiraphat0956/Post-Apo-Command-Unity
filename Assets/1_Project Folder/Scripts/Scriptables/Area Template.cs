using UnityEngine;

[CreateAssetMenu(fileName = "NewArea", menuName = "Game/Expedition Area")]
public class AreaTemplate : ScriptableObject
{
    public string AreaName;
    [TextArea] public string Description; // คำอธิบายพื้นที่ให้ผู้เล่นอ่าน
    public AreaStats Stats;
    public int FoodRewardRange; // จำนวนเสบียงที่จะได้รับถ้าสำเร็จ
}