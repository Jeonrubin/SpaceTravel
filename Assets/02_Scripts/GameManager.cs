using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard,
        Korean
    }

    public static GameManager Instance;
    public Difficulty difficulty;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
