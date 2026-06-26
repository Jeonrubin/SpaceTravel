using UnityEngine;
using UnityEngine.TestTools;

public class SimpleItemSpawner : MonoBehaviour
{
    public GameObject[] items;         // 아이템 프리팹들
    public Transform[] spawnPoints;    // 스폰 위치들
    public float spawnTime = 5f;

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

        timer += Time.deltaTime;

        if (timer >= spawnTime)
        {
            SpawnItem();
            timer = 0f;
        }
    }

    void SpawnItem()
    {
        // 배열이 비어있으면 실행하지 않음
        if (items == null || items.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("아이템 또는 스폰 위치가 설정되지 않았습니다.");
            return;
        }

        // 랜덤 인덱스 선택 (배열 길이 기준)
        int itemIndex = Random.Range(0, items.Length);
        int pointIndex = Random.Range(0, spawnPoints.Length);

        // 아이템 생성
        Instantiate(items[itemIndex], spawnPoints[pointIndex].position, Quaternion.identity);
    }
}