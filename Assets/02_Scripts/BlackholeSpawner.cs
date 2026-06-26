using UnityEngine;

public class BlackholeSpawner : MonoBehaviour
{
    public GameObject blackholePrefab;     // 생성할 블랙홀 프리팹
    public Transform[] spawnPoints;        // 스폰 위치들
    public float spawnInterval = 5f;       // 블랙홀 생성 주기

    public float moveSpeed = 2f;           // 좌우 이동 속도
    public float moveRange = 3f;           // 이동 범위

    private float timer;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 좌우 이동
        float offset = Mathf.PingPong(Time.time * moveSpeed, moveRange) - (moveRange / 2f);
        transform.position = new Vector3(startPos.x + offset, startPos.y, startPos.z);

        // 블랙홀 생성 타이머
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnBlackhole();
            timer = 0f;
        }
    }

    void SpawnBlackhole()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnLocation = spawnPoints[randomIndex];

        Instantiate(blackholePrefab, spawnLocation.position, spawnLocation.rotation);
        Debug.Log("블랙홀 소환");
    }
}