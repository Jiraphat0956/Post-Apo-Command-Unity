using UnityEngine;

[CreateAssetMenu(fileName = "NewSurvivorTemplate", menuName = "Game/Survivor Template")]
public class SurvivorTemplate : ScriptableObject
{
    public Sprite Sprite;
    public string DefaultName;
    [TextArea] public string Description;
    public SurvivorStats BaseStats;
}