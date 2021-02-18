using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class RightSpeaker : MonoBehaviour
{
    Vector2 MousePosition;
    public bool SpeakerRotation;
    public GameObject soundwave_center, soundwave_up, soundwave_down;

    WaveMaker_R wv, wv2, wv3;
    SpriteRenderer spr;
    Color color;

    BulletBtn bulletBtn;  // 탄막 버튼 스크립트
    public PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        SpeakerRotation = true;
        wv = GameObject.Find("rsoundwave_center").GetComponent<WaveMaker_R>();
        wv2 = GameObject.Find("rsoundwave_up").GetComponent<WaveMaker_R>();
        wv3 = GameObject.Find("rsoundwave_down").GetComponent<WaveMaker_R>();

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

                        //마우스 클릭 & 오른쪽일 경우
                        if (MousePosition.x > 0 && SpeakerRotation)
                        {
                            if (Input.GetMouseButtonDown(0) && SpeakerRotation)
                            {
                                SpeakerRotation = false;    //플래그
                                pv.RPC("Shooting_Routine", RpcTarget.All, 2f);     //코루틴을 호출합니다
                            }
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

    [PunRPC]
    public void Shooting_Routine(float fadeTime)
    {
        StartCoroutine(CoShootingRoutine(fadeTime));
    }

    IEnumerator CoShootingRoutine(float fadeTime)
    {
        pv.RPC("setColor", RpcTarget.All, 1);
        wv.power = 60.0f; wv2.power = 60.0f; wv3.power = 60.0f;
        yield return new WaitForSeconds(2);             //스킬범위 점멸 대기
        callLaunch();
        yield return new WaitForSeconds(4);
        SpeakerRotation = true;
        pv.RPC("setColor", RpcTarget.All, 0);
    }

    void callLaunch()
    {
        wv.setShootDir_right();
        wv2.setShootDir_right();
        wv3.setShootDir_right();
    }

}
