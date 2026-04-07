using TMPro;
using UnityEngine;

public class SurvivorButton : MonoBehaviour
{
    ActiveSurvivor _info;
    [SerializeField] bool isSelectedButton; // กำหนดใน Inspector ว่าปุ่มนี้เป็นปุ่มสำหรับคนที่ถูกเลือกอยู่หรือไม่
    [SerializeField] TextMeshProUGUI nameText;

    public void SetupInfo(ActiveSurvivor info)
    {
        this._info = info;
        nameText.text = info.Name;
    }
}
