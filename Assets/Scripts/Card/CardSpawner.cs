using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Transform cardParent;
    public int cardsToSpawn = 3;

    [Header("Weapon & Item Data")]
    public List<WeaponSO> allWeapons;
    public List<ItemSO> allItems; // ✅ New list for items

    private WeaponManager weaponManager;
    private ItemManager itemManager; // ✅ If you have a manager for items
    private List<GameObject> spawnedCards = new List<GameObject>();

    void OnEnable()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
        itemManager = FindObjectOfType<ItemManager>(); // Optional, used if items need management
        ClearOldCards();
        SpawnRandomCards();

        // ✅ Freeze game time when panel is active
        Time.timeScale = 0f;
    }

    void SpawnRandomCards()
    {
        if ((allWeapons.Count == 0 && allItems.Count == 0) || cardsToSpawn <= 0) return;

        // Create a temporary list for valid weapons
        List<WeaponSO> tempWeapons = new List<WeaponSO>();
        foreach (var weapon in allWeapons)
        {
            if (weaponManager.HasWeapon(weapon))
            {
                int currentLevel = weaponManager.GetWeaponLevel(weapon);
                if (currentLevel < weapon.levels.Count - 1)
                    tempWeapons.Add(weapon);
            }
            else
            {
                tempWeapons.Add(weapon);
            }
        }

        // ✅ Pick 2 random weapons and 1 random item each time
        int weaponsToSpawn = Mathf.Min(2, tempWeapons.Count);
        int itemsToSpawn = Mathf.Min(1, allItems.Count);

        List<GameObject> cards = new List<GameObject>();

        // Spawn weapon cards
        for (int i = 0; i < weaponsToSpawn; i++)
        {
            int randomIndex = Random.Range(0, tempWeapons.Count);
            WeaponSO selectedWeapon = tempWeapons[randomIndex];
            tempWeapons.RemoveAt(randomIndex);

            GameObject newCard = Instantiate(cardPrefab, cardParent);
            cards.Add(newCard);

            CardUI cardUI = newCard.GetComponent<CardUI>();
            if (cardUI != null)
                cardUI.InitializeWeapon(selectedWeapon, weaponManager, this);
        }

        // Spawn item card
        for (int i = 0; i < itemsToSpawn; i++)
        {
            int randomIndex = Random.Range(0, allItems.Count);
            ItemSO selectedItem = allItems[randomIndex];

            GameObject newCard = Instantiate(cardPrefab, cardParent);
            cards.Add(newCard);

            CardUI cardUI = newCard.GetComponent<CardUI>();
            if (cardUI != null)
                cardUI.InitializeItem(selectedItem, itemManager, this);
        }

        spawnedCards = cards;
    }

    public void OnCardSelected()
    {
        ClearOldCards();
        gameObject.SetActive(false);

        // ✅ Resume game time when a card is selected
        Time.timeScale = 1f;
    }

    void ClearOldCards()
    {
        foreach (var card in spawnedCards)
        {
            Destroy(card);
        }
        spawnedCards.Clear();
    }
}
