using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject projectilePrefab;
    public float fireRate = 0.5f;
    public float projectileSpeed = 30f;
    public float damage = 25f;
}
