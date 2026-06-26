using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    public static ScoreManager Instance => instance;

    private Text scoreText;

    private int _myScore = 0;
    private int _bestScore = 0;
    private bool isGameOver = false;

    public int MyScore
    {
        get => _myScore;
        set
        {
            _myScore = value;

            if (!isGameOver && _myScore > _bestScore)
            {
                _bestScore = _myScore;
                PlayerPrefs.SetInt("BestScore", _bestScore);
                PlayerPrefs.Save();
                Debug.Log($"최고 점수 갱신: {_bestScore}");
            }

            UpdateScoreUI();
        }
    }

    public int BestScore => _bestScore;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadBestScore();

        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindScoreText();
        MyScore = 0;
        UpdateScoreUI();
        InvokeRepeating(nameof(IncreaseScore), 1f, 1f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindScoreText();
        MyScore = 0;
        UpdateScoreUI();
    }

    private void FindScoreText()
    {
        GameObject textObject = GameObject.Find("TimeText");
        if (textObject != null)
        {
            scoreText = textObject.GetComponent<Text>();
        }
        else
        {
            Debug.LogWarning("TimeText 오브젝트를 찾지 못했습니다.");
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {_myScore}   Best: {_bestScore}";
    }

    private void IncreaseScore()
    {
        if (!isGameOver)
        {
            MyScore += 1;
        }
    }

    private void LoadBestScore()
    {
        _bestScore = PlayerPrefs.GetInt("BestScore", 0);
        Debug.Log($"불러온 최고 점수: {_bestScore}");
    }

    public void SetGameOver()
    {
        isGameOver = true;
        Debug.Log("게임 오버 → 점수 멈춤");
    }
}
