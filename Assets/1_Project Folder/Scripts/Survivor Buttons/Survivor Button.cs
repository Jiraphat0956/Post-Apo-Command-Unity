using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SurvivorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    ActiveSurvivor _info;
    [SerializeField] bool isSelectedButton; // กำหนดใน Inspector ว่าปุ่มนี้เป็นปุ่มสำหรับคนที่ถูกเลือกอยู่หรือไม่
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI strengethText;
    [SerializeField] TextMeshProUGUI perceptionText;
    [SerializeField] TextMeshProUGUI agilityText;
    [SerializeField] Image hpGage;
    [SerializeField] Image staminaGage;
    [SerializeField] Image survivorImage;

    Button _button;

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(isSelectedButton ? DeselectThisSurvivor : SelectThisSurvivor);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        UIManager.OnHideInfoWindow?.Invoke();

        _button.onClick.RemoveAllListeners();
    }
    public void SetupInfo(ActiveSurvivor info)
    {
        this._info = info;
        survivorImage.sprite = info.Sprite;
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
    IEnumerator ShowInfoWindow()
    {
        string info = $"<color=green>{_info.Name}</color>\n" +
                        $"Description: {_info.Description}\n" +
                        $"<color=green>----------------------------------------------</color>\n" +
                        $"Health: {_info.CurrentHealth}/{_info.Stats.MaxHealth}\n" +
                        $"Fatigue: {_info.Fatigue}%\n" +
                        $"Strength: {_info.Stats.Strength}\n" +
                        $"Perception: {_info.Stats.Perception}\n" +
                        $"Agility: {_info.Stats.Agility}";
        while (true)
        {

            Vector2 mousePos = Mouse.current.position.ReadValue();
            UIManager.OnShowInfoWindow?.Invoke(info, mousePos);
            yield return null;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ShowInfoWindow());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        UIManager.OnHideInfoWindow?.Invoke();
    }
}
