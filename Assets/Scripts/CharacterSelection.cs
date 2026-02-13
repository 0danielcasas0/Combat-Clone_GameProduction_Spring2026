using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public UnityEngine.UI.Image CharacterRenderer; // Reference to the UI Image component to display the character
    public Sprite[] characterSprites; // Array of character sprites to choose from
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int currentCharacterIndex = 0; // Index to keep track of the currently selected character
    void Start()
    {
        updatecharacterImage(); 
    }


    public void NextCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex + 1) % characterSprites.Length; // Move to the next character, wrap around if at the end
        updatecharacterImage(); 
    }

    public void PreviousCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex - 1 + characterSprites.Length) % characterSprites.Length; // Move to the previous character, wrap around if at the beginning
        updatecharacterImage(); // Update the character image to reflect the new selection
    }

    void updatecharacterImage()
    {
        CharacterRenderer.sprite = characterSprites[currentCharacterIndex]; // Set the UI Image sprite to the currently selected character sprite
    }
    // Update is called once per frame
    public void savecharacterSelection()
    {
        PlayerPrefs.SetInt("CurrentCharacterIndex", currentCharacterIndex); // Save the selected character index to PlayerPrefs
        PlayerPrefs.Save();    
    }
    void Update()
    {
        
    }
}
