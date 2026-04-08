using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivorButton : MonoBehaviour
{
    ActiveSurvivor _info;
    [SerializeField] bool isSelectedButton; // กำหนดใน Inspector ว่าปุ่มนี้เป็นปุ่มสำหรับคนที่ถูกเลือกอยู่หรือไม่
    [SerializeField] TextMeshProUGUI nameText;

    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(isSelectedButton ? DeselectThisSurvivor : SelectThisSurvivor);
    }
    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }
    public void SetupInfo(ActiveSurvivor info)
    {
        this._info = info;
        nameText.text = info.Name;
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
