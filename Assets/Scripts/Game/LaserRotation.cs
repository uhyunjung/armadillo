using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    bool check = true;
    Vector2 target, mouse;
    Color color;

    public float cool;

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();
        }
        spr = GetComponent<SpriteRenderer>();

        target = transform.position;
        pv.RPC("disappear", RpcTarget.All);
    }

    private void Update()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            pv.RPC("disappear", RpcTarget.All);

            mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;

            //마우스 클릭 시 레이저 각도 고정
            if (Input.GetMouseButtonDown(0) && bulletBtn.num == 1 && check)
            {
                pv.RPC("FixAngle", RpcTarget.All, angle);
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
        stopAngle = angle;
        this.transform.rotation = Quaternion.AngleAxis(stopAngle - 90, Vector3.forward);
        check = false;

        //코루틴 호출 함수
        StartCoroutine(Cooltime(cool));
    }

    IEnumerator Cooltime(float cool)
    {
        //레이저 발동시간 내 카메라 흔들기 및 각도 고정
        check = false;
        while (cool > 0.0f)
        {
            cool -= Time.deltaTime;
            GameObject.Find("Main Camera").GetComponent<CameraShake>().Shake();
            yield return null;
        }
        GameObject.Find("Main Camera").GetComponent<CameraShake>().cameraReset();
        check = true;
    }
}