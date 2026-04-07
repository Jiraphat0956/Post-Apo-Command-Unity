using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : UIPanel
{
    [SerializeField] Button playButton;

    private void OnEnable()
    {
        playButton.onClick.AddListener(PlayGame);
    }
    private void OnDisable()
    {
        playButton.onClick.RemoveListener(PlayGame);
    }
    void PlayGame()
    {
        GameManager.Instance.ChangeGameState(GameState.Prepare);
    }
}