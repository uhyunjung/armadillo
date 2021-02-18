using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class WaveMaker : MonoBehaviour
{
    /*
        탄막 5번 음파 생성(왼쪽) 관련 스크립트입니다. LeftSpeaker에서 제어하고 함수는 이쪽에 작성했어요.
     */

    public int PositionNum;                                             //1=up, 2=down, 0=center
    float angle, stopAngle;
    public float power = 60.0f;                                         //음파 발사 속도
    Vector2 target, mouse, shootdir, first, reset;                      //음파 각도 계산...,,,,
    Rigidbody2D rb;

    BulletBtn bulletBtn;  // 탄막 버튼 스크립트
    public PhotonView pv;
    SpriteRenderer spr;
    Color color;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        target = transform.position;                                  
        first = new Vector2(-7.69f, 1.48f);                            //음파 생성 위치 고정

        spr = GetComponent<SpriteRenderer>();        //스프라이트 렌더러 선언
        color = spr.color;
        pv.RPC("setColor", RpcTarget.All, 0);

    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("Game Scene"))
        {
            if (GameObject.Find("RoomManager") != null)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
                {
                    bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();

                    if (bulletBtn)
                    {
                        checkPosition();

                        if (Input.GetMouseButtonDown(0))
                        {
                            checkPosition();
                        }
                    }
                }
            }
        }
        
    }

    private void checkPosition()                                    //마우스 좌표 및 발사 각도,방향을 정하는 함수입니다
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //mouse 위치
        angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;        //스프라이트 제어에 필요한 각도계산
        shootdir = mouse - target;                                  //음파 발사 각도 계산
        stopAngle = angle;
    }


    public void setShootDir_left()                                  //왼쪽 음파 발사 함수
    {
        transform.position = first;                                 //음파 위치 초기화
        pv.RPC("setColor", RpcTarget.All, 1);
        gameObject.SetActive(true);                                 //음파 표시

        switch (PositionNum)
        {
            case 0:                 //center
                Launch(shootdir, power);                            //발사
                break;

            case 1:                 //up
                shootdir.x = shootdir.x + 2.0f;                     //음파 발사 방향 조정
                shootdir.y = shootdir.y + 1.0f;                     //음파 발사 방향 조정
                this.transform.rotation = Quaternion.AngleAxis(stopAngle + 30, Vector3.forward);    //음파 스프라이트 각도 제어
                Launch(shootdir, power);
                break;

            case 2:                 //down
                shootdir.x = shootdir.x - 2.0f;                     //음파 발사 방향 조정
                shootdir.y = shootdir.y - 1.0f;                     //음파 발사 방향 조정
                this.transform.rotation = Quaternion.AngleAxis(stopAngle - 30, Vector3.forward);    //음파 스프라이트 각도 제어
                Launch(shootdir, power);
                break;
        }

        resetbull();                                                //1회 발사 종료 후 reset 호출
    }



    public void Launch(Vector2 Direction, float Speed)      //음파 발사함수
    {
        rb.AddForce(Direction * Speed);
    }


    void resetbull()
    {
        StartCoroutine(CoResetWave());
    }

    IEnumerator CoResetWave()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(1.5f);              //스킬범위 점멸 대기
            Debug.Log("초기화 실행");
            rb.velocity = Vector2.zero;                         //정지
            setShootDir_left();                                 //재호출
            power += 15.0f;                                     //다음 발사 속도 증가
        }

        gameObject.SetActive(false);

    }

    [PunRPC]
    void setColor(int value)
    {
        spr = GetComponent<SpriteRenderer>();
        color.a = value;
        spr.color = color;
    }

}
