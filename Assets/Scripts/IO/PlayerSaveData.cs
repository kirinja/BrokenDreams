using System.Collections.Generic;
using UnityEngine;

public struct PlayerSaveData
{
    [SerializeField]
    public List<Ability> Abilities;
    public int HP;
    [SerializeField]
    public SortedDictionary<int, bool> BeatenLevels;
}