using UnityEngine;

[CreateAssetMenu(fileName = "NewSurvivorTemplate", menuName = "Game/Survivor Template")]
public class SurvivorTemplate : ScriptableObject
{
    public string DefaultName;
    public SurvivorStats BaseStats;
}