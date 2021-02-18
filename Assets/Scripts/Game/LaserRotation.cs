using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LaserRotation : MonoBehaviour
{
    /*
        탄환 2번 스크립트입니다.
        레이저 각도 계산 및 적용에 관련된 스크립트입니다.
    */
    BulletBtn bulletBtn;  // 탄막 버튼 스크립트
    public PhotonView pv;
    SpriteRenderer spr;

    // 레이저 회전 관련 변수들
    float angle;
    float stopAngle;
    public bool check = true;
    Vector2 target, mouse;

    public bool isFinish = true;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();

        target = transform.position;
        pv.RPC("disappear", RpcTarget.All);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("Game Scene"))
        {
            if(GameObject.Find("RoomManager")!=null)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
                {
                    bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();

                    if (bulletBtn)
                    {
                        //마우스 클릭 시 레이저 각도 고정
                        if (Input.GetMouseButtonDown(0) && bulletBtn.num == 1 && check)
                        {
                            mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;

                            pv.RPC("FixAngle", RpcTarget.All, angle);
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    public void disappear()
    {
        //레이저 소멸 함수
        Color color = spr.color;
        color.a = 0f;
        spr.color = color;
    }

    [PunRPC]
    public void FixAngle(float angle)
    {
        check = false;
        isFinish = false;
        stopAngle = angle;
        this.transform.rotation = Quaternion.AngleAxis(stopAngle - 90, Vector3.forward);

        //코루틴 호출 함수
        StartCoroutine(Cooltime());
    }

    IEnumerator Cooltime()
    {
        //레이저 발동시간 내 카메라 흔들기 및 각도 고정
        while (true)
        {
            GameObject.Find("Main Camera").GetComponent<CameraShake>().Shake();

            if(isFinish)
            {
                GameObject.Find("Main Camera").GetComponent<CameraShake>().cameraReset();
                isFinish = false;
                check = true;
                break;
            }
            yield return null;
        }
    }
}