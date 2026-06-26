using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject cam = GameObject.Find("Main Camera");
        if (cam != null)
        {
            if (!cam.activeSelf)
            {
                cam.SetActive(true);
                Debug.Log("[CameraManager] Main Camera를 강제로 활성화 했습니다.");
            }

            Camera cameraComp = cam.GetComponent<Camera>();
            if (cameraComp != null && !cameraComp.enabled)
            {
                cameraComp.enabled = true;
                Debug.Log("[CameraManager] Camera 컴포넌트를 활성화 했습니다.");
            }
        }
        else
        {
            Debug.LogWarning("[CameraManager] 씬에 'Main Camera'가 없습니다.");
        }
    }
}
