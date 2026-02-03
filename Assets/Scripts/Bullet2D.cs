using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet2D : MonoBehaviour
{
    public float lifeTime = 3f;

    // Optional: keep speed consistent after bounces
    public float desiredSpeed = 8f;

    // Optional: limit ricochets
    public int maxBounces = 8;

    [HideInInspector] public int ownerPlayerId;

    Rigidbody2D rb;
    int bounceCount;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        // Keep constant speed (optional but makes ricochets feel �arcade�)
        if (rb.linearVelocity.sqrMagnitude > 0.001f)
            rb.linearVelocity = rb.linearVelocity.normalized * desiredSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If we hit a tank, do hit reaction and destroy bullet
        TankHitReaction hit = collision.collider.GetComponentInParent<TankHitReaction>();
        TankIdentity identity = collision.collider.GetComponentInParent<TankIdentity>();

        if (hit != null && identity != null)
        {
            if (identity.playerId != ownerPlayerId)
            {
                Vector2 hitDir = (identity.transform.position - transform.position).normalized;
                hit.OnHit(hitDir);
            }

            Destroy(gameObject);
            return;
        }

        // Otherwise we hit a wall/obstacle: bounce instead of destroying
        bounceCount++;
        if (bounceCount > maxBounces)
        {
            Destroy(gameObject);
        }
        // No Destroy here = physics material can do its bounce
    }
}
