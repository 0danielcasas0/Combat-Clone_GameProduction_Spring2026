using UnityEngine;

public class TankController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float turnSpeed = 180f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public float fireCooldown = 0.3f;

    [Header("Controls")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode fireKey = KeyCode.LeftShift;

    Rigidbody2D rb;

    float moveInput;
    float turnInput;
    float fireTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void Update()
    {
        // Move
        moveInput = 0f;
        if (Input.GetKey(forwardKey)) moveInput = 1f;
        if (Input.GetKey(backKey)) moveInput = -1f;

        // Turn
        turnInput = 0f;
        if (Input.GetKey(leftKey)) turnInput = 1f;
        if (Input.GetKey(rightKey)) turnInput = -1f;

        // Fire
        fireTimer -= Time.deltaTime;

        if (Input.GetKey(fireKey) && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireCooldown;
        }
    }

    void FixedUpdate()
    {
        // Rotate
        float newRot = rb.rotation + turnInput * turnSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(newRot);

        // Move forward/back
        Vector2 forward = transform.right;
        rb.MovePosition(
            rb.position + forward * (moveInput * moveSpeed * Time.fixedDeltaTime)
        );
    }

    void Shoot()
    {
        if (!bulletPrefab || !firePoint) return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            transform.rotation
        );

        // Ignore self collision
        Collider2D myCol = GetComponent<Collider2D>();
        Collider2D bulletCol = bullet.GetComponent<Collider2D>();

        if (myCol && bulletCol)
            Physics2D.IgnoreCollision(bulletCol, myCol);

        Rigidbody2D brb = bullet.GetComponent<Rigidbody2D>();

        if (brb)
            brb.linearVelocity = (Vector2)transform.right * bulletSpeed;
    }
}