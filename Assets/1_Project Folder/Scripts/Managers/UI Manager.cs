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
}
