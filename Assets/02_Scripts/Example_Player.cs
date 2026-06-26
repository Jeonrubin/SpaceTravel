using UnityEngine;

public class Example_Player : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D 또는 ←/→
        float v = Input.GetAxisRaw("Vertical");   // W/S 또는 ↑/↓

        Vector3 moveDir = new Vector3(h, 0f, v).normalized;
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
    }
}
