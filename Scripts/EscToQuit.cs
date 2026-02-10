using UnityEngine;
using UnityEditor;

public class escToQuit : MonoBehaviour
{
	// Key to hold down
	public KeyCode quitKey = KeyCode.Escape;
	// Duration to hold the key
	public float maxTimer = 3.0f; 
	private float timer = 0.0f;
	private bool isHoldingKey = false;

	private void Update()
	{
		// Check if the quit key is being held down
		if (Input.GetKey(quitKey))
		{
			if (!isHoldingKey)
			{
				// Start the timer when the key is first pressed
				isHoldingKey = true;
				timer = 0.0f;
			}
			else
			{
				// Increment the timer while the key is held
				timer += Time.deltaTime;
				// Check if the timer has exceeded the max duration
				if (timer >= maxTimer)
				{
					QuitGame();
				}
			}
		}
		else
		{
			// Reset the timer if the key is released
			isHoldingKey = false;
			timer = 0.0f;
		}
	}
	private void QuitGame()
	{
		Debug.Log("Quitting game...");
		// Quit the game 
		Application.Quit();
	}
}


