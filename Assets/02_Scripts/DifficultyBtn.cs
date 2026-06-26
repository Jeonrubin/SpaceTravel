using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyBtn : MonoBehaviour
{
    public GameManager.Difficulty selectedDifficulty;

    public void GameStart()
    {
        int randomIndex = Random.Range(0, 3); // Skybox 종류가 3개 있다고 가정
        PlayerPrefs.SetInt("SkyboxIndex", randomIndex);
        GameManager.Instance.difficulty = selectedDifficulty;

        SceneManager.LoadScene("01_Main");
    }
}
