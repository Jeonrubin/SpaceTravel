using UnityEngine;

public class Enemy_flickerController : MonoBehaviour
{
    public float moveSpeed = 6f;             // 전진 속도
    public float phaseInterval = 1.5f;       // 위상 이동 간격
    public float jumpDistance = 3f;          // 순간이동 거리

    private float phaseTimer = 0f;
    private Renderer bulletRenderer;

    public GameObject effectPrefab; // 이펙트 프리팹
    public Transform spawnPoint;    // 소환 위치

    void Start()
    {
        bulletRenderer = GetComponent<Renderer>();
        Destroy(gameObject, 20f);
    }

    void Update()
    {
        // 앞으로 전진
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // 시간 체크 → 위상 전이
        phaseTimer += Time.deltaTime;
        if (phaseTimer >= phaseInterval)
        {
            PhaseShift();
            phaseTimer = 0f;
        }
    }

    void PhaseShift()
    {
        // 1. 깜빡임 (렌더러 끄기)
        bulletRenderer.enabled = false;

        // 2. 랜덤한 방향으로 순간이동
        Vector3 jumpDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        transform.position += jumpDir * jumpDistance;

        // 3. 짧은 시간 뒤 다시 보임
        Invoke("Reappear", 0.15f);
    }

    void Reappear()
    {
        bulletRenderer.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Blackhole") || other.CompareTag("Destroy") || other.CompareTag("Skill"))
            SpawnEffect();
        Destroy(gameObject);
    }

    void SpawnEffect()
    {
        GameObject effect = Instantiate(effectPrefab, spawnPoint.position, spawnPoint.rotation);
        effect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // 원하는 크기로 조절
    }
}