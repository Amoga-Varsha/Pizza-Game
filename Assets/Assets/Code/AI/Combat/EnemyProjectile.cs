using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 6f;

    private Vector3 velocity;
    private int damage;
    private GameObject owner;

    public void Initialize(Vector3 direction, float speed, int damageValue, GameObject ownerObject)
    {
        velocity = direction.normalized * speed;
        damage = damageValue;
        owner = ownerObject;
    }

    private void OnEnable()
    {
        if (lifetime > 0f)
        {
            Destroy(gameObject, lifetime);
        }
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner != null && other.transform.IsChildOf(owner.transform))
        {
            return;
        }

        var health = other.GetComponentInParent<PlayerHealth>();
        if (health != null)
        {
            health.ApplyDamage(damage);
        }

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
