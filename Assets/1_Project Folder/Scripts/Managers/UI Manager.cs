using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] RectTransform infoWindow;
    [SerializeField] RectTransform infoWindowAnchor;

    [SerializeField] List<UIPanel> panels;

    MainMenuPanel _mainMenuPanel;
    PreparePanel _preparePanel;
    ExpeditionPanel _expeditionPanel;
    ResultPanel _resultPanel;

    public static Action<string, Vector2> OnShowInfoWindow;
    public static Action OnHideInfoWindow;

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
        GameManager.Instance.OnGameStateChange += (x) => { if (x == GameState.Prepare) { _preparePanel.UpdateTotalSupply(); _preparePanel.UpdateDay(); } };
        GameManager.Instance.OnGameStateChange += (x) => { if (x == GameState.Result) _resultPanel.HandleButtons(); };
        GameManager.Instance.OnSurvivorListChange += _preparePanel.UpdateSurvivorList;
        GameManager.Instance.OnGameOver += _resultPanel.DisplayGameOver;
        ExpeditionManager.Instance.OnAreaChange += _preparePanel.UpdateAreaInfo;
        ExpeditionManager.Instance.OnExpeditionComplete += _resultPanel.DisplayResult;

        OnShowInfoWindow += ShowInfoWindow;
        OnHideInfoWindow += HideInfoWindow;
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

    public void ShowInfoWindow(string info, Vector2 mousePosition)
    {
        infoText.text = info;
        infoWindow.gameObject.SetActive(true);
        infoWindow.sizeDelta = new Vector2(infoText.preferredWidth > 450 ? 450 : infoText.preferredWidth, infoText.preferredHeight);

        infoWindowAnchor.transform.position = new Vector2(mousePosition.x, mousePosition.y);

    }
    public void HideInfoWindow()
    {
        infoWindow.gameObject.SetActive(false);
    }
}
