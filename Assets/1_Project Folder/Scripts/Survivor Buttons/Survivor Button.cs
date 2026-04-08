using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivorButton : MonoBehaviour
{
    ActiveSurvivor _info;
    [SerializeField] bool isSelectedButton; // กำหนดใน Inspector ว่าปุ่มนี้เป็นปุ่มสำหรับคนที่ถูกเลือกอยู่หรือไม่
    [SerializeField] TextMeshProUGUI nameText;

    Button _button;

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(isSelectedButton ? DeselectThisSurvivor : SelectThisSurvivor);
    }
    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }
    public void SetupInfo(ActiveSurvivor info)
    {
        this._info = info;
        nameText.text = info.Name;
        _button.interactable = !info.IsResting; // ถ้าคนนี้กำลังพักผ่อนอยู่ ให้ปุ่มไม่สามารถกดได้
    }
    void SelectThisSurvivor()
    {
        GameManager.Instance.AddSelectedSurvivor(_info);
    }
    void DeselectThisSurvivor()
    {
        GameManager.Instance.RemoveSelectedSurvivor(_info);
    }
}
