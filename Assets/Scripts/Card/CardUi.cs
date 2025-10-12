using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public Button selectButton;

    private WeaponSO weaponData;
    private ItemSO itemData;
    private WeaponManager weaponManager;
    private ItemManager itemManager;
    private CardSpawner cardSpawner;
    private bool isWeaponCard;
    private bool isInitialized;

    private void Awake()
    {
        if (selectButton == null)
            selectButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        // Rebind listeners if this card was reused from the pool
        if (isInitialized)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnCardClicked);
        }

        // ✅ Force EventSystem refresh to catch pooled UI
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnDisable()
    {
        // Avoid memory leaks when pooling
        selectButton.onClick.RemoveAllListeners();
    }

    // ✅ Initialize for Weapon
    public void InitializeWeapon(WeaponSO weapon, WeaponManager manager, CardSpawner spawner)
    {
        weaponData = weapon;
        weaponManager = manager;
        cardSpawner = spawner;
        isWeaponCard = true;
        isInitialized = true;

        titleText.text = weapon.weaponName;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnCardClicked);
    }

    // ✅ Initialize for Item
    public void InitializeItem(ItemSO item, ItemManager manager, CardSpawner spawner)
    {
        itemData = item;
        itemManager = manager;
        cardSpawner = spawner;
        isWeaponCard = false;
        isInitialized = true;

        titleText.text = item.itemName;

        selectButton.onClick.RemoveAllListeners();
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
            itemManager?.AddItem(itemData);
        }

        cardSpawner.OnCardSelected();
    }
}
