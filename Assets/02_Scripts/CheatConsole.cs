using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CheatConsole : MonoBehaviour
{
    public static CheatConsole Instance;

    [Header("Cheat UI")]
    public GameObject cheatPanel;
    public InputField inputField;

    private bool isOpen = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        if (cheatPanel != null)
        {
            cheatPanel.SetActive(false); // 시작 시 패널 꺼두기
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash)) // 슬래시(/) 키로 열고 닫기
        {
            isOpen = !isOpen;

            if (cheatPanel != null)
            {
                cheatPanel.SetActive(isOpen);

                if (isOpen)
                {
                    inputField.text = "";
                    inputField.ActivateInputField();
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                }
            }
        }

        if (isOpen && Input.GetKeyDown(KeyCode.Return))
        {
            string command = inputField.text.Trim().ToLower();
            HandleCommand(command);

            cheatPanel.SetActive(false);
            isOpen = false;
        }
    }

    void HandleCommand(string cmd)
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogWarning("PlayerController.Instance 가 존재하지 않습니다.");
            return;
        }

        PlayerStats stats = PlayerController.Instance.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogWarning("PlayerStats 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        switch (cmd)
        {
            case "hp":
                stats.RestoreFullHealth();
                Debug.Log("체력 회복!");
                break;
            case "god":
                stats.ActivateBarrier(9999999f); // 무한 배리어
                Debug.Log("갓모드 활성화!");
                break;
            case "clear":
                Debug.Log("적 모두 제거! (기능은 아직 구현되지 않음)");
                break;
            default:
                Debug.Log("알 수 없는 명령어: " + cmd);
                break;
        }
    }
}