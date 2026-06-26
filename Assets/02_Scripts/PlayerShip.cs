using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    public GameObject[] shipPrefabs;  // 우주선 프리팹들

    void Start()
    {
        int shipID = PlayerPrefs.GetInt("SelectedShip", -1);

        if (shipID < 0 || shipID >= shipPrefabs.Length)
        {
            Debug.LogError("잘못된 우주선 ID입니다: " + shipID);
            return;
        }

        Instantiate(shipPrefabs[shipID], Vector3.zero, Quaternion.identity);
    }
}
