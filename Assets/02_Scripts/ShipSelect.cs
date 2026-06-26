using UnityEngine;

public class ShipSelect : MonoBehaviour
{
    public void SelectSpaceship(int index)
    {
        PlayerPrefs.SetInt("SelectedShip", index);
        PlayerPrefs.Save();
        Debug.Log("우주선 " + index + " 선택됨");
    }
}