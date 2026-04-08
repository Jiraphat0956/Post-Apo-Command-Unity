using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel: UIPanel
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button nextAreaButton;

    [Header ("Recruit Survivor")]
    [SerializeField] GameObject recruitPanel;
    [SerializeField] Button recruitButton;
    [SerializeField] Button ignoreButton;

    private void OnEnable()
    {
        nextAreaButton.onClick.AddListener(GoNextArea);
    }
    private void OnDisable()
    {
        nextAreaButton.onClick.RemoveListener(GoNextArea);
        recruitButton.onClick.RemoveAllListeners();
        ignoreButton.onClick.RemoveAllListeners();
    }
    public void DisplayResult(ExpeditionResult result)
    {
        string displayMessage = string.Empty;
        if (result.IsSuccess)
        {
            displayMessage += "Expedition Successful!\n";
            displayMessage += $"Supply Gained: {result.SupplyGained}\n";
        }
        else
        {
            displayMessage += "Expedition Failed.\n";
        }
        displayMessage += "Log:\n" + result.Log + "\n";
        foreach (string status in result.StatusUpdates)
        {
            displayMessage += "- " + status + "\n";
        }
        resultText.text = displayMessage;

        if(result.FoundSurvivor != null)
        {
            recruitPanel.SetActive(true);
            nextAreaButton.interactable = false;
            recruitButton.onClick.AddListener(RecruitSurvivor);
            ignoreButton.onClick.AddListener(IgnoreRecruitment);
        }
        else
        {
            recruitPanel.SetActive(false);
        }
    }
    void GoNextArea()
    {
        GameManager.Instance.ChangeGameState(GameState.Prepare);
    }
    void RecruitSurvivor()
    {
        var survivor = ExpeditionManager.Instance.CurrentResult.FoundSurvivor;
        if(survivor != null)
        {
            GameManager.Instance.AddActiveSurvivor(survivor);
            recruitPanel.SetActive(false);
        }
        nextAreaButton.interactable = true;
    }
    void IgnoreRecruitment()
    {
        recruitPanel.SetActive(false);
        nextAreaButton.interactable = true;
    }
}