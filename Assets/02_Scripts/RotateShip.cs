using UnityEngine;

public class RotateShip : MonoBehaviour
{
    public float rotationAngle = 20f;           // 회전 각도
    public float rotationSpeed = 5f;            // 회전 속도

    private Quaternion targetRotation;          // 목표 회전

    void Start()
    {
        // 처음에는 현재 방향이 목표 방향
        targetRotation = transform.rotation;
    }

    void Update()
    {
        // 키 입력에 따라 목표 회전 각도 설정
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            targetRotation = Quaternion.Euler(0, 0, rotationAngle);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            targetRotation = Quaternion.Euler(0, 0, -rotationAngle);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            targetRotation = Quaternion.Euler(-rotationAngle, 0, 0);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            targetRotation = Quaternion.Euler(rotationAngle, 0, 0);
        }
        else
        {
            targetRotation = Quaternion.Euler(0, 0, 0);
        }

        // 현재 회전에서 목표 회전으로 부드럽게 보간
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}