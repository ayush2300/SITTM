using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Header("Starting Items")]
    public List<ItemSO> startingItems; // Optional starting items to spawn at game start

    private readonly List<ActiveItem> activeItems = new List<ActiveItem>();

    private class ActiveItem
    {
        public ItemSO itemSO;
        public int currentLevel;
        public GameObject instance;
    }

    void Start()
    {
        // ? Spawn starting items if any are set
        foreach (var itemSO in startingItems)
        {
            AddItemFromSO(itemSO, 0);
        }
    }

    /// <summary>
    /// ? Adds a new item (spawns prefab at level 0 or specified level)
    /// </summary>
    public void AddItemFromSO(ItemSO itemSO, int levelIndex)
    {
        if (itemSO.levels == null || itemSO.levels.Count == 0)
        {
            Debug.LogWarning($"[ItemManager] ItemSO '{itemSO.itemName}' has no levels defined!");
            return;
        }

        levelIndex = Mathf.Clamp(levelIndex, 0, itemSO.levels.Count - 1);
        var levelData = itemSO.levels[levelIndex];

        // Spawn prefab for this item
        GameObject itemInstance = Instantiate(levelData.itemPrefab, transform.position, Quaternion.identity, transform);

        var newItem = new ActiveItem
        {
            itemSO = itemSO,
            currentLevel = levelIndex,
            instance = itemInstance
        };

        activeItems.Add(newItem);
    }

    /// <summary>
    /// ? Upgrades an existing item to its next level, if available.
    /// </summary>
    public void UpgradeItem(ItemSO itemSO)
    {
        ActiveItem activeItem = activeItems.Find(i => i.itemSO == itemSO);
        if (activeItem == null)
        {
            Debug.LogWarning($"[ItemManager] Tried to upgrade '{itemSO.itemName}' but player doesn't have it!");
            return;
        }

        int nextLevel = activeItem.currentLevel + 1;
        if (nextLevel >= itemSO.levels.Count)
        {
            Debug.Log($"[ItemManager] '{itemSO.itemName}' is already at max level!");
            return;
        }

        // Destroy current prefab
        if (activeItem.instance != null)
            Destroy(activeItem.instance);

        // Spawn upgraded version
        var newLevelData = itemSO.levels[nextLevel];
        GameObject newInstance = Instantiate(newLevelData.itemPrefab, transform.position, Quaternion.identity, transform);

        // Update active data
        activeItem.currentLevel = nextLevel;
        activeItem.instance = newInstance;

        Debug.Log($"[ItemManager] Upgraded '{itemSO.itemName}' to level {nextLevel + 1}");
    }

    /// <summary>
    /// ? Checks if the player already has this item.
    /// </summary>
    public bool HasItem(ItemSO itemSO)
    {
        return activeItems.Exists(i => i.itemSO == itemSO);
    }

    /// <summary>
    /// ? Returns the current level index of this item (-1 if not owned).
    /// </summary>
    public int GetItemLevel(ItemSO itemSO)
    {
        ActiveItem activeItem = activeItems.Find(i => i.itemSO == itemSO);
        return activeItem != null ? activeItem.currentLevel : -1;
    }

    /// <summary>
    /// ? Called by CardUI when a new item card is selected.
    /// Adds new or upgrades existing.
    /// </summary>
    public void AddItem(ItemSO itemSO)
    {
        if (HasItem(itemSO))
        {
            UpgradeItem(itemSO);
        }
        else
        {
            AddItemFromSO(itemSO, 0);
        }
    }
}
