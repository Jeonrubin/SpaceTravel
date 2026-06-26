using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTime : MonoBehaviour
{
    public Slider skillslider;             // 스킬 쿨타임 슬라이더
    public float fillSpeed = 0.5f;         // 쿨타임 회복 속도

    public ParticleSystem skillEffect;     // 스킬 이펙트
    public Transform playerTransform;      // 플레이어 위치

    private void Start()
    {
        // 플레이어 자동 연결
        if (playerTransform == null && PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
        }

        if (skillslider != null)
        {
            skillslider.maxValue = 1f;
            skillslider.value = 1f; // 처음엔 쿨타임이 끝난 상태
        }
        else
        {
            Debug.LogWarning("Skill slider가 연결되지 않았습니다.");
        }
    }

    private void Update()
    {
        if (skillslider == null || PlayerController.Instance == null) return;

        // 스킬 키 입력 체크
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (skillslider.value >= skillslider.maxValue)
            {
                PlayerController.Instance.TriggerSkillEffect();
                skillslider.value = 0f;

                if (skillEffect != null)
                {
                    skillEffect.Play();
                }
            }
            else
            {
                Debug.Log("스킬 쿨타임이 아직 끝나지 않았습니다.");
            }
        }

        // 쿨타임 회복
        if (skillslider.value < skillslider.maxValue)
        {
            skillslider.value += fillSpeed * Time.deltaTime;
        }
    }
}