using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] List<UIPanel> panels;

    MainMenuPanel _mainMenuPanel;
    PreparePanel _preparePanel;
    ExpeditionPanel _expeditionPanel;
    ResultPanel _resultPanel;

    private void Awake()
    {
        _mainMenuPanel = panels.Find(panel => panel is MainMenuPanel) as MainMenuPanel;
        _preparePanel = panels.Find(panel => panel is PreparePanel) as PreparePanel;
        _expeditionPanel = panels.Find(panel => panel is ExpeditionPanel) as ExpeditionPanel;
        _resultPanel = panels.Find(panel => panel is ResultPanel) as ResultPanel;
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChange += ChangeUIPanel;
        GameManager.Instance.OnGameStateChange += (x) => { if (x == GameState.Prepare) _preparePanel.UpdateTotalSupply(); };
        GameManager.Instance.OnGameStateChange += (x) => { if (x == GameState.Result) _resultPanel.HandleButtons(); };
        GameManager.Instance.OnSurvivorListChange += _preparePanel.UpdateSurvivorList;
        ExpeditionManager.Instance.OnAreaChange += _preparePanel.UpdateAreaInfo;
        ExpeditionManager.Instance.OnExpeditionComplete += _resultPanel.DisplayResult;
    }
    void ChangeUIPanel(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.MainMenu:
                ShowPanel(_mainMenuPanel);
                break;
            case GameState.Prepare:
                ShowPanel(_preparePanel);
                _preparePanel.UpdateSurvivorList();
                break;
            case GameState.Expedition:
                ShowPanel(_expeditionPanel);
                break;
            case GameState.Result:
                ShowPanel(_resultPanel);
                break;
        }
    }
    void ShowPanel(UIPanel panelToShow)
    {
        foreach (var panel in panels)
        {
            panel.gameObject.SetActive(panel == panelToShow);
        }
    }
}
