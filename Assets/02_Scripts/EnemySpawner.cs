using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static GameManager;

public class EnemySpawner : MonoBehaviour
{
    [Header("탄막 프리팹")]
    public GameObject[] bulletPrefabs; // 0: 기본, 1: Sine, 2: Wave, 3: Homing, 4: Flicker

    [Header("스폰 위치")]
    public Transform[] spawnPoints;

    [Header("패턴 설정")]
    public bool enableRandomPatterns = true;
    public bool allowCirclePattern = true;
    public bool allowSpiralPattern = true;
    public bool allowRotatingPattern = true;
    public bool allowZigzagPattern = true;
    public bool allowSingleTarget = true;
    public bool allowSineBullet = true;
    public bool allowWaveBullet = true;
    public bool allowTargetedBurst = true;
    public bool allowHomingBullet = true;
    public bool allowFlickerBullet = true;

    [Header("탄막 속성")]
    public float rotationSpeed = 1f;
    public float spiralDelay = 0.05f;
    public float spreadSpeed = 1f;

    [Header("이동 관련")]
    public float moveSpeed = 2f;
    public float moveRange = 3f;

    private float timer;
    private float nextSpawnTime;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        StartCoroutine(WaitForGameManagerAndStart());
    }

    IEnumerator WaitForGameManagerAndStart()
    {
        while (GameManager.Instance == null)
        {
            Debug.Log("GameManager 인스턴스 대기 중...");
            yield return null;
        }

        SetNextSpawnTime();
    }

    void Update()
    {
        if (GameManager.Instance == null) return; // 혹시 몰라 Update도 방어

        float offset = Mathf.PingPong(Time.time * moveSpeed, moveRange) - (moveRange / 2f);
        transform.position = new Vector3(startPos.x + offset, startPos.y, startPos.z);

        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            timer = 0f;
            SetNextSpawnTime();

            if (enableRandomPatterns)
                SpawnRandomPattern();
            else
                SpawnSingleEnemy();
        }
    }

    void SetNextSpawnTime()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager 인스턴스를 찾을 수 없습니다. 기본 스폰 시간 적용");
            nextSpawnTime = 3f;
            return;
        }

        switch (GameManager.Instance.difficulty)
        {
            case Difficulty.Easy:
                nextSpawnTime = Random.Range(5f, 10f);
                break;
            case Difficulty.Normal:
                nextSpawnTime = Random.Range(4f, 8f);
                break;
            case Difficulty.Hard:
                nextSpawnTime = Random.Range(3f, 6f);
                break;
            case Difficulty.Korean:
                nextSpawnTime = Random.Range(2f, 4f);
                break;
            default:
                nextSpawnTime = 5f;
                break;
        }
    }

    void SpawnRandomPattern()
    {
        var choices = new List<int>();
        if (allowCirclePattern) choices.Add(0);
        if (allowSpiralPattern) choices.Add(1);
        if (allowRotatingPattern) choices.Add(2);
        if (allowZigzagPattern) choices.Add(3);
        if (allowSingleTarget) choices.Add(4);
        if (allowSineBullet) choices.Add(5);
        if (allowWaveBullet) choices.Add(6);
        if (allowTargetedBurst) choices.Add(7);
        if (allowHomingBullet) choices.Add(8);
        if (allowFlickerBullet) choices.Add(9);

        if (choices.Count == 0)
        {
            Debug.LogWarning("허용된 패턴 없음. 기본 적 생성!");
            SpawnSingleEnemy();
            return;
        }

        int selected = choices[Random.Range(0, choices.Count)];

        switch (selected)
        {
            case 0: SpawnCirclePattern(); break;
            case 1: StartCoroutine(SpawnSpiralPattern()); break;
            case 2: SpawnCirclePattern(useRotation: true); break;
            case 3: SpawnCirclePattern(useZigzag: true); break;
            case 4: SpawnSingleEnemy(); break;
            case 5: SpawnSpecialBullet(1); break;
            case 6: SpawnSpecialBullet(2); break;
            case 7: SpawnTargetedBurstPattern(); break;
            case 8: SpawnHomingBullet(); break;
            case 9: SpawnFlickerBullet(); break;
        }
    }

    void SpawnSingleEnemy()
    {
        if (spawnPoints.Length == 0 || bulletPrefabs.Length == 0) return;

        int index = Random.Range(0, spawnPoints.Length);
        Instantiate(bulletPrefabs[0], spawnPoints[index].position, Quaternion.identity);
    }

    void SpawnCirclePattern(bool useRotation = false, bool useZigzag = false)
    {
        Vector3 center = transform.position;
        int count = GetBulletCount();

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            if (useRotation)
                angle += Time.time * rotationSpeed;

            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            if (useZigzag)
            {
                direction += new Vector3(0f, Mathf.Sin(Time.time + i) * 0.3f, 0f);
                direction.Normalize();
            }

            GameObject bullet = Instantiate(bulletPrefabs[0], center, Quaternion.identity);
            EnemySpread spread = bullet.GetComponent<EnemySpread>();
            if (spread != null)
                spread.SetDirection(direction * spreadSpeed);
        }
    }

    IEnumerator SpawnSpiralPattern()
    {
        Vector3 center = transform.position;
        int count = GetBulletCount();

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count + Time.time;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;

            GameObject bullet = Instantiate(bulletPrefabs[0], center, Quaternion.identity);
            EnemySpread spread = bullet.GetComponent<EnemySpread>();
            if (spread != null)
                spread.SetDirection(dir * spreadSpeed);

            yield return new WaitForSeconds(spiralDelay);
        }
    }

    void SpawnSpecialBullet(int prefabIndex)
    {
        if (bulletPrefabs.Length <= prefabIndex) return;
        Instantiate(bulletPrefabs[prefabIndex], transform.position, Quaternion.identity);
    }

    void SpawnTargetedBurstPattern()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector3 center = transform.position;
        Vector3 toPlayer = (player.transform.position - center).normalized;

        int count = GetBulletCount();
        float spreadAngle = 30f;

        for (int i = 0; i < count; i++)
        {
            float angleOffset = Mathf.Lerp(-spreadAngle, spreadAngle, (float)i / (count - 1));
            Quaternion rotation = Quaternion.AngleAxis(angleOffset, Vector3.up);
            Vector3 direction = rotation * toPlayer;

            GameObject bullet = Instantiate(bulletPrefabs[0], center, Quaternion.identity);
            EnemySpread spread = bullet.GetComponent<EnemySpread>();
            if (spread != null)
                spread.SetDirection(direction * spreadSpeed);
        }
    }

    void SpawnHomingBullet()
    {
        if (bulletPrefabs.Length <= 3) return;
        Instantiate(bulletPrefabs[3], transform.position, Quaternion.identity);
    }

    void SpawnFlickerBullet()
    {
        if (bulletPrefabs.Length <= 4) return;
        Instantiate(bulletPrefabs[4], transform.position, Quaternion.identity);
    }

    int GetBulletCount()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager 인스턴스를 찾을 수 없습니다. 기본 탄 개수 사용.");
            return 8;
        }

        switch (GameManager.Instance.difficulty)
        {
            case Difficulty.Easy: return Random.Range(4, 6);
            case Difficulty.Normal: return Random.Range(6, 8);
            case Difficulty.Hard: return Random.Range(8, 10);
            case Difficulty.Korean: return Random.Range(10, 12);
            default: return 12;
        }
    }
}
