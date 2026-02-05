using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public WeaponData[] weapons;
    public InputActionReference fireAction;
    public InputActionReference switchWeaponAction;

    private int currentWeaponIndex;
    private float nextFireTime;

    private void OnEnable()
    {
        fireAction.action.performed += OnFire;
        switchWeaponAction.action.performed += OnSwitchWeapon;

        fireAction.action.Enable();
        switchWeaponAction.action.Enable();
    }

    private void OnDisable()
    {
        fireAction.action.performed -= OnFire;
        switchWeaponAction.action.performed -= OnSwitchWeapon;

        fireAction.action.Disable();
        switchWeaponAction.action.Disable();
    }

    void OnFire(InputAction.CallbackContext context)
    {
        WeaponData weapon = weapons[currentWeaponIndex];

        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + weapon.fireRate;

        GameObject projectileObj = Instantiate(
            weapon.projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.Initialize(weapon.damage, weapon.projectileSpeed);
    }

    void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Length)
            currentWeaponIndex = 0;

        Debug.Log("Switched to: " + weapons[currentWeaponIndex].weaponName);
    }
}
