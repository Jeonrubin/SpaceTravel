using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitBtn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameQuit()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
