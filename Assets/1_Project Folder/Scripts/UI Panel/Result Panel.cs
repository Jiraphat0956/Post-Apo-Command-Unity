using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel: UIPanel
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button nextAreaButton;
    [SerializeField] Button newExpeditionButton;

    [Header ("Recruit Survivor")]
    [SerializeField] GameObject recruitPanel;
    [SerializeField] Button recruitButton;
    [SerializeField] Button ignoreButton;

    private void OnEnable()
    {
        nextAreaButton.onClick.AddListener(GoNextArea);
        recruitButton.onClick.AddListener(RecruitSurvivor);
        ignoreButton.onClick.AddListener(IgnoreRecruitment);
        newExpeditionButton.onClick.AddListener(GoToMainMenu);
    }
    private void OnDisable()
    {
        nextAreaButton.onClick.RemoveListener(GoNextArea);
        recruitButton.onClick.RemoveAllListeners();
        ignoreButton.onClick.RemoveAllListeners();
        newExpeditionButton.onClick.RemoveListener(GoToMainMenu);
    }
    public void DisplayResult(ExpeditionResult result)
    {
        string displayMessage = string.Empty;
        if (result.IsSuccess)
        {
            displayMessage += "<color=green>Expedition Successful!</color>\n";
            displayMessage += $"Supply Gained: {result.SupplyGained}\n";
        }
        else
        {
            displayMessage += "<color=red>Expedition Failed.</color>\n";
        }
        displayMessage += "Supply Used: " + result.SupplyUsed + "\n";
        displayMessage += "Log:\n" + result.Log + "\n";
        foreach (string status in result.StatusUpdates)
        {
            displayMessage += "- " + status + "\n";
        }

        resultText.text = displayMessage;
    }
    public void DisplayGameOver()
    {
        GameManager gameManager = GameManager.Instance;
        string displayMessage = resultText.text;
        displayMessage += gameManager.IsGameOver ? $"Game Over! : {gameManager.GetGameOverReason()}" : "Prepare for the next expedition.";
        resultText.text = displayMessage;
    }
    public void HandleButtons()
    {
        GameManager gameManager = GameManager.Instance;

        ExpeditionResult result = ExpeditionManager.Instance.CurrentResult;
        if (result.FoundSurvivor != null && !gameManager.IsGameOver)
        {
            recruitPanel.SetActive(true);
            nextAreaButton.interactable = false;
            recruitButton.interactable = !gameManager.IsActiveSurvivorFull();
            newExpeditionButton.gameObject.SetActive(false);

        }
        else
        {
            if (gameManager.IsGameOver)
            {
                nextAreaButton.gameObject.SetActive(false);
                newExpeditionButton.gameObject.SetActive(true);
            }
            else
            {
                nextAreaButton.gameObject.SetActive(true);
                newExpeditionButton.gameObject.SetActive(false);
            }
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
    void GoToMainMenu()
    {
        GameManager.Instance.ChangeGameState(GameState.MainMenu);
    }
}