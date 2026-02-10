using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemanager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadScene()
    {
        SceneManager.LoadScene("Map1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
