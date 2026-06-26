using UnityEngine;

public class SpaceSkyboxManager : MonoBehaviour
{
    [Header("Skybox 회전 설정")]
    [Tooltip("초당 회전 속도 (도 단위)")]
    [Range(0f, 5f)]
    public float rotationSpeed = 0.3f;

    [Header("사용 가능한 Skybox 리스트")]
    public Material[] skyboxMaterials; // 인스펙터에서 Skybox Material 할당

    private Material skyboxMaterial;
    private float rotation = 0f;

    void Start()
    {
        // PlayerPrefs에서 Skybox 인덱스 가져오기 (기본값 0)
        int index = PlayerPrefs.GetInt("SkyboxIndex", 0);
        Debug.Log("적용될 SkyboxIndex: " + index);

        // Skybox 배열 유효성 검사
        if (skyboxMaterials != null && skyboxMaterials.Length > 0)
        {
            // 인덱스 범위 보정
            index = Mathf.Clamp(index, 0, skyboxMaterials.Length - 1);

            // Skybox 머티리얼 생성 및 적용
            skyboxMaterial = new Material(skyboxMaterials[index]);
            RenderSettings.skybox = skyboxMaterial;

            // _Rotation 지원 여부 확인
            if (!skyboxMaterial.HasProperty("_Rotation"))
            {
                Debug.LogWarning("선택된 Skybox 머티리얼은 _Rotation 속성을 지원하지 않습니다!");
            }
        }
        else
        {
            Debug.LogWarning("Skybox Material 배열이 비어 있습니다!");
        }
    }

    void Update()
    {
        if (skyboxMaterial == null || !skyboxMaterial.HasProperty("_Rotation")) return;

        // 회전 적용
        rotation += rotationSpeed * Time.deltaTime;
        if (rotation > 360f) rotation -= 360f;
        skyboxMaterial.SetFloat("_Rotation", rotation);
    }

    // 선택한 Skybox 인덱스를 저장하고 적용하는 함수 (선택 UI 연결 시 사용 가능)
    public void SetSkyboxByIndex(int index)
    {
        if (index < 0 || index >= skyboxMaterials.Length)
        {
            Debug.LogWarning("잘못된 Skybox 인덱스: " + index);
            return;
        }

        PlayerPrefs.SetInt("SkyboxIndex", index);
        PlayerPrefs.Save();

        // 즉시 적용도 가능
        skyboxMaterial = new Material(skyboxMaterials[index]);
        RenderSettings.skybox = skyboxMaterial;
        rotation = 0f; // 회전 리셋
    }
}
