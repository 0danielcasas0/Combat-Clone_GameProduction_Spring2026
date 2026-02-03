using UnityEngine;

public class Bullet2D : MonoBehaviour
{
    [Header("Settings")]
    public float lifeTime = 3f;

    [HideInInspector]
    public int ownerPlayerId;

    void Start()
    {
        // Auto-destroy after time so bullets don't live forever
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Try to find a tank on what we hit
        TankHitReaction hit =
            collision.collider.GetComponentInParent<TankHitReaction>();

        TankIdentity identity =
            collision.collider.GetComponentInParent<TankIdentity>();

        // If we hit a tank
        if (hit != null && identity != null)
        {
            // Prevent self-hits (optional safety)
            if (identity.playerId != ownerPlayerId)
            {
                // Direction from bullet to tank (for knockback)
                Vector2 hitDir =
                    (identity.transform.position - transform.position).normalized;

                hit.OnHit(hitDir);
            }

            // Bullet always dies on impact
            Destroy(gameObject);
            return;
        }

        // Hit anything else (wall, obstacle, etc)
        Destroy(gameObject);
    }
}
