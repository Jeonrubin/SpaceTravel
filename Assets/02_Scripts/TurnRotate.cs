using UnityEngine;

public class TurnRotate : MonoBehaviour
{
    public float rotateYSpeed = 20f;


    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(0, rotateYSpeed * Time.deltaTime, 0);
    }
}
