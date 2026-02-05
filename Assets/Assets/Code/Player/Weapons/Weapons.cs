using UnityEngine;
using UnityEngine.InputSystem;

public class Weapons : MonoBehaviour
{
    [SerializeField] private GameObject[] weaponSlots = new GameObject[9];
    [SerializeField] private int startSlot = 0;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference[] weaponActions;

    private int currentSlot = -1;

    private void OnEnable()
    {
        for (int i = 0; i < weaponActions.Length; i++)
        {
            int slotIndex = i; // capture for closure
            weaponActions[i].action.performed += _ => SelectSlot(slotIndex);
            weaponActions[i].action.Enable();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < weaponActions.Length; i++)
        {
            weaponActions[i].action.Disable();
        }
    }

    private void Start()
    {
        SelectSlot(Mathf.Clamp(startSlot, 0, weaponSlots.Length - 1));
    }

    private void SelectSlot(int slot)
    {
        if (slot == currentSlot || slot < 0 || slot >= weaponSlots.Length)
            return;

        currentSlot = slot;

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] != null)
                weaponSlots[i].SetActive(i == currentSlot);
        }
    }
}