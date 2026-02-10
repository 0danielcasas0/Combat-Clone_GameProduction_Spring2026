using UnityEngine;

public class esckeyexit : MonoBehaviour
{
    public KeyCode exitKey = KeyCode.Escape; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(exitKey))
        {
            Application.Quit();

            Debug.Log("Escape key activated. Application exiting...");
        }
    }
}
