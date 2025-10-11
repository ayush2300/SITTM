using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemLevel
{
    public GameObject itemPrefab; // prefab for this item level
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [Header("Per Level Settings")]
    public List<ItemLevel> levels;
}
