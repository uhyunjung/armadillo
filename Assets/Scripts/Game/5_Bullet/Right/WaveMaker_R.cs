using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class WaveMaker_R : MonoBehaviour
{

    public int PositionNum;                //1=up, 2=down, 3=center
    float angle, stopAngle;
    public float power = 60.0f;
    Vector2 target, mouse, shootdir, first, reset;
    Rigidbody2D rb;

    BulletBtn bulletBtn;  // 탄막 버튼 스크립트
    public PhotonView pv;
    SpriteRenderer spr;
    Color color;


    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        target = transform.position;
        first = new Vector2(7.58f, 1.48f);                 //지정

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
                    int selectNum = bulletBtn.num;
                    if (selectNum == 4)
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

    [PunRPC]
    void setColor(int value)
    {
        spr = GetComponent<SpriteRenderer>();
        color = spr.color;
        color.a = value;
        spr.color = color;
    }

    private void checkPosition()                                    //마우스 좌표 및 발사 각도,방향을 정하는 함수입니다
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //mouse 위치
        angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;        //스프라이트 제어에 필요한 각도계산
        shootdir = mouse - target;                                  //음파 발사 각도 계산
        stopAngle = angle;
    }

    public void setShootDir_right()
    {
        transform.position = first;
        pv.RPC("setColor", RpcTarget.All, 1);
        gameObject.SetActive(true);

        switch (PositionNum)
        {
            case 0:                 //center
                Launch(shootdir, power);
                break;

            case 1:                 //up
                shootdir.x = shootdir.x + 2.0f;
                shootdir.y = shootdir.y + 1.0f;
                this.transform.rotation = Quaternion.AngleAxis(stopAngle - 150, Vector3.forward);
                Launch(shootdir, power);
                break;

            case 2:                 //down
                shootdir.x = shootdir.x - 2.0f;
                shootdir.y = shootdir.y - 1.0f;
                this.transform.rotation = Quaternion.AngleAxis(stopAngle - 120, Vector3.forward);
                Launch(shootdir, power);
                break;
        }

        resetbull();
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
            yield return new WaitForSeconds(1.5f);             //스킬범위 점멸 대기
            Debug.Log("초기화 실행");
            rb.velocity = Vector2.zero;                        //정지
            setShootDir_right();                                //재호출
            power += 15.0f;
        }

        gameObject.SetActive(false);
    }
}
