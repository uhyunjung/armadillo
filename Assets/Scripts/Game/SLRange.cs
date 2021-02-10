using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SLRange : MonoBehaviour
{
    BulletBtn bulletBtn;  // 탄막 버튼 스크립트

    private bool check = true;
    public float timer;
    public int waitingTime;

    SpriteRenderer rangesr;
    //public GameObject SLRng;
    // private bool state;          추후에 다른 스킬들이 추가될 경우, 스킬의 발동 상태를 표시하기 위한 플래그 변수(중복 체크용)

    public void FadeIn(float fadeOutTime)
    {
        StartCoroutine(CoFadeIn(fadeOutTime));
    }

    IEnumerator CoFadeIn(float fadeOutTime)
    {
        check = false;
        rangesr = this.gameObject.GetComponent<SpriteRenderer>();
        Color tempColor = rangesr.color;
        while (tempColor.a < 0.5f)
        {
            tempColor.a += Time.deltaTime / fadeOutTime;
            rangesr.color = tempColor;

            if (tempColor.a >= 0.5f) tempColor.a = 0.5f;

            yield return null;
        }

        rangesr.color = tempColor;
        check = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();

        timer = 0.0F;
        waitingTime = 2;
        //SLRng = GameObject.Find("StageLightRange");
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && bulletBtn.num == 1 && check)
        {
            FadeIn(2f);
            timer += Time.deltaTime;
            if (timer > waitingTime)
            {
                //Action
                //SLRng.SetActive(false);
            }
        }

        //state = false;   // 추후에 추가될 플래그 변수
    }
}
