using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject effectPrefab; // 이펙트 프리팹
    public Transform spawnPoint;    // 소환 위치

    void Start()
    {
        Destroy(gameObject, 50f); // 자동 제거
    }

    void Update()
    {
        transform.Translate(-transform.forward * Time.deltaTime * 2f); // 이동
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Skill") || other.CompareTag("Blackhole"))
        {
            SpawnEffect();
            Destroy(gameObject);
        }
    }

    void SpawnEffect()
    {
        GameObject effect = Instantiate(effectPrefab, spawnPoint.position, spawnPoint.rotation);
        effect.transform.localScale = new Vector3(3f, 3f, 3f); // 원하는 크기로 조절
    }

}