using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 2f;

    void Start()
    {
    }

    void Update()
    {
        transform.Translate(-transform.forward * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Blackhole"))
            Debug.Log(other.name + "ИэСп!");
        Destroy(gameObject);
    }
}
