using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Transform cardParent;
    public List<WeaponSO> allWeapons;
    public int cardsToSpawn = 3;

    private WeaponManager weaponManager;
    private List<GameObject> spawnedCards = new List<GameObject>();

    void OnEnable()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
        ClearOldCards();
        SpawnRandomCards();

        // ✅ Freeze game time when panel is active
        Time.timeScale = 0f;
    }

    void SpawnRandomCards()
    {
        if (allWeapons.Count == 0 || cardsToSpawn <= 0) return;

        // Filter weapons: only spawn if player doesn't have it OR it has levels left
        List<WeaponSO> tempList = new List<WeaponSO>();
        foreach (var weapon in allWeapons)
        {
            if (weaponManager.HasWeapon(weapon))
            {
                int currentLevel = weaponManager.GetWeaponLevel(weapon);
                if (currentLevel < weapon.levels.Count - 1) // still has levels left
                    tempList.Add(weapon);
            }
            else
            {
                tempList.Add(weapon); // player doesn't have it
            }
        }

        if (tempList.Count == 0) return; // nothing to spawn

        for (int i = 0; i < cardsToSpawn; i++)
        {
            if (tempList.Count == 0) break;

            int randomIndex = Random.Range(0, tempList.Count);
            WeaponSO selectedWeapon = tempList[randomIndex];
            tempList.RemoveAt(randomIndex);

            GameObject newCard = Instantiate(cardPrefab, cardParent);
            spawnedCards.Add(newCard);

            CardUI cardUI = newCard.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.Initialize(selectedWeapon, weaponManager, this);
            }
        }
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
