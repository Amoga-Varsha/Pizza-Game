using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float lifeTime = 5f;
    public bool stickToTarget = true;

    private float damage;
    private Rigidbody rb;
    private bool hasHit;

    public void Initialize(float damage, float speed)
    {
        this.damage = damage;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
            return;

        hasHit = true;

        // Damage
        if (collision.collider.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
        }

        // Stick to enemy
        if (stickToTarget)
        {
            rb.isKinematic = true;
            transform.SetParent(collision.transform);
        }

        // Stop movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Destroy later if stuck
        Destroy(gameObject, 10f);
    }
}
