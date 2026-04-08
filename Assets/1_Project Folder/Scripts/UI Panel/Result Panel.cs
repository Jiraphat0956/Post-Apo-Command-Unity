using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel: UIPanel
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button nextAreaButton;

    private void OnEnable()
    {
        nextAreaButton.onClick.AddListener(GoNextArea);
    }
    private void OnDisable()
    {
        nextAreaButton.onClick.RemoveListener(GoNextArea);
    }
    public void DisplayResult()
    {
        ExpeditionResult result = ExpeditionManager.Instance.CurrentResult;
        string displayMessage = string.Empty;
        if (result.IsSuccess)
        {
            displayMessage += "Expedition Successful!\n";
            displayMessage += $"Food Gained: {result.FoodGained}\n";
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
    }
    void GoNextArea()
    {
        GameManager.Instance.ChangeGameState(GameState.Prepare);
    }
}