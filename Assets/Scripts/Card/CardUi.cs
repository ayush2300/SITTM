using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Button selectButton;

    private WeaponSO weaponData;
    private WeaponManager weaponManager;
    private CardSpawner cardSpawner;

    public void Initialize(WeaponSO weapon, WeaponManager manager, CardSpawner spawner)
    {
        weaponData = weapon;
        weaponManager = manager;
        cardSpawner = spawner;

        titleText.text = weapon.weaponName;
        selectButton.onClick.AddListener(OnCardClicked);
    }

    private void OnCardClicked()
    {
        weaponManager.AddWeaponFromSO(weaponData);
        cardSpawner.OnCardSelected();
    }
}
