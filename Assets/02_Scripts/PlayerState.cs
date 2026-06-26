using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerStats : MonoBehaviour
{
    public int hp = 100;
    public int maxHp = 100;

    public bool barrierActive = false;
    public float barrierTime = 0f;

    public float moveSpeed = 5f;
    private float debuffTime = 0f;
    private bool isDebuffed = false;

    public GameObject barrierEffectPrefab;
    private GameObject barrierEffectInstance;

    public GameObject healEffectPrefab;
    private GameObject healEffectInstance;

    public GameObject reversePrefab;
    private GameObject reverseEffectInstance;

    public AudioClip barrierSound;
    public AudioClip healSound;
    public AudioClip debuffSound;
    public AudioClip resizeSound;
    public AudioClip teleportSound;
    public AudioClip reverseSound;
    private AudioSource audioSource;

    private Transform spaceshipTransform;
    private Vector3 originalSpaceshipScale;
    private bool isResizing = false;
    private float resizeTime = 0f;

    private BoxCollider playerBoxCollider;
    private Vector3 originalColliderSize;

    public PostProcessVolume postProcessVolume;
    private Vignette vignette;
    private Coroutine vignetteCoroutine;

    private bool isReversed = false;
    private float reverseDuration = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        playerBoxCollider = GetComponent<BoxCollider>();
        if (playerBoxCollider != null)
            originalColliderSize = playerBoxCollider.size;

        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("SpaceShip") && child != transform)
            {
                spaceshipTransform = child;
                break;
            }
        }

        if (spaceshipTransform != null)
            originalSpaceshipScale = spaceshipTransform.localScale;

        if (postProcessVolume != null)
            postProcessVolume.profile.TryGetSettings(out vignette);
    }

    void Update()
    {
        if (barrierActive)
        {
            barrierTime -= Time.deltaTime;
            if (barrierTime <= 0f)
            {
                if (barrierEffectInstance != null)
                {
                    var ps = barrierEffectInstance.GetComponent<ParticleSystem>();
                    if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    barrierEffectInstance.SetActive(false);
                }
                barrierActive = false;
            }
        }

        if (isDebuffed)
        {
            debuffTime -= Time.deltaTime;
            if (debuffTime <= 0f)
            {
                isDebuffed = false;
                moveSpeed = 5f;
            }
        }

        if (isResizing && spaceshipTransform != null)
        {
            resizeTime -= Time.deltaTime;
            if (resizeTime <= 0f)
            {
                spaceshipTransform.localScale = originalSpaceshipScale;
                if (playerBoxCollider != null)
                    playerBoxCollider.size = originalColliderSize;
                isResizing = false;
            }
        }

        if (isReversed)
        {
            reverseDuration -= Time.deltaTime;
            if (reverseDuration <= 0f)
            {
                isReversed = false;
            }
        }
    }

    public void Heal(int amount)
    {
        hp += amount;
        hp = Mathf.Min(hp, maxHp);
        if (PlayerController.Instance.PlayerHP != null)
            PlayerController.Instance.PlayerHP.value = hp;

        if (audioSource != null)
            audioSource.PlayOneShot(healSound);

        if (healEffectInstance != null)
            Destroy(healEffectInstance);

        healEffectInstance = Instantiate(healEffectPrefab, transform);
        healEffectInstance.transform.localPosition = Vector3.zero;
        Destroy(healEffectInstance, 2f);
    }

    public void TakeDamage(int amount)
    {
        if (barrierActive) return;

        hp -= amount;
        hp = Mathf.Max(hp, 0);
        if (PlayerController.Instance.PlayerHP != null)
            PlayerController.Instance.PlayerHP.value = hp;

        if (hp <= 0)
        {
            Time.timeScale = 0;
            PlayerController.Instance.gameOver.SetActive(true);
            Camera.main.GetComponent<FollowCamera>().PlayCameraShake(0);
        }

        Camera.main.GetComponent<FollowCamera>().PlayCameraShake(0.5f, 0.7f);

        if (audioSource != null)
            audioSource.PlayOneShot(debuffSound);
    }

    public void ActivateBarrier(float time)
    {
        barrierActive = true;
        barrierTime = time;

        if (barrierEffectInstance != null)
            Destroy(barrierEffectInstance);

        barrierEffectInstance = Instantiate(barrierEffectPrefab, transform);
        barrierEffectInstance.transform.localPosition = new Vector3(0, -1f, 0);
        barrierEffectInstance.SetActive(true);

        if (audioSource != null)
            audioSource.PlayOneShot(barrierSound);
    }

    public void ApplyDebuff(float slowAmount, float duration)
    {
        if (isDebuffed) return;

        moveSpeed *= slowAmount;
        debuffTime = duration;
        isDebuffed = true;

        if (audioSource != null)
            audioSource.PlayOneShot(debuffSound);

        PlayVignetteEffect(1f, duration);
    }

    public void ApplySize(float minMultiplier, float duration)
    {
        if (isResizing || spaceshipTransform == null) return;

        float multiplier = Random.Range(minMultiplier, 2.0f);
        spaceshipTransform.localScale = originalSpaceshipScale * multiplier;

        if (playerBoxCollider != null)
            playerBoxCollider.size = originalColliderSize * multiplier;

        resizeTime = duration;
        isResizing = true;

        if (audioSource != null)
            audioSource.PlayOneShot(resizeSound);
    }

    public void PlayVignetteEffect(float maxIntensity, float duration)
    {
        if (vignette == null) return;

        if (vignetteCoroutine != null)
            StopCoroutine(vignetteCoroutine);

        vignetteCoroutine = StartCoroutine(VignetteRoutine(maxIntensity, duration));
    }

    private IEnumerator VignetteRoutine(float maxIntensity, float duration)
    {
        float originalIntensity = vignette.intensity.value;
        float elapsed = 0f;
        float fadeDuration = 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(originalIntensity, maxIntensity, elapsed / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(maxIntensity, originalIntensity, elapsed / fadeDuration);
            yield return null;
        }
    }

    public void RestoreFullHealth()
    {
        hp = maxHp;
        if (PlayerController.Instance.PlayerHP != null)
            PlayerController.Instance.PlayerHP.value = hp;
    }

    public void ReverseControls(float duration)
    {
        isReversed = true;
        reverseDuration = duration;

        if (audioSource != null)
            audioSource.PlayOneShot(reverseSound);

        if (reverseEffectInstance != null)
            Destroy(reverseEffectInstance);

        reverseEffectInstance = Instantiate(reversePrefab, transform);
        reverseEffectInstance.transform.localPosition = new Vector3(0, 0.5f, 0);
        Destroy(reverseEffectInstance, duration);
    }

    public bool IsControlReversed()
    {
        return isReversed;
    }

    public void TeleportRandomly(float range)
    {
        Vector3 offset = new Vector3(
            Random.Range(-range, range),
            Random.Range(-range, range),
            0f
        );

        transform.position += offset;

        Debug.Log($"플레이어가 무작위 위치로 텔레포트됨. 오프셋: {offset}");

        if (audioSource != null)
            audioSource.PlayOneShot(teleportSound);
    }
}
