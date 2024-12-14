using UnityEngine;

// This class will be responsible from wearing and unwearing the weapon.
public class WeaponManager : MonoBehaviour
{
    private IWeapon weapon;

    private void Awake()
    {
        weapon = GetComponentInChildren<IWeapon>();
    }

    public void SetWeapon()
    {

    }
}
