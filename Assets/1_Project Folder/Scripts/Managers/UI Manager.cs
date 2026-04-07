using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] List<UIPanel> panels;

    MainMenuPanel _mainMenuPanel;
    PreparePanel _preparePanel;
    ResultPanel _resultPanel;

    private void Awake()
    {
        _mainMenuPanel = panels.Find(panel => panel is MainMenuPanel) as MainMenuPanel;
        _preparePanel = panels.Find(panel => panel is PreparePanel) as PreparePanel;
        _resultPanel = panels.Find(panel => panel is ResultPanel) as ResultPanel;
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChange += ChangeUIPanel;
        ExpeditionManager.Instance.OnAreaChange += _preparePanel.UpdateAreaInfo;
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
                _preparePanel.SetupSurvivorList();
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
