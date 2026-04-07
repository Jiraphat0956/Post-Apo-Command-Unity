using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets; // ต้องเพิ่ม
using UnityEngine.ResourceManagement.AsyncOperations; // ต้องเพิ่ม

public class GameManager : MonoBehaviour
{
    public List<AreaTemplate> allAreas { get; private set; } = new List<AreaTemplate>();
    public List<SurvivorTemplate> allSurvivors { get; private set; } = new List<SurvivorTemplate>();

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    void Start()
    {
        // โหลดข้อมูลด้วย Label
        LoadAssetsByLabel<AreaTemplate>("AreaData", allAreas);
        LoadAssetsByLabel<SurvivorTemplate>("SurvivorData", allSurvivors);
    }

    private void LoadAssetsByLabel<T>(string label, List<T> targetList) where T : ScriptableObject
    {
        // โหลด Asset ทั้งหมดที่มี Label ตรงกับที่ระบุ
        Addressables.LoadAssetsAsync<T>(label, null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                targetList.AddRange(handle.Result);
                Debug.Log($"Successfully loaded {handle.Result.Count} {typeof(T).Name} assets via Addressables.");
            }
            else
            {
                Debug.LogError($"Failed to load Addressables for label: {label}");
            }
        };
    }
    void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        switch (CurrentState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Prepare:
                break;
            case GameState.Result:
                break;
        }
    }
}
