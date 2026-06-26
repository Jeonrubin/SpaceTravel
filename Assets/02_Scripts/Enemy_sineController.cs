using UnityEngine;

public class Enemy_sineController : MonoBehaviour
{
    public float movespeed = 5f;
    public float frequency = 10f;
    public float magnitude = 0.3f;

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

        // 앞쪽 이동
        transform.Translate(-transform.forward * movespeed * Time.deltaTime, Space.Self);

        // 좌우 흔들림 (Sin)
        float waveOffset = Mathf.Sin(elapsed * frequency) * magnitude;
        transform.Translate(Vector3.right * waveOffset, Space.Self); // 흔들림
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Destroy") || other.CompareTag("Skill"))
         SpawnEffect();
        Destroy(gameObject);
    }

    void SpawnEffect()
    {
        GameObject effect = Instantiate(effectPrefab, spawnPoint.position, spawnPoint.rotation);
        effect.transform.localScale = new Vector3(2f, 2f, 2f); // 원하는 크기로 조절
    }
}