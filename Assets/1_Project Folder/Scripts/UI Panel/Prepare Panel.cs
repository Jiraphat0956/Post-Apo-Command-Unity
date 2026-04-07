using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PreparePanel : UIPanel
{
    [SerializeField] TextMeshProUGUI areaNameText;
    [SerializeField] TextMeshProUGUI areaDetailText;

    [SerializeField] List<Button> survivorList;
    public void SetupArea()
    {

    }
    public void SetupSurvivorList(List<SurvivorTemplate> survivors)
    {
        for (int i = 0; i < survivorList.Count; i++)
        {
            if (i < survivors.Count)
            {
                survivorList[i].gameObject.SetActive(true);
                survivorList[i].GetComponentInChildren<SurvivorButton>().SetupInfo(survivors[i]);
            }
            else
            {
                survivorList[i].gameObject.SetActive(false);
            }
        }
    }
}