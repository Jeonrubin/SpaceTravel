using UnityEngine;

public class BlackHoleGravity : MonoBehaviour
{
    [Header("중력 설정")]
    public float gravityForce = 60f;
    public float pullRadius = 5f;
    public LayerMask playerLayer;
    public float maxPullSpeed = 10f;
    public float slowDownRadius = 0.3f;

    [Header("재활성화 시간")]
    public float reactivateDelay = 3f;

    private GameObject absorbedPlayer;

    void FixedUpdate()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, pullRadius, playerLayer);

        foreach (var col in targets)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null && rb.gameObject != absorbedPlayer)
            {
                Vector3 dir = (transform.position - rb.position);
                float distance = dir.magnitude;
                dir.Normalize();

                float forceMag = gravityForce / Mathf.Max(distance * distance, 0.1f);
                Vector3 force = dir * forceMag;

                rb.AddForce(force, ForceMode.Acceleration);

                if (rb.velocity.magnitude > maxPullSpeed)
                    rb.velocity = rb.velocity.normalized * maxPullSpeed;

                if (distance < slowDownRadius)
                {
                    float slowFactor = Mathf.InverseLerp(0f, slowDownRadius, distance);
                    rb.velocity = Vector3.Lerp(Vector3.zero, rb.velocity, slowFactor);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.position = transform.position; // ✅ 중심으로 이동

                PlayerController.IsAbsorbed = true;
                absorbedPlayer = rb.gameObject;

                Debug.Log("🌀 플레이어 블랙홀에 흡수됨");
                Invoke(nameof(ReleasePlayer), reactivateDelay);
            }
        }
    }

    void ReleasePlayer()
    {
        if (absorbedPlayer != null)
        {
            PlayerController.IsAbsorbed = false;
            absorbedPlayer = null;
            Debug.Log("✅ 플레이어 복원됨");
        }
    }
}