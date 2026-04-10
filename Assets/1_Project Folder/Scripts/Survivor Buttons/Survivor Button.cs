using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivorButton : MonoBehaviour
{
    ActiveSurvivor _info;
    [SerializeField] bool isSelectedButton; // กำหนดใน Inspector ว่าปุ่มนี้เป็นปุ่มสำหรับคนที่ถูกเลือกอยู่หรือไม่
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI strengethText;
    [SerializeField] TextMeshProUGUI perceptionText;
    [SerializeField] TextMeshProUGUI agilityText;
    [SerializeField] Image hpGage;
    [SerializeField] Image staminaGage;

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
        hpGage.fillAmount = info.CurrentHealth / info.Stats.MaxHealth;
        staminaGage.fillAmount = 1 - (info.Fatigue / 100);
        strengethText.text = info.Stats.Strength.ToString();
        perceptionText.text = info.Stats.Perception.ToString();
        agilityText.text = info.Stats.Agility.ToString();   
        if (!isSelectedButton)
        {
            _button.interactable = !info.IsResting && !GameManager.Instance.IsPartyFull();
        }
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
