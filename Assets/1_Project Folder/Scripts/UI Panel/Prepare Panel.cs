using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreparePanel : UIPanel
{
    [SerializeField] TextMeshProUGUI areaNameText;
    [SerializeField] TextMeshProUGUI areaDetailText;

    [SerializeField] List<Button> notSelectedSurvivorButtonList;
    [SerializeField] List<Button> selectedSurvivorButtonList;
    public void UpdateAreaInfo(AreaTemplate area)
    {
        areaNameText.text = area.AreaName;
        areaDetailText.text = area.Description;
    }
    public void SetupSurvivorList()
    {
        var allSurvivors = GameManager.Instance.activeSurvivors;
        var selectedList = GameManager.Instance.selectedSurvivor;

        // 1. กรองคนที่ "ไม่ได้ถูกเลือก" และ "ไม่พักอยู่"
        var availableSurvivors = allSurvivors
            .Where(s => !selectedList.Contains(s) && !s.IsResting)
            .ToList();

        // 2. กรองคนที่ "ถูกเลือกอยู่" ในขณะนี้
        // (ใช้ Contains เพื่อให้แน่ใจว่าคนใน selectedList ยังมีตัวตนอยู่ใน activeSurvivors จริง)
        var currentSelection = allSurvivors
            .Where(s => selectedList.Contains(s))
            .ToList();

        // 3. กรองคนที่ "กำลังพักผ่อน" (ถ้าต้องการแยกไปโชว์อีกที่ หรือทำให้กดไม่ได้)
        var restingSurvivors = allSurvivors
            .Where(s => s.IsResting && !selectedList.Contains(s))
            .ToList();

        // เรียกฟังก์ชันเพื่อวาดปุ่มบน UI (ตัวอย่าง Logic การแสดงผล)
        RenderButtons(notSelectedSurvivorButtonList, availableSurvivors);
        RenderButtons(selectedSurvivorButtonList, currentSelection);
    }
    private void RenderButtons(List<Button> buttonList, List<ActiveSurvivor> dataList)
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            if (i < dataList.Count)
            {
                buttonList[i].gameObject.SetActive(true);
                buttonList[i].GetComponentInChildren<SurvivorButton>().SetupInfo(dataList[i]);
            }
            else
            {
                buttonList[i].gameObject.SetActive(false);
            }
        }
    }
}