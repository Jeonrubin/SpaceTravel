using UnityEngine;

public class EnemySpread : MonoBehaviour
{
    private Vector3 moveDir = Vector3.zero;
    private float delay = 1.2f;
    private bool canMove = false;

    public void SetDirection(Vector3 dir)
    {
        moveDir = dir.normalized;
        Invoke(nameof(EnableMovement), delay);
    }

    void EnableMovement()
    {
        canMove = true;
    }

    void Update()
    {
        if (canMove)
        {
            transform.position += moveDir * Time.deltaTime;
        }
    }
}