using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : Singleton<GameManager>
{
    public List<AreaTemplate> allAreas { get; private set; } = new List<AreaTemplate>();
    public List<SurvivorTemplate> allSurvivors { get; private set; } = new List<SurvivorTemplate>();

    public List<ActiveSurvivor> activeSurvivors = new List<ActiveSurvivor>();
    public List<ActiveSurvivor> selectedSurvivor = new List<ActiveSurvivor>();

    public GameState CurrentState { get; private set; }
    public bool IsSkiped { get; set; }

    public delegate void GameGenericHandler<T>(T data);
    public delegate void GameHandler();

    public event GameGenericHandler<GameState> OnGameStateChange;
    public event GameHandler OnSurvivorListChange;

    async void Start()
    {
        // โหลดข้อมูลด้วย Label
        await LoadAssetsByLabel<AreaTemplate>("AreaData", allAreas);
        await LoadAssetsByLabel<SurvivorTemplate>("SurvivorData", allSurvivors);

        RandomSurvivor(3);
        ChangeGameState(GameState.MainMenu);

        ExpeditionManager.Instance.OnExpeditionComplete += () =>
        {
            ChangeGameState(GameState.Result);
        };
    }
    #region Loading Assets & Setup Data
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
    #endregion
    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        switch (CurrentState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Prepare:
                ExpeditionManager.Instance.SetArea();
                IsSkiped = false;
                break;
            case GameState.Expedition:
                if (IsSkiped)
                {
                    selectedSurvivor.Clear();
                    ExpeditionManager.Instance.SkipExpedition();
                }
                else
                {
                    ExpeditionManager.Instance.ExecutePartyExpedition(selectedSurvivor);
                }
                break;
            case GameState.Result:
                HandleSurvivorRestStatus();
                break;
        }
        OnGameStateChange?.Invoke(CurrentState);
    }

    public void AddActiveSurvivor(SurvivorTemplate survivorTemplate)
    {
        ActiveSurvivor newSurvivor = new ActiveSurvivor(survivorTemplate);
        activeSurvivors.Add(newSurvivor);
        Debug.Log($"Added new survivor: {newSurvivor.Name}");
    }
    public void RemoveActiveSurvivor(ActiveSurvivor survivor)
    {
        activeSurvivors.Remove(survivor);
        Debug.Log($"Removed survivor: {survivor.Name}");
    }
    public void AddSelectedSurvivor(ActiveSurvivor survivor)
    {
        selectedSurvivor.Add(survivor);
        Debug.Log($"Added selected survivor: {survivor.Name}");
        OnSurvivorListChange?.Invoke();
    }
    public void RemoveSelectedSurvivor(ActiveSurvivor survivor)
    {
        selectedSurvivor.Remove(survivor);
        Debug.Log($"Remove selected survivor: {survivor.Name}");
        OnSurvivorListChange?.Invoke();
    }
    void HandleSurvivorRestStatus()
    {
        ExpeditionResult result = ExpeditionManager.Instance.CurrentResult;

        // 1. กรองคนที่ "ไม่ได้ถูกเลือก"
        var notSelectedSurvivors = activeSurvivors
            .Where(s => !selectedSurvivor.Contains(s))
            .ToList();
        foreach (var survivor in notSelectedSurvivors)
        {
            survivor.IsResting = false; // คนที่ไม่ได้ไปจะไม่พัก (Mechanic: Active Recovery)
        }
        foreach (var member in selectedSurvivor)
        {
            member.IsResting = true; // ทุกคนที่ไปต้องพักในเทิร์นหน้า (Mechanic: Fatigue Management)
        }
        selectedSurvivor.Clear();
    }
}
