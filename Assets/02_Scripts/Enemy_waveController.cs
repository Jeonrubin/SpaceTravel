using UnityEngine;

public class Enemy_waveController : MonoBehaviour
{
    public float movespeed = 4f;
    public float waveSpeed = 6f;
    public float waveAmplitude = 0.5f;

    private float elapsed;

    public GameObject effectPrefab; // 이펙트 프리팹
    public Transform spawnPoint;    // 소환 위치

    void Start()
    {
        Destroy(gameObject, 20f);
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // 앞으로 이동
        transform.Translate(-transform.forward * movespeed * Time.deltaTime, Space.Self);

        // 상하 흔들림
        float waveOffset = Mathf.Sin(elapsed * waveSpeed) * waveAmplitude;
        transform.Translate(Vector3.up * waveOffset, Space.Self);
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