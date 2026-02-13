using UnityEngine;
using TMPro;
using System.Collections;

public class FlashingText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text UiText; // Reference to the UI Text component to flash 
    public float flashrate = 0.5f; // Rate at which the text will flash (in seconds)
    void Start()
    {
        if (UiText != null)
        {
            StartCoroutine(FlashText()); // Start the flashing coroutine if the UiText reference is set
        }
        else
        {
            Debug.LogError("UiText reference is not set in the inspector.");
        }
    }
    IEnumerator FlashText()
    {
        UiText.enabled = true; // Ensure the text starts as visible
        yield return new WaitForSeconds(flashrate); // Wait for the specified flash rate
        UiText.enabled = false; // Hide the text
        yield return new WaitForSeconds(flashrate); // Wait for the specified flash rate
        StartCoroutine(FlashText()); // Restart the coroutine to continue flashing
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
