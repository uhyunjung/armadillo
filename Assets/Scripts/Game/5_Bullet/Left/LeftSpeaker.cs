using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LeftSpeaker : MonoBehaviour
{
    Vector2 MousePosition;
    public bool SpeakerRotation;
    public GameObject soundwave_center, soundwave_up, soundwave_down;

    WaveMaker wv, wv2, wv3;
    SpriteRenderer spr;
    Color color;

    BulletBtn bulletBtn;  // 탄막 버튼 스크립트
    public PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        SpeakerRotation = true;
        wv = GameObject.Find("lsoundwave_center").GetComponent<WaveMaker>();
        wv2 = GameObject.Find("lsoundwave_up").GetComponent<WaveMaker>();
        wv3 = GameObject.Find("lsoundwave_down").GetComponent<WaveMaker>();

        spr = GetComponent<SpriteRenderer>();        //스프라이트 렌더러 선언
        color = spr.color;
        pv.RPC("setColor", RpcTarget.All, 0);

    }

    // Update is called once per frame
    void Update()
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
                        //마우스 포지션 계산
                        MousePosition = Input.mousePosition;
                        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


                        //마우스 클릭 & 왼쪽일 경우
                        if (MousePosition.x < 0 && SpeakerRotation)
                        {
                            if (Input.GetMouseButtonDown(0) && SpeakerRotation)
                            {
                                SpeakerRotation = false;    //플래그
                                Shooting_Routine(2.0f);     //코루틴을 호출합니다
                            }
                        }
                    }
                }
            }
        }




    }


    [PunRPC]
    //스프라이트 투명도 제어용 함수
    void setColor(int value)
    {
        spr = GetComponent<SpriteRenderer>();
        color.a = value;
        spr.color = color;
    }


    public void Shooting_Routine(float fadeTime)
    {
        StartCoroutine(CoShootingRoutine(fadeTime));
    }

    IEnumerator CoShootingRoutine(float fadeTime)
    {
        //음파 관련 함수 호출
        pv.RPC("setColor", RpcTarget.All, 1);
        wv.power = 60.0f; wv2.power = 60.0f; wv3.power = 60.0f;
        yield return new WaitForSeconds(2);             //스킬범위 점멸 대기
        callLaunch();
        yield return new WaitForSeconds(4);
        SpeakerRotation = true;
        pv.RPC("setColor", RpcTarget.All, 0);
    }


    void startShooting()
    {
        Shooting_Routine(2f);
    }

   
    void callLaunch()
    {
        wv.setShootDir_left();
        wv2.setShootDir_left();
        wv3.setShootDir_left();
    }

}
