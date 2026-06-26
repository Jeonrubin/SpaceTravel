using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public static bool IsAbsorbed = false; // ✅ 흡수 상태 플래그

    [Header("UI & Effects")]
    public GameObject gameOver;
    public Slider PlayerHP;

    [Header("Skill Prefabs")]
    public GameObject skillPrefab;
    public GameObject barrierPrefab;

    [Header("Movement")]
    public float Limitx = 10f;
    public float Limity = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip enemyHitSound;
    [SerializeField] private AudioClip blackholeHitSound;
    [SerializeField] private AudioClip timeSound;
    [SerializeField] private AudioClip barrierSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip skillSound;

    private AudioSource audioSource;
    private PlayerStats stats;

    private int selectedShip;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        stats = GetComponent<PlayerStats>();

        selectedShip = PlayerPrefs.GetInt("SelectedShip", 0);
        int childCount = transform.childCount;

        if (selectedShip < 0 || selectedShip >= childCount)
        {
            Debug.LogError($"잘못된 우주선 ID: {selectedShip}. 기본값 0번 우주선을 사용합니다.");
            selectedShip = 0;
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(i == selectedShip);
        }

        if (stats == null)
        {
            Debug.LogError("PlayerStats 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        if (PlayerHP != null)
        {
            PlayerHP.maxValue = stats.maxHp;
            PlayerHP.minValue = 0;
            PlayerHP.value = stats.hp;
        }
        else
        {
            Debug.LogWarning("PlayerHP 슬라이더가 연결되지 않았습니다.");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource가 연결되지 않았습니다.");
        }
    }

    void Update()
    {
        if (stats == null || IsAbsorbed) return; // ✅ 흡수 상태일 땐 이동 금지

        Vector3 pos = transform.position;
        float speed = stats.moveSpeed;

        float xInput = 0f;
        float yInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) xInput = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) xInput = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) yInput = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) yInput = -1f;

        if (stats.IsControlReversed())
        {
            xInput *= -1f;
            yInput *= -1f;
        }

        Vector3 moveDir = new Vector3(xInput, yInput, 0).normalized;
        pos += moveDir * speed * Time.unscaledDeltaTime;

        pos.x = Mathf.Clamp(pos.x, -Limitx, Limitx);
        pos.y = Mathf.Clamp(pos.y, -Limity, Limity);
        transform.position = pos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (stats == null) return;

        if (other.CompareTag("Enemy") || other.CompareTag("Blackhole"))
        {
            int damage = 0;

            if (other.CompareTag("Enemy"))
            {
                if (enemyHitSound != null && audioSource != null)
                    audioSource.PlayOneShot(enemyHitSound, 2.0f);

                switch (GameManager.Instance.difficulty)
                {
                    case Difficulty.Easy: damage = 20; break;
                    case Difficulty.Normal: damage = 30; break;
                    case Difficulty.Hard: damage = 45; break;
                    case Difficulty.Korean: damage = 70; break;
                }
            }
            else if (other.CompareTag("Blackhole"))
            {
                if (blackholeHitSound != null && audioSource != null)
                    audioSource.PlayOneShot(blackholeHitSound);

                switch (GameManager.Instance.difficulty)
                {
                    case Difficulty.Easy: damage = 30; break;
                    case Difficulty.Normal: damage = 45; break;
                    case Difficulty.Hard: damage = 70; break;
                    case Difficulty.Korean: damage = 100000; break;
                }

                // ✅ 블랙홀 흡수 시 정지
                IsAbsorbed = true;
                StartCoroutine(ReleaseAfterDelay(0.5f)); // 몇 초 후 복원
            }

            stats.TakeDamage(damage);
        }
    }

    private IEnumerator ReleaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsAbsorbed = false;
        Debug.Log("✅ 플레이어 복원됨");
    }

    public void TriggerSkillEffect()
    {
        switch (selectedShip)
        {
            case 0:
                audioSource.PlayOneShot(timeSound);
                StartCoroutine(SlowTimeCoroutine());
                break;
            case 1:
                if (barrierPrefab != null)
                {
                    stats.ActivateBarrier(6f);
                    audioSource.PlayOneShot(barrierSound);
                    Debug.Log("배리어 생성됨!");
                }
                else
                {
                    Debug.LogWarning("barrierPrefab이 설정되지 않았습니다.");
                }
                break;
            case 2:
                audioSource.PlayOneShot(dashSound);
                StartCoroutine(DashCoroutine());
                 break;
            case 3:
                if (skillPrefab != null)
                {
                    Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
                    Quaternion prefabRotation = skillPrefab.transform.rotation;
                    audioSource.PlayOneShot(skillSound);
                    Instantiate(skillPrefab, spawnPos, prefabRotation);
                    Debug.Log("스킬 오브젝트 생성됨!");
                }
                else
                {
                    Debug.LogWarning("projectilePrefab이 설정되지 않았습니다.");
                }
                break;
            default:
                Debug.LogWarning("알 수 없는 우주선 ID입니다.");
                break;
        }
    }

    private IEnumerator SlowTimeCoroutine()
    {
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    private IEnumerator DashCoroutine()
    {
        float dashSpeed = stats.moveSpeed * 5f;
        float dashDuration = 0.2f;

        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftArrow)) direction = Vector3.left;
        else if (Input.GetKey(KeyCode.RightArrow)) direction = Vector3.right;
        else if (Input.GetKey(KeyCode.UpArrow)) direction = Vector3.up;
        else if (Input.GetKey(KeyCode.DownArrow)) direction = Vector3.down;

        if (stats.IsControlReversed())
            direction *= -1f;

        if (direction == Vector3.zero) yield break;

        float timeElapsed = 0f;
        while (timeElapsed < dashDuration)
        {
            transform.position += direction * dashSpeed * Time.deltaTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}