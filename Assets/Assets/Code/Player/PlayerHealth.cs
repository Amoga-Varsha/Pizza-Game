using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damagePerHit = 10;

    [Header("Damage Input")]
    [SerializeField] private Key[] damageKeys =
    {
        Key.A, Key.S, Key.D, Key.F,
        Key.J, Key.K, Key.L, Key.Space
    };

    private int _currentHealth;
    private Key _currentDamageKey;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
    public float Health01 => maxHealth <= 0 ? 1f : Mathf.Clamp01((float)_currentHealth / maxHealth);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentHealth = maxHealth;
        ChooseRandomDamageKey();
    }

    // Update is called once per frame
    void Update()
    {
        if (damageKeys == null || damageKeys.Length == 0)
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current[_currentDamageKey].wasPressedThisFrame)
        {
            TakeDamage(damagePerHit);
            ChooseRandomDamageKey();
        }
    }

    public void ApplyDamage(int amount)
    {
        TakeDamage(amount);
    }

    private void TakeDamage(int amount)
    {
        _currentHealth = Mathf.Max(_currentHealth - amount, 0);
        Debug.Log($"Player took {amount} damage. Health: {_currentHealth}/{maxHealth}");
    }

    private void ChooseRandomDamageKey()
    {
        int index = Random.Range(0, damageKeys.Length);
        _currentDamageKey = damageKeys[index];
        Debug.Log($"Press {_currentDamageKey} to take damage.");
    }
}
