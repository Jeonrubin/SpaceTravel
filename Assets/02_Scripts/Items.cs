using UnityEngine;

public class Items : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float teleportRange = 5f; // 텔레포트 범위 설정 가능

    void Update()
    {
        transform.Translate(-transform.forward * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroy"))
        {
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats == null) return;

            if (gameObject.name.Contains("Item_Hp"))
            {
                Debug.Log("아이템 획득: 체력 회복");
                stats.Heal(30);
            }
            else if (gameObject.name.Contains("Item_Barrier"))
            {
                Debug.Log("아이템 획득: 배리어");
                stats.ActivateBarrier(6f);
            }
            else if (gameObject.name.Contains("Item_Debuff"))
            {
                Debug.Log("아이템 획득: 디버프");
                stats.ApplyDebuff(0.5f, 5f);
            }
            else if (gameObject.name.Contains("Item_Size"))
            {
                Debug.Log("아이템 획득: 크기 랜덤");
                stats.ApplySize(0.1f, 3f);
            }
            else if (gameObject.name.Contains("Item_Key"))
            {
                Debug.Log("아이템 획득: 컨트롤 반전");
                stats.ReverseControls(5f);
            }
            else if (gameObject.name.Contains("Item_Teleport"))
            {
                Debug.Log("아이템 획득: 무작위 텔레포트");
                stats.TeleportRandomly(teleportRange); // 핵심 호출
            }

            Destroy(gameObject);
        }
    }
}
