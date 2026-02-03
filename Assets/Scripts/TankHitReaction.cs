using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankHitReaction : MonoBehaviour
{
    public float knockbackForce = 6f;

    [Header("Spin")]
    public float spinSpeed = -1080f; // degrees per second (negative = clockwise)
    public float stunTime = 0.6f;

    Rigidbody2D rb;
    TankController controller;

    bool stunned;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<TankController>();

        // Helps spinning stop nicely
        rb.angularDamping = 3f;
    }

    public void OnHit(Vector2 hitDirection)
    {
        if (stunned) return;

        StartCoroutine(HitRoutine(hitDirection));
    }

    IEnumerator HitRoutine(Vector2 hitDir)
    {
        stunned = true;

        // Disable control
        if (controller != null)
            controller.enabled = false;

        // Reset motion
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Knockback (away from bullet)
        rb.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);

        // Start clockwise spin
        rb.angularVelocity = spinSpeed;

        // Wait
        yield return new WaitForSeconds(stunTime);

        // Stop motion
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Restore control
        if (controller != null)
            controller.enabled = true;

        stunned = false;
    }
}
