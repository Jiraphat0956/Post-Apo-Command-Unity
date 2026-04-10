using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreparePanel : UIPanel
{
    [SerializeField] TextMeshProUGUI areaNameText;
    [SerializeField] TextMeshProUGUI areaDetailText;
    [SerializeField] TextMeshProUGUI totalSupplyText;
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI successChanceText;

    [SerializeField] Button startExpeditionButton;
    [SerializeField] Button skipExpeditionButton;

    [SerializeField] List<Button> notSelectedSurvivorButtonList;
    [SerializeField] List<Button> selectedSurvivorButtonList;

    [Header("Requirement Stats")]
    [SerializeField] TextMeshProUGUI strengethText;
    [SerializeField] TextMeshProUGUI perceptionText;
    [SerializeField] TextMeshProUGUI agilityText;

    private void OnEnable()
    {
        startExpeditionButton.onClick.AddListener(StartExpedition);
        skipExpeditionButton.onClick.AddListener(SkipExpedition);
    }
    private void OnDisable()
    {
        startExpeditionButton.onClick.RemoveListener(StartExpedition);
        skipExpeditionButton.onClick.RemoveListener(SkipExpedition);
    }
    public void UpdateTotalSupply()
    {
        var totalSupply = GameManager.Instance.TotalSupply;
        totalSupplyText.text = $"Supply: {totalSupply}";
    }
    public void UpdateDay()
    {
        var currentDay = GameManager.Instance.CurrentDay;
        var targetDay = GameManager.Instance.TargetDay;
        dayText.text = $"Day {currentDay}/{targetDay}";
    }

    public void UpdateAreaInfo(AreaTemplate area)
    {
        areaNameText.text = area.AreaName;
        areaDetailText.text = area.Description;
        strengethText.text = area.Stats.RequiredStrength.ToString();
        perceptionText.text = area.Stats.RequiredPerception.ToString();
        agilityText.text = area.Stats.RequiredAgility.ToString();
    }
    public void UpdateSurvivorList()
    {
        var allSurvivors = GameManager.Instance.activeSurvivors;
        var selectedList = GameManager.Instance.selectedSurvivor;

        // 1. กรองคนที่ "ไม่ได้ถูกเลือก"
        var availableSurvivors = allSurvivors
            .Where(s => !selectedList.Contains(s))
            .ToList();

        // 2. กรองคนที่ "ถูกเลือกอยู่" ในขณะนี้
        // (ใช้ Contains เพื่อให้แน่ใจว่าคนใน selectedList ยังมีตัวตนอยู่ใน activeSurvivors จริง)
        var currentSelection = allSurvivors
            .Where(s => selectedList.Contains(s))
            .ToList();

        // เรียกฟังก์ชันเพื่อวาดปุ่มบน UI (ตัวอย่าง Logic การแสดงผล)
        RenderButtons(notSelectedSurvivorButtonList, availableSurvivors);
        RenderButtons(selectedSurvivorButtonList, currentSelection);

        UpdateStatRequrirementColor();
        UpdateExpeditionButton(selectedList);

        successChanceText.text = $"Success Chance: {ExpeditionManager.Instance.GetSuccessChance(selectedList) * 100f:0}%";
    }
    public void UpdateStatRequrirementColor()
    {
        var selectedList = GameManager.Instance.selectedSurvivor;
        int totalStr = selectedList.Sum(s => s.Stats.Strength);
        int totalPer = selectedList.Sum(s => s.Stats.Perception);
        int totalAgi = selectedList.Sum(s => s.Stats.Agility);
        var area = ExpeditionManager.Instance.CurrentArea;
        strengethText.color = totalStr >= area.Stats.RequiredStrength ? Color.green : Color.red;
        perceptionText.color = totalPer >= area.Stats.RequiredPerception ? Color.green : Color.red;
        agilityText.color = totalAgi >= area.Stats.RequiredAgility ? Color.green : Color.red;
    }
    private void RenderButtons(List<Button> buttonList, List<ActiveSurvivor> dataList)
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            if (i < dataList.Count)
            {
                buttonList[i].gameObject.SetActive(true);
                buttonList[i].GetComponent<SurvivorButton>().SetupInfo(dataList[i]);
            }
            else
            {
                buttonList[i].gameObject.SetActive(false);
            }
        }
    }
    void UpdateExpeditionButton(List<ActiveSurvivor> dataList)
    {
        startExpeditionButton.interactable = dataList.Count > 0;
    }
    void StartExpedition()
    {
        GameManager.Instance.ChangeGameState(GameState.Expedition);
    }
    void SkipExpedition()
    {
        GameManager.Instance.IsSkiped = true;
        StartExpedition();
    }
}