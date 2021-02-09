using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmManager : MonoBehaviour
{
    /*
     탄막 2번 스피커 등장 해제 함수입니다.
     */

    BulletBtn bulletBtn; // 탄막 버튼 스크립트

    private void Start()
    {
        bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();

    }
    private void Update()
    {
        SpriteRenderer spr = GetComponent<SpriteRenderer>();        //스프라이트 렌더러 선언
        Color color = spr.color;



        if (bulletBtn.num == 1)             //활성화
        {
            Debug.Log("2번 탄환 실행");
            color.a = 1f;
            spr.color = color;            
        }
        else if (bulletBtn.num != 1)   //bulletBtn.num != 1일때 비활성화
        {
            Debug.Log("2번 탄환 종료");
            color.a = 0f;
            spr.color = color;
        }



    }


}
