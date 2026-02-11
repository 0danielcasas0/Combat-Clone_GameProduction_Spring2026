using UnityEngine;

public class TankController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float turnSpeedKeyboard = 180f;   // deg/sec for keys

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public float fireCooldown = .8f;

    [Header("Controls")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode fireKey = KeyCode.LeftShift;

    [Header("Mouse Controls")]
    public bool enableMouseControls = true;  // Player1 ON, Player2 OFF
    public bool mouseAimOverridesRotation = true;
    public int moveMouseButton = 0;          // 0=LMB
    public int fireMouseButton = 1;          // 1=RMB

    Rigidbody2D rb;
    Camera cam;

    float moveInput;
    float turnInput;
    
    float fireTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
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

        // --- Mouse input (adds on top) ---
        bool mouseMove = enableMouseControls && Input.GetMouseButton(moveMouseButton);
        bool mouseFire = enableMouseControls && Input.GetMouseButton(fireMouseButton);

        // If mouse button is held to move, treat that as "move forward" in addition to keyboard
        if (mouseMove)
            moveInput = Mathf.Clamp(moveInput + 1f, -1f, 1f);

        // Fire
        fireTimer -= Time.deltaTime;

        bool keyboardFire = Input.GetKey(fireKey);
        if ((keyboardFire || mouseFire) && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireCooldown;
        }
    }

    void FixedUpdate()
    {
        // Face cursor (Player 1)
        if (enableMouseControls && mouseAimOverridesRotation)
        {
            if (cam == null) cam = Camera.main;
            if (cam != null)
            {
                Vector3 mouseScreen = Input.mousePosition;

                // ScreenToWorldPoint needs a Z distance from camera for perspective cameras.
                // For 2D (orthographic) this still works fine; we just set z so it's in front of the camera.
                mouseScreen.z = -cam.transform.position.z;

                Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

                Vector2 toMouse = (Vector2)(mouseWorld - transform.position);
                if (toMouse.sqrMagnitude > 0.0001f)
                {
                    // Your "forward" is transform.right, so angle uses atan2(y, x)
                    float angleDeg = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;
                    rb.MoveRotation(angleDeg);
                }
            }
        }
        else
        {
            // Keyboard rotation when mouse aim is off
            float newRot = rb.rotation + (turnInput * turnSpeedKeyboard * Time.fixedDeltaTime);
            rb.MoveRotation(newRot);
        }

        // Move forward/back along facing direction (transform.right = forward)
        Vector2 forward = transform.right;
        rb.MovePosition(rb.position + forward * (moveInput * moveSpeed * Time.fixedDeltaTime));
    
        // Rotation: keyboard turn + mouse steer
        //float rotFromKeyboard = turnInput * turnSpeedKeyboard;
        // Rotate
        //float newRot = rb.rotation + (rotFromKeyboard + rotFromMouse) * Time.fixedDeltaTime;
        //rb.MoveRotation(newRot);

        // Move forward/back
        //Vector2 forward = transform.right;
        //rb.MovePosition(
          //  rb.position + forward * (moveInput * moveSpeed * Time.fixedDeltaTime)
        //);
    }

    void Shoot()
    {
        if (!bulletPrefab || !firePoint) return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            transform.rotation
        );

        var bulletScript = bullet.GetComponent<Bullet2D>();
        var myId = GetComponent<TankIdentity>();

        if (bulletScript != null && myId != null)
            bulletScript.ownerPlayerId = myId.playerId;

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