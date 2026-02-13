using UnityEngine;
using TMPro;

public class GameManagerCombat : MonoBehaviour
{
    public static GameManagerCombat Instance { get; private set; }
    public GameObject winScreen;

	[Header("Players")]
    public GameObject player1;
    public GameObject player2;

    [Header("Rules")]
    public int pointsToWin = 3;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text winnerMessage;

	int p1Score;
    int p2Score;
    bool gameOver;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (winnerMessage != null)
            winnerMessage.text = "";
		UpdateUI();
    }

    // Call this once per HIT that should score
    public void RegisterHit(int victimPlayerId, int attackerPlayerId)
    {
        if (gameOver) return;
        if (victimPlayerId == attackerPlayerId) return; // no self score

        // Award point
        if (attackerPlayerId == 1) p1Score++;
        else if (attackerPlayerId == 2) p2Score++;

        UpdateUI();

        // Check win condition
        int winner = GetWinner();
        if (winner != 0)
        {
            ShowVictoryScreen();
			EndGame(winner);
            return;
        }


    }

    int GetWinner()
    {
        if (p1Score >= pointsToWin) return 1;
        if (p2Score >= pointsToWin) return 2;
        return 0;
    }

    void EndGame(int winnerPlayerId)
    {
        gameOver = true;

        if (winnerMessage != null)
            winnerMessage.text = $"PLAYER {winnerPlayerId} WINS!";

        DisableTankControls(player1);
        DisableTankControls(player2);
    }

    void DisableTankControls(GameObject tank)
    {
        if (tank == null) return;

        // Stop physics completely (prevents movement even if some script is still running)
        var rb = tank.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false; // <- hard stop
        }

        // Disable known controllers anywhere on the tank object (root or children)
        foreach (var c in tank.GetComponentsInChildren<TankController>(true))
            c.enabled = false;

        foreach (var ai in tank.GetComponentsInChildren<TankAIController>(true))
            ai.enabled = false;
    }

    public void ShowVictoryScreen()
    {
        winScreen.SetActive(true);
	}

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"P1: {p1Score}   P2: {p2Score}";
    }
}
