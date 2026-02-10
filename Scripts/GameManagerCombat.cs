using UnityEngine;
using TMPro;

public class GameManagerCombat : MonoBehaviour
{
    public static GameManagerCombat Instance { get; private set; }

    [Header("Players")]
    public GameObject player1;
    public GameObject player2;

    [Header("Spawns")]
    public Transform p1Spawn;
    public Transform p2Spawn;

    [Header("UI")]
    public TMP_Text p1_scoreText; // use TextMeshPro
    public TMP_Text p2_scoreText;


	int p1Score;
    int p2Score;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void OnTankKilled(int victimPlayerId, int killerPlayerId)
    {
        // Score
        if (killerPlayerId == 1) p1Score++;
        else if (killerPlayerId == 2) p2Score++;

        // Respawn victim
        if (victimPlayerId == 1) Respawn(player1, p1Spawn);
        else if (victimPlayerId == 2) Respawn(player2, p2Spawn);

        UpdateUI();
    }

    void Respawn(GameObject tank, Transform spawn)
    {
        if (tank == null || spawn == null) return;

        // Reset physics + teleport
        var rb = tank.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = spawn.position;
            rb.rotation = spawn.eulerAngles.z;
        }
        else
        {
            tank.transform.position = spawn.position;
            tank.transform.rotation = spawn.rotation;
        }
    }

    void UpdateUI()
    {
        if (p1_scoreText != null)
            p1_scoreText.text = $"P1: {p1Score}";
        if (p2_scoreText != null)
            p2_scoreText.text = $"P2: {p2Score}";
	}
}