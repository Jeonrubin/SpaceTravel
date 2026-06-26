using UnityEngine;
using UnityEngine.SceneManagement;

public class MainBtn : MonoBehaviour
{
    public void OverMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("00_Intro");
    }
}
