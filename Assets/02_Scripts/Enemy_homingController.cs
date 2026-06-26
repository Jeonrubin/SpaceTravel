using UnityEngine;

public class Enemy_homingController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float rotateSpeed = 5f;
    public float predictionTime = 0.5f;

    private Rigidbody rb;
    private Transform target;
    private Vector3 lastTargetPos;
    private Vector3 targetVelocity;

    public GameObject effectPrefab; // 이펙트 프리팹
    public Transform spawnPoint;    // 소환 위치

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            target = playerObj.transform;
            lastTargetPos = target.position;
        }
        else
        {
            Debug.LogWarning("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
        
        Destroy(gameObject, 12f);
    }

    void Update()
    {
        if (target == null) return;

        targetVelocity = (target.position - lastTargetPos) / Time.deltaTime;
        lastTargetPos = target.position;

        Vector3 futurePos = target.position + targetVelocity * predictionTime;
        Vector3 direction = (futurePos - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        rb.linearVelocity = transform.forward * moveSpeed;
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
        effect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // 원하는 크기로 조절
    }
}