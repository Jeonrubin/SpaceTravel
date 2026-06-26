using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    [Header("Shake Settings")]
    private Vector3 shakeOffset = Vector3.zero;
    private Coroutine shakeCoroutine;

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset + shakeOffset;
        }
    }

    public void PlayCameraShake(float shakeTime = 0.3f, float shakeAmount = 0.01f)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(shakeTime, shakeAmount));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            shakeOffset = new Vector3(
                Random.Range(-magnitude, magnitude),
                Random.Range(-magnitude, magnitude),
                Random.Range(-magnitude, magnitude)
            );

            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeCoroutine = null;
    }
}