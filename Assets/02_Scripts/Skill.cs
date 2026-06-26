using UnityEngine;

public class Skill : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float growSpeed = 0.5f;    // Y축 커지는 속도

    void Start()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        Destroy(gameObject, 5f);
    }
    void Update()
    {
        // 월드 Z축 방향으로 이동
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);

        // 현재 스케일 가져오기
        Vector3 scale = transform.localScale;
        
        // X, Y축 증가
        scale.x += growSpeed * Time.deltaTime;
        scale.y += growSpeed * Time.deltaTime;

        // 적용
        transform.localScale = scale;
    }
}

