using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    private float TimeLimit;                // 제한시간
    private float TimeCount;                // 누적시간 
    public Text Timertext_limit;

    // ※※※※※※※※생존 아르마딜로 숫자 카운트 - 나중에 삭제!※※※※※※※※
    private int userNum;                                                  //※※※
    //※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※

    //※※※※※※※※TestUserNum Button을 위한 함수 - 나중에 삭제!※※※※※※※
    public void UserNumTest()                                            //※※※
    {                                                                    //※※※
        userNum -= 1;                                                    //※※※
    }
    //※※※
    //※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        TimeLimit = 30.0F;
        TimeCount = 0F;
        userNum = 4;
        Timertext_limit = GameObject.Find("TimeText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeLimit > 0F)
        {
            TimeLimit -= Time.deltaTime;
            TimeCount += Time.deltaTime;
            // 제한시간 출력
            Timertext_limit.text = "시간 : " + Mathf.Round(TimeLimit) + "초";

            if (userNum <= 0)
            {
                Timertext_limit.text = "아르마딜로 Lose!\n" + Mathf.Round(TimeCount) + "초 버텼습니다!";
                GameObject.Find("Canvas").transform.Find("GameOverUI").gameObject.SetActive(true);
                Time.timeScale = 0;
            }
        }
        else
        {
            // 아르마딜로 승리 - 버틴 시간 출력
            Timertext_limit.text = "아르마딜로 Win!";
            GameObject.Find("Canvas").transform.Find("GameOverUI").gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
