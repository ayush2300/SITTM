using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    [Header("Card Settings")]
    public Transform cardParent;
    public int cardsToSpawn = 3;

    [Header("Weapon & Item Data")]
    public List<WeaponSO> allWeapons;
    public List<ItemSO> allItems;

    private WeaponManager weaponManager;
    private ItemManager itemManager;
    private CardPooler cardPooler;

    private List<GameObject> spawnedCards = new List<GameObject>();

    void OnEnable()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
        itemManager = FindObjectOfType<ItemManager>();
        cardPooler = FindObjectOfType<CardPooler>();

        ClearOldCards();
        StartCoroutine(SpawnNextFrame());
    }

    private System.Collections.IEnumerator SpawnNextFrame()
    {
        yield return null; // Wait one frame to avoid layout spikes
        SpawnRandomCards();
        Time.timeScale = 0f;
    }

    void SpawnRandomCards()
    {
        if ((allWeapons.Count == 0 && allItems.Count == 0) || cardsToSpawn <= 0) return;

        // Filter valid weapons
        List<WeaponSO> tempWeapons = new List<WeaponSO>();
        foreach (var weapon in allWeapons)
        {
            if (weaponManager.HasWeapon(weapon))
            {
                int currentLevel = weaponManager.GetWeaponLevel(weapon);
                if (currentLevel < weapon.levels.Count - 1)
                    tempWeapons.Add(weapon);
            }
            else tempWeapons.Add(weapon);
        }

        int weaponsToSpawn = Mathf.Min(2, tempWeapons.Count);
        int itemsToSpawn = Mathf.Min(1, allItems.Count);

        HashSet<WeaponSO> usedWeapons = new HashSet<WeaponSO>();
        HashSet<ItemSO> usedItems = new HashSet<ItemSO>();

        for (int i = 0; i < weaponsToSpawn; i++)
        {
            WeaponSO selected = GetUniqueRandom(tempWeapons, usedWeapons);
            if (selected == null) continue;

            GameObject card = cardPooler.GetCard(cardParent);
            CardUI ui = card.GetComponent<CardUI>();
            ui.InitializeWeapon(selected, weaponManager, this);
            spawnedCards.Add(card);
        }

        for (int i = 0; i < itemsToSpawn; i++)
        {
            ItemSO selected = GetUniqueRandom(allItems, usedItems);
            if (selected == null) continue;

            GameObject card = cardPooler.GetCard(cardParent);
            CardUI ui = card.GetComponent<CardUI>();
            ui.InitializeItem(selected, itemManager, this);
            spawnedCards.Add(card);
        }
    }

    private T GetUniqueRandom<T>(List<T> list, HashSet<T> used)
    {
        if (list.Count == 0) return default;

        int attempts = 0;
        T selected;
        do
        {
            selected = list[Random.Range(0, list.Count)];
            attempts++;
        }
        while (used.Contains(selected) && attempts < 10);

        used.Add(selected);
        return selected;
    }

    public void OnCardSelected()
    {
        ClearOldCards();
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    void ClearOldCards()
    {
        foreach (var card in spawnedCards)
        {
            cardPooler.ReturnCard(card);
        }
        spawnedCards.Clear();
    }
}
