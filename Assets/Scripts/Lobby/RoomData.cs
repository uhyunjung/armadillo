using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
    public string roomName = "";
    public int playerCount = 0;
    public int maxPlayer = 5;
    public string roomID = "";

    [System.NonSerialized]
    public Text roomDataTxt;

    void Awake()
    {
        roomDataTxt = GetComponentInChildren<Text>();
    }

    public void UpdateInfo()
    {
        roomDataTxt.text = string.Format(" {0} [{1}/{2}]"
                                        , roomName
                                        , playerCount.ToString("0")
                                        , maxPlayer);
    }
}
