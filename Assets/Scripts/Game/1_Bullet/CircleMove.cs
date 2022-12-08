﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 1번 원형 탄막 이동
public class CircleMove : MonoBehaviour
{
    public PhotonView pv;
    public GameObject obj;

    SpriteRenderer circleSprite;       // 원형 탄막 스프라이트
    Vector3 scale;                     // 원형 탄막 스프라이트 크기
    Color circleColor;                 // 원형 탄막 명도
    GameObject circleBullet;           // 작은 소품 오브젝트
    GameObject circleUnit;
    public int circleBulletNum=8;          // 작은 소품 개수(8개)
    public GameObject[] circlePrefab;

    GameObject temp;                                                    // 탄막 유닛 삭제
    SpriteRenderer[] bulletSprite;                                      // 투명도 조절 스프라이트
    Color bulletColor;                                                  // 투명도 조절 색깔

    float speed = 3f;                                                   // 속도
    float currentTime = 0f;                                             // 시간 측정

    float duration = 1.0f;                                              // 회전 주기(1초)
    float rotateAngle = 360.0f;                                         // 회전 각도(360도)
    float currentAngle = 0;                                             // 현재 각도

    int count = 0;                                                      // 화면 밖으로 사라진 소품들 개수
    int random = 0;
    bool play = false;
    bool isFinish = false;                                              // 1번만 실행
    
    void Start()
    {
        bulletSprite = new SpriteRenderer[circleBulletNum];
        if(GameObject.Find("CircleMouse")!=null)
        {
            GameObject.Find("CircleMouse").GetComponent<CircleMouse>().isFinish = false;
        }
        pv.RPC("startBullet", RpcTarget.All);
    }

    [PunRPC]
    void startBullet()
    {
        StartCoroutine(makeBullet());
    }

    // 원형 탄막 발생
    IEnumerator makeBullet()
    {
        // 원형 탄막 스프라이트
        circleSprite = GetComponent<SpriteRenderer>();
        circleColor = circleSprite.color;
        scale = new Vector3(0.3f, 0.3f, 1f);
        transform.localScale = scale;
        
        // 원형 탄막 유닛 스프라이트 크기 조절
        while (scale.x < 1.5f)
        {
            scale.x += Time.deltaTime * 0.4f;
            scale.y += Time.deltaTime * 0.4f;
            transform.localScale = scale;

            // 1초에 3번 깜빡이기                    // 시간 확인
            if ((int)(scale.x / 0.1f % 2) == 0)
            {
                circleSprite.color = Color.white;
            }
            else
            {
                circleSprite.color = circleColor;
            }
            yield return null;

            if (scale.x > 1)
            {
                if (!play)
                {
                    // 작은 소품 랜덤 생성
                    for (int i = 0; i < circleBulletNum; i++)
                    {
                        random = (int)UnityEngine.Random.Range(0, 3);
                        if (pv.AmOwner)
                        {
                            pv.RPC("makeSmallBullet", RpcTarget.All, random);
                        }
                    }
                    play = true;
                    if (pv.AmOwner)
                    {
                        GameObject.Find("CircleMouse").GetComponent<CircleMouse>().cnt--;
                    }
                }
                circleSprite.enabled = false;
                
                break;
            }
        }
        yield break;
    }

    [PunRPC]
    void makeSmallBullet(int random)
    {
        circleBullet = Instantiate(circlePrefab[random], gameObject.transform.position, Quaternion.identity);
        circleBullet.transform.localScale = new Vector3(0.3f, 0.3f, 1f); // 소품 크기
        circleBullet.transform.SetParent(this.transform);
    }
    
    void Update()
    {
        if (currentTime != 0f)
        {
            currentTime += Time.deltaTime;
        }

        // 3초 후 작은 소품들 투명하게
        if (currentTime > 3f)
        {
            StartCoroutine(makeTransparent());
        }

        // 1초에 한 번 회전
        if (rotateAngle < currentAngle)
        {
            currentAngle = 0f;
        }

        // 작은 소품들 이동
        try
        {
            // 작은 소품 모두 생성 확인 && 원형 탄막 유닛 && 발사 준비 완료
            if ((!isFinish) && (play) && (this.CompareTag("BulletUnit1")))
            {
                // 시간 측정 시작
                currentTime += Time.deltaTime;

                // 작은 소품들 스프라이트 초기화
                for (int i = 0; i < circleBulletNum; i++)
                {
                    bulletSprite[i] = transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
                }

                bulletColor = bulletSprite[0].color;

                // 작은 소품 이동
                for (int i = 0; i < circleBulletNum; i++)
                {
                    transform.GetChild(i).gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(Mathf.PI * 2 * i / circleBulletNum), speed * Mathf.Sin(Mathf.PI * 2 * i / circleBulletNum));
                    StartCoroutine(makeRotate(i));
                }

                isFinish = true;
            }
        }
        catch (Exception e) { }

        // 화면 밖 탄막 삭제
        try
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if ((transform.GetChild(i).gameObject.transform.position.x > 10) || (transform.GetChild(i).gameObject.transform.position.x < -10) || (transform.GetChild(i).gameObject.transform.position.y > 5) || (transform.GetChild(i).gameObject.transform.position.y < -5))
                {
                    Destroy(transform.GetChild(i).gameObject);
                    count++;
                }
            }
        }
        catch (Exception e) { }

        // 탄막 유닛 삭제
        if(pv.AmOwner)
        {
            if ((count == circleBulletNum) && (temp != null))  // 작은 소품 모두 생성된 후 삭제 && 원형 탄막 유닛 존재
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
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
            
            for (int i = 0; i < circleBulletNum; i++)
            {
                if(bulletSprite[i] != null)
                {
                    bulletSprite[i].color = bulletColor;
                }
            }
            yield return null;
        }

        // 작은 소품들 삭제
        if (pv.AmOwner)
        {
            if (this.gameObject != null)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    // 작은 소품 1초에 1번 회전
    IEnumerator makeRotate(int n)
    {
        // 작은 소품들 회전
        while (transform.childCount > n)
        {
            // 시간 확인
            currentAngle += (rotateAngle / duration) * Time.deltaTime / 3000;
            
            transform.GetChild(n).gameObject.transform.Rotate(new Vector3(0f, 0f, currentAngle));
            
            yield return null;
        }
    }
}
