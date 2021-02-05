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

    // 로비 이동(바꿔야 됨)
    public void OnStartClick()
    {
        SceneManager.LoadScene("Room Scene");
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
