using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankAIController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                  // drag Player1 here
    public float desiredDistance = 6f;        // AI tries to stay around this distance

    [Header("Movement")]
    public float moveSpeed = 3.8f;
    public float turnSpeed = 220f;            // degrees/sec
    public float wanderTurnSpeed = 120f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public float fireCooldown = 0.8f;
    public float fireAngleTolerance = 10f;    // degrees
    public float sightRange = 20f;

    [Header("Line of Sight")]
    public LayerMask wallMask;                // set to Walls layer
    public float selfHitIgnoreDistance = 0.6f;

    [Header("Behavior")]
    public float decisionInterval = 0.4f;     // how often it changes wander direction
    public float obstacleFeelDistance = 1.2f; // raycast ahead distance
    public float strafeChance = 0.25f;        // occasionally strafe instead of direct chase

    [Header("Anti-stuck")]
    public float stuckCheckInterval = 0.25f;
    public float stuckDistanceEpsilon = 0.03f;
    public float unstickTime = 0.5f;
    public float reverseSpeedFactor = 0.7f;
    public float avoidRayDistance = 1.2f;

    float stuckTimer;
    float unstickTimer;
    Vector2 lastPos;
    int unstickTurnSign = 1;

    Rigidbody2D rb;
    float fireTimer;
    float decisionTimer;

    // internal behavior choices
    float wanderSign = 1f;    // -1 or +1
    bool strafeMode;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = false; // allow hit spin; your hit reaction disables controllers anyway
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;
        decisionTimer -= Time.deltaTime;

        if (decisionTimer <= 0f)
        {
            decisionTimer = decisionInterval;
            wanderSign = (Random.value < 0.5f) ? -1f : 1f;
            strafeMode = (Random.value < strafeChance);
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 toTarget = (Vector2)(target.position - transform.position);
        float dist = toTarget.magnitude;

        // 1) Decide desired facing direction
        Vector2 desiredDir = toTarget.normalized;

        // Optional: strafe-ish behavior (adds Combat-like variety)
        if (strafeMode)
        {
            // rotate desired direction 90 degrees to circle target
            desiredDir = new Vector2(-desiredDir.y, desiredDir.x) * wanderSign;
        }

        // 2) Avoid obstacles: if something is directly ahead, bias turning away
        float avoidTurn = ObstacleAvoidTurn();

        // --- Stuck detection ---
        stuckTimer -= Time.fixedDeltaTime;
        if (stuckTimer <= 0f)
        {
            stuckTimer = stuckCheckInterval;

            float moved = Vector2.Distance(rb.position, lastPos);
            bool tryingToMove = true; // or check your move decision if you have one
            bool blocked = IsWallAhead(wallMask);

            if (tryingToMove && blocked && moved < stuckDistanceEpsilon)
            {
                // Enter unstick mode
                unstickTimer = unstickTime;
                unstickTurnSign = (Random.value < 0.5f) ? -1 : 1;
            }

            lastPos = rb.position;
        }

        // --- Unstick behavior ---
        if (unstickTimer > 0f)
        {
            unstickTimer -= Time.fixedDeltaTime;

            // Reverse and turn away from the corner
            float newAngle = rb.rotation + unstickTurnSign * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(newAngle);

            Vector2 backward = -transform.right;
            rb.MovePosition(rb.position + backward * (moveSpeed * reverseSpeedFactor * Time.fixedDeltaTime));

            // Skip normal AI movement this tick
            return;
        }


        // 3) Rotate toward either target or strafe direction
        RotateToward(desiredDir, avoidTurn);

        // 4) Move: approach if too far, back up if too close, otherwise drift
        float move = 0f;
        if (!strafeMode)
        {
            if (dist > desiredDistance + 1f) move = 1f;
            else if (dist < desiredDistance - 1f) move = -0.6f; // back up a bit
            else move = 0.2f; // mild creep
        }
        else
        {
            // strafe mode: keep moving forward while circling
            move = 1f;
        }

        Vector2 forward = transform.right;
        rb.MovePosition(rb.position + forward * (move * moveSpeed * Time.fixedDeltaTime));

        // 5) Shoot if we have a clean shot
        TryShootAtTarget(toTarget);
    }

    float ObstacleAvoidTurn()
    {
        // Cast a ray forward; if we see a wall close, turn away
        RaycastHit2D hit = Physics2D.Raycast(rb.position, transform.right, obstacleFeelDistance, wallMask);
        if (hit.collider == null) return 0f;

        // Also check a little to the left/right to pick a turn direction
        Vector2 leftDir = Rotate(transform.right, 25f);
        Vector2 rightDir = Rotate(transform.right, -25f);

        bool leftBlocked = Physics2D.Raycast(rb.position, leftDir, obstacleFeelDistance, wallMask).collider != null;
        bool rightBlocked = Physics2D.Raycast(rb.position, rightDir, obstacleFeelDistance, wallMask).collider != null;

        if (leftBlocked && !rightBlocked) return -1f;
        if (rightBlocked && !leftBlocked) return 1f;

        // both or unknown: pick a random-ish consistent sign
        return wanderSign;
    }

    void RotateToward(Vector2 desiredDir, float avoidTurnSign)
    {
        // Desired angle based on "forward = transform.right"
        float desiredAngle = Mathf.Atan2(desiredDir.y, desiredDir.x) * Mathf.Rad2Deg;

        // If avoiding obstacles, add a bit of turn bias
        if (Mathf.Abs(avoidTurnSign) > 0.01f)
        {
            desiredAngle += avoidTurnSign * 35f;
        }

        float newAngle = Mathf.MoveTowardsAngle(rb.rotation, desiredAngle, turnSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newAngle);
    }

    void TryShootAtTarget(Vector2 toTarget)
    {
        if (fireTimer > 0f) return;

        // must be roughly aiming at target
        Vector2 forward = transform.right;
        float angleTo = Vector2.Angle(forward, toTarget);
        if (angleTo > fireAngleTolerance) return;

        // must have line of sight (no wall between)
        if (!HasLineOfSight(toTarget)) return;

        Shoot();
        fireTimer = fireCooldown;
    }

    bool HasLineOfSight(Vector2 toTarget)
    {
        float dist = toTarget.magnitude;
        if (dist > sightRange) return false;

        Vector2 dir = toTarget.normalized;

        // start ray a bit in front so we don't hit our own collider
        Vector2 origin = rb.position + dir * selfHitIgnoreDistance;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist, wallMask);
        return hit.collider == null; // no wall hit => clear shot
    }

    void Shoot()
    {
        if (!bulletPrefab || !firePoint) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);

        // Ignore self collision
        Collider2D myCol = GetComponent<Collider2D>();
        Collider2D bulletCol = bullet.GetComponent<Collider2D>();
        if (myCol && bulletCol) Physics2D.IgnoreCollision(bulletCol, myCol);

        // Owner id (so Bullet2D can ignore self hits)
        var bulletScript = bullet.GetComponent<Bullet2D>();
        var myId = GetComponent<TankIdentity>();
        if (bulletScript != null && myId != null)
            bulletScript.ownerPlayerId = myId.playerId;

        // Launch
        Rigidbody2D brb = bullet.GetComponent<Rigidbody2D>();
        if (brb) brb.linearVelocity = (Vector2)transform.right * bulletSpeed;
    }

    static Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        return new Vector2(c * v.x - s * v.y, s * v.x + c * v.y);
    }

    bool IsWallAhead(LayerMask wallMask)
    {
        // “Feelers” straight, slight left, slight right
        Vector2 pos = rb.position;
        Vector2 fwd = transform.right;

        bool hitCenter = Physics2D.Raycast(pos, fwd, avoidRayDistance, wallMask).collider != null;
        bool hitLeft = Physics2D.Raycast(pos, Rotate(fwd, 25f), avoidRayDistance, wallMask).collider != null;
        bool hitRight = Physics2D.Raycast(pos, Rotate(fwd, -25f), avoidRayDistance, wallMask).collider != null;

        return hitCenter || hitLeft || hitRight;
    }
}