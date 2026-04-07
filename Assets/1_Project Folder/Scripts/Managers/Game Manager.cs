using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets; // ต้องเพิ่ม
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks; // ต้องเพิ่ม

public class GameManager : Singleton<GameManager>
{
    public List<AreaTemplate> allAreas { get; private set; } = new List<AreaTemplate>();
    public List<SurvivorTemplate> allSurvivors { get; private set; } = new List<SurvivorTemplate>();

    public List<ActiveSurvivor> activeSurvivors = new List<ActiveSurvivor>();
    public List<ActiveSurvivor> selectedSurvivor = new List<ActiveSurvivor>();

    public GameState CurrentState { get; private set; }

    public delegate void GameGenericHandler<T>(T data);

    public event GameGenericHandler<GameState> OnGameStateChange;

    async void Start()
    {
        // โหลดข้อมูลด้วย Label
        await LoadAssetsByLabel<AreaTemplate>("AreaData", allAreas);
        await LoadAssetsByLabel<SurvivorTemplate>("SurvivorData", allSurvivors);

        RandomSurvivor(3);
    }

    private async Task LoadAssetsByLabel<T>(string label, List<T> targetList) where T : ScriptableObject
    {
        // สร้าง AsyncOperationHandle
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);

        // ใช้ await แทนการใช้ Callback (.Completed)
        IList<T> result = await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            targetList.AddRange(result);
            Debug.Log($"Successfully loaded {result.Count} {typeof(T).Name} assets.");
        }
        else
        {
            Debug.LogError($"Failed to load Addressables for label: {label}");
        }
    }
    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        switch (CurrentState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Prepare:
                ExpeditionManager.Instance.SetArea();
                break;
            case GameState.Result:
                break;
        }
        OnGameStateChange?.Invoke(CurrentState);
    }
    public void RandomSurvivor(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (allSurvivors.Count > 0)
            {
                var randomTemplate = allSurvivors[Random.Range(0, allSurvivors.Count)];
                AddActiveSurvivor(randomTemplate);
            }
            else
            {
                Debug.LogError("No survivor templates available to create active survivors!");
            }
        }
    }
    public void AddActiveSurvivor(SurvivorTemplate survivorTemplate)
    {
        ActiveSurvivor newSurvivor = new ActiveSurvivor(survivorTemplate);
        activeSurvivors.Add(newSurvivor);
        Debug.Log($"Added new survivor: {newSurvivor.Name}");
    }
}
