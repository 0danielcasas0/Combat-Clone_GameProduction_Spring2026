using UnityEngine;
using UnityEngine.UI;

public class CharacterLoader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SpriteRenderer characterRenderer; // Reference to the SpriteRenderer component to display the character
    public Sprite characterSprite;
    void Start()
    {
        int characterIndex = PlayerPrefs.GetInt("CurrentCharacterIndex", 0);
        characterSprite = Resources.Load<Sprite>("Characters/Character_" + characterIndex);
        characterRenderer.sprite = characterSprite;
        
        if (characterSprite == null)
        {
            Debug.LogError("Character sprite not found for index: " + characterIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
