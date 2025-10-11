using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Button selectButton;

    private WeaponSO weaponData;
    private ItemSO itemData; // ✅ New for items
    private WeaponManager weaponManager;
    private ItemManager itemManager;
    private CardSpawner cardSpawner;

    private bool isWeaponCard;

    // ✅ Initialize for Weapon
    public void InitializeWeapon(WeaponSO weapon, WeaponManager manager, CardSpawner spawner)
    {
        weaponData = weapon;
        weaponManager = manager;
        cardSpawner = spawner;
        isWeaponCard = true;

        titleText.text = weapon.weaponName;
        selectButton.onClick.AddListener(OnCardClicked);
    }

    // ✅ Initialize for Item
    public void InitializeItem(ItemSO item, ItemManager manager, CardSpawner spawner)
    {
        itemData = item;
        itemManager = manager;
        cardSpawner = spawner;
        isWeaponCard = false;

        titleText.text = item.itemName;
        selectButton.onClick.AddListener(OnCardClicked);
    }

    private void OnCardClicked()
    {
        if (isWeaponCard)
        {
            if (weaponManager.HasWeapon(weaponData))
                weaponManager.UpgradeWeapon(weaponData);
            else
                weaponManager.AddWeaponFromSO(weaponData, 0);
        }
        else
        {
            // ✅ For items — if you have stacking or upgrades, handle it here
            itemManager?.AddItem(itemData);
        }

        cardSpawner.OnCardSelected();
    }
}
