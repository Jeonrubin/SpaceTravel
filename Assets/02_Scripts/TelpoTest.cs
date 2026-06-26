using UnityEngine;

public class TelpoTest : MonoBehaviour
{
    public float teleportRange = 5f; // 텔레포트 범위

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 randomOffset = Random.insideUnitSphere * teleportRange;
            randomOffset.y = 0f;

            Vector3 newPosition = other.transform.position + randomOffset;
            other.transform.position = newPosition;

            Debug.Log("플레이어가 텔레포트됨: " + newPosition);
            Destroy(gameObject); // 아이템 제거
        }
    }
}
