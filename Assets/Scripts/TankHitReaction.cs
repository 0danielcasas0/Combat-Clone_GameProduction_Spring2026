using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankHitReaction : MonoBehaviour
{
    [Header("Knockback")] 
    public float knockbackForce = 6f;

    [Header("Spin")]
    public float spinSpeed = -1080f; // degrees per second (negative = clockwise)
    public float stunTime = 0.6f;

    [Header("Damping")]
    public float angularDragWhileStunned = 3f;

    Rigidbody2D rb;

    TankController playerController;
    TankAIController aiController;

    bool stunned;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Helps spins settle nicely
        rb.angularDamping = angularDragWhileStunned;

        // Cache whichever controller(s) might exist on this tank
        playerController = GetComponent<TankController>();
        aiController = GetComponent<TankAIController>();
    }

    public void OnHit(Vector2 hitDirection)
    {
        if (stunned) return;

        StartCoroutine(HitRoutine(hitDirection));
    }

    IEnumerator HitRoutine(Vector2 hitDir)
    {
        stunned = true;

        // Disable control so it doesn't overwrite spin/knockback
        if (playerController != null) playerController.enabled = false;
        if (aiController != null) aiController.enabled = false;

        // Reset motion then apply hit
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Knockback away from impact
        rb.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);

        // Start clockwise spin
        rb.angularVelocity = spinSpeed;

        // Wait
        yield return new WaitForSeconds(stunTime);

        // Stop motion
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Re-enable control
        if (playerController != null) playerController.enabled = true;
        if (aiController != null) aiController.enabled = true;

        stunned = false;
    }
}
