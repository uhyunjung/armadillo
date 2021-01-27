using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1번 원형 탄막 이동
public class CircleMove : MonoBehaviour
{
    CircleFire bulletFire;                                        // 탄막 생성 스크립트
    public GameObject bulletUnit;                                       // 탄막 유닛
    GameObject temp;                                                    // 탄막 유닛 삭제
    SpriteRenderer[] bulletSprite;                                      // 투명도 조절 스프라이트
    Color bulletColor;                                                  // 투명도 조절 색깔

    float speed = 3f;                                                   // 속도
    float currentTime = 0f;                                             // 시간 측정

    float duration = 1.0f;                                              // 회전 주기(1초)
    float rotateAngle = 360.0f;                                         // 회전 각도(360도)
    float currentAngle = 0;                                             // 현재 각도

    int count = 0;                                                      // 화면 밖으로 사라진 소품들 개수
    bool isFinish = false;                                              // 1번만 실행

    void Start()
    {
        bulletFire = GameObject.Find("FollowingMouse").GetComponent<CircleFire>();

        bulletSprite = new SpriteRenderer[bulletFire.circleBulletNum];
    }

    void Update()
    {
        if(currentTime != 0f)
        {
            currentTime += Time.deltaTime;
        }

        // 3초 후 작은 소품들 투명하게
        if (currentTime > 3f)
        {
            StartCoroutine(makeTransparent());
        }

        // 1초에 한 번 회전
        if(rotateAngle < currentAngle)
        {
            currentAngle = 0f;
        }

        // 작은 소품들 이동
        try
        {
            // 작은 소품 모두 생성 확인 && 원형 탄막 유닛 && 발사 준비 완료
            if ((!isFinish) && (bulletUnit.transform.childCount == 11) && (bulletUnit.CompareTag("BulletUnit1")) && (bulletFire.isFire[bulletFire.number.Peek()] == true))
            {
                // 시간 측정 시작
                currentTime += Time.deltaTime;

                // 작은 소품들 스프라이트 초기화
                for (int i = 0; i < bulletFire.circleBulletNum; i++)
                {
                    bulletSprite[i] = bulletUnit.transform.GetChild(i+3).gameObject.GetComponent<SpriteRenderer>();
                }

                bulletColor = bulletSprite[0].color;

                // 작은 소품 이동
                for (int i = 3; i < bulletFire.circleBulletNum + 3; i++)
                {
                    bulletUnit.transform.GetChild(i).gameObject.SetActive(true);
                    bulletUnit.transform.GetChild(i).gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(Mathf.PI * 2 * i / bulletFire.circleBulletNum), speed * Mathf.Sin(Mathf.PI * 2 * i / bulletFire.circleBulletNum));
                    StartCoroutine(makeRotate(i));
                }

                bulletFire.isFire[bulletFire.number.Peek()] = false;
                bulletFire.number.Dequeue();

                isFinish = true;
            }
        }
        catch (Exception e) { }

        // 화면 밖 탄막 삭제
        try
        {
            for (int i = 1; i < bulletUnit.transform.childCount + 1; i++)
            {
                if ((bulletUnit.transform.GetChild(i).gameObject.transform.position.x > Screen.width) || (bulletUnit.transform.GetChild(i).gameObject.transform.position.x < -Screen.width) || (bulletUnit.transform.GetChild(i).gameObject.transform.position.y > Screen.height) || (bulletUnit.transform.GetChild(i).gameObject.transform.position.y < -Screen.height))
                {
                    Destroy(bulletUnit.transform.GetChild(i).gameObject);
                    count++;
                }
            }
        }
        catch (Exception e) { }

        // 탄막 유닛 삭제
        temp = GameObject.Find("CircleBulletUnit(Clone)");

        if ((count == bulletFire.circleBulletNum) && (temp != null))  // 작은 소품 모두 생성된 후 삭제 && 원형 탄막 유닛 존재
        {
            Destroy(temp);
        }
    }

    // 작은 소품들 투명하게
    IEnumerator makeTransparent()
    {
        // 작은 소품들 투명도 조절
        while (bulletColor.a > 0f)
        {
            // 0.5초 확인 필요
            bulletColor.a -= Time.deltaTime * 0.1f;
            
            for (int i = 0; i < bulletFire.circleBulletNum; i++)
            {
                if(bulletSprite[i] != null)
                {
                    bulletSprite[i].color = bulletColor;
                }
            }
            yield return null;
        }
        
        if(bulletUnit != null)
        {
            // 작은 소품들 삭제
            Destroy(bulletUnit);
            currentTime = 0;
        }
    }

    // 작은 소품 1초에 1번 회전
    IEnumerator makeRotate(int n)
    {
        // 작은 소품들 회전
        while (bulletUnit.transform.childCount > n)
        {
            // 시간 확인
            currentAngle += (rotateAngle / duration) * Time.deltaTime / 3000;
            
            bulletUnit.transform.GetChild(n).gameObject.transform.Rotate(new Vector3(0f, 0f, currentAngle));
            
            yield return null;
        }
    }
}
