using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : Singleton<GameManager>
{
    public List<GameBalanceConfig> allGameBalanceConfig { get; private set; } = new List<GameBalanceConfig>();
    public List<AreaTemplate> allAreas { get; private set; } = new List<AreaTemplate>();
    public List<SurvivorTemplate> allSurvivors { get; private set; } = new List<SurvivorTemplate>();

    public List<ActiveSurvivor> activeSurvivors = new List<ActiveSurvivor>();
    public List<ActiveSurvivor> selectedSurvivor = new List<ActiveSurvivor>();
    public List<ActiveSurvivor> notSelectedSurvivor { get { return activeSurvivors.Where(s => !selectedSurvivor.Contains(s)).ToList(); } }

    public GameBalanceConfig CurrentConfig { get; private set; }
    public GameState CurrentState { get; private set; }
    public int CurrentDay { get; private set; }
    public int TargetDay { get { return CurrentConfig != null ? CurrentConfig.TargetDayToWin : 0; } }
    public float TotalSupply { get; private set; }
    public bool IsSkiped { get; set; }
    public bool IsGameOver { get; set; }

    public delegate void GameGenericHandler<T>(T data);
    public delegate void GameHandler();

    public event GameGenericHandler<GameState> OnGameStateChange;
    public event GameHandler OnSurvivorListChange;
    public event GameHandler OnGameOver;

    async void Start()
    {
        // โหลดข้อมูลด้วย Label
        await LoadAssetsByLabel<GameBalanceConfig>("GameConfig", allGameBalanceConfig);
        await LoadAssetsByLabel<AreaTemplate>("AreaData", allAreas);
        await LoadAssetsByLabel<SurvivorTemplate>("SurvivorData", allSurvivors);

        SetGameBalanceConfig(0);

        ChangeGameState(GameState.MainMenu);

        ExpeditionManager.Instance.OnExpeditionComplete += (x) =>
        {
            CurrentDay++;
            AddSupply(x.SupplyGained);
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
    public void SetGameBalanceConfig(int index)
    {
        if (index >= 0 && index < allGameBalanceConfig.Count)
        {
            CurrentConfig = allGameBalanceConfig[index];
            Debug.Log($"Current Game Balance Config set to: {CurrentConfig.name}");
        }
        else
        {
            Debug.LogError($"Invalid Game Balance Config index: {index}");
        }
    }
    #endregion

    #region Game State Management
    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        switch (CurrentState)
        {
            case GameState.MainMenu:
                CurrentDay = 0;
                activeSurvivors.Clear();
                TotalSupply = CurrentConfig.StartingSupply;
                RandomSurvivor(CurrentConfig.StartingSurvivorCount);
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
                HandleSurvivorRecovery();
                CheckGameOver();

                break;
        }
        OnGameStateChange?.Invoke(CurrentState);
    }
    public void CheckGameOver()
    {
        IsGameOver = false;
        if (TotalSupply <= 0 || activeSurvivors.Count <= 0)
        {
            IsGameOver = true; // Game Over
            OnGameOver?.Invoke();
        }
        else if (CurrentDay >= CurrentConfig.TargetDayToWin)
        {
            IsGameOver = true; // Win Condition
            OnGameOver?.Invoke();
        }
    }
    public string GetGameOverReason()
    {
        if (TotalSupply <= 0)
            return "<color=red>You ran out of supplies!</color>";
        else if (activeSurvivors.Count <= 0)
            return "<color=red>All survivors have perished!</color>";
        else if (CurrentDay >= CurrentConfig.TargetDayToWin)
            return "<color=green>Congratulations! You've survived until the target area!</color>";
        return "Game is still ongoing.";
    }

    #endregion

    #region Survivor Management
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
    void HandleSurvivorRecovery()
    {
        ExpeditionResult result = ExpeditionManager.Instance.CurrentResult;

        // 1. กรองคนที่ "ไม่ได้ถูกเลือก"
        var notSelectedSurvivors = notSelectedSurvivor;
        float totalSupplyConsumption = notSelectedSurvivors.Count * CurrentConfig.SupplyConsumptionPerPerson;
        if (IsSkiped)//ถ้า SkipExpedition จะมีการลด Penalty ในการบริโภคอาหาร (Mechanic: Skip Expedition Penalty Reduction)
        {
            totalSupplyConsumption *= CurrentConfig.SkipExpeditionPenaltyMultiplier;
        }
        RemoveSupply(totalSupplyConsumption);

        foreach (var survivor in notSelectedSurvivors)
        {
            survivor.RestAndRecover();
            survivor.IsResting = false; // คนที่ไม่ได้ไปจะไม่พัก (Mechanic: Active Recovery)
        }
        foreach (var member in selectedSurvivor)
        {
            if(member.Fatigue >= 100) member.IsResting = true; // ทุกคนที่เหนื่อยมากเกินไปต้องพักในเทิร์นหน้า (Mechanic: Fatigue Management)
        }
        selectedSurvivor.Clear();
    }
    public bool IsPartyFull() => selectedSurvivor.Count >= CurrentConfig.MaxPartySize;
    public bool IsActiveSurvivorFull() => activeSurvivors.Count >= CurrentConfig.MaxActiveSurvivor;
    #endregion

    #region Supply Management
    public void AddSupply(float amount)
    {
        TotalSupply += amount;
        Debug.Log($"Added {amount} supply. Total supply: {TotalSupply}");
    }
    public void RemoveSupply(float amount)
    {
        TotalSupply -= amount;
    }
    #endregion
}
