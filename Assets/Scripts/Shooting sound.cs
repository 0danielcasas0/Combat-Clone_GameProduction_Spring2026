using UnityEngine;

public class Shootingsound : MonoBehaviour
{
    public KeyCode fireKey = KeyCode.LeftShift;
    public AudioSource shootingSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
    
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetKeyDown(fireKey))
        {
            shootingSound.Play();
        }
    }
}
