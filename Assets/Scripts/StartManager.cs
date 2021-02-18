using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public Button start;
    public Button setting;
    public Button finish;

    public void Start()
    {
        Screen.SetResolution(960, 540, false);  // 1920 1080
    }

    // 로비 이동
    public void OnStartClick()
    {
        SceneManager.LoadScene("Lobby Scene"); // 로비
    }

    // 환경설정 이동
    public void OnSettingClick()
    {
        SceneManager.LoadScene("Setting Scene");
    }

    // 게임 종료
    public void OnFinishClick()
    {
        Application.Quit();
    }
}
